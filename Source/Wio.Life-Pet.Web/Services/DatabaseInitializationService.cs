using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Wio.Life_Pet.Web.Services;

public class DatabaseInitializationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(IConfiguration configuration, ILogger<DatabaseInitializationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Starting database initialization...");
            
            // Test connection to existing SQL Server
            _logger.LogInformation("Testing connection to existing SQL Server...");
            await TestConnectionAsync();
            
            // Execute SQL script
            _logger.LogInformation("Executing SQL script...");
            await ExecuteSqlScriptAsync();
            
            _logger.LogInformation("Database initialized successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database: {Message}", ex.Message);
            throw;
        }
    }

    private async Task TestConnectionAsync()
    {
        var connectionString = GetMasterConnectionString();
        
        var maxRetries = 6;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                _logger.LogInformation("Successfully connected to SQL Server!");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Connection attempt {Attempt}/{MaxRetries} failed: {Message}", i + 1, maxRetries, ex.Message);
                if (i < maxRetries - 1)
                {
                    await Task.Delay(10000);
                }
            }
        }
        
        throw new Exception("Failed to connect to SQL Server after all retries");
    }

    private async Task ExecuteSqlScriptAsync()
    {
        var connectionString = GetMasterConnectionString();
        var scriptPath = GetScriptPath();
        
        var script = await File.ReadAllTextAsync(scriptPath);
        _logger.LogInformation("Script loaded, length: {Length} characters", script.Length);
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        // Split script by GO statements with better regex handling
        var batches = Regex.Split(script, @"^\s*GO\s*$", 
            RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Where(batch => !string.IsNullOrWhiteSpace(batch))
            .ToArray();
            
        _logger.LogInformation("Script split into {BatchCount} batches", batches.Length);
        
        for (int i = 0; i < batches.Length; i++)
        {
            var batch = batches[i].Trim();
            if (!string.IsNullOrWhiteSpace(batch))
            {
                try
                {
                    _logger.LogInformation("Executing batch {Current}/{Total}", i + 1, batches.Length);
                    _logger.LogDebug("Batch preview: {Preview}...", batch.Substring(0, Math.Min(100, batch.Length)));
                    
                    using var command = new SqlCommand(batch, connection);
                    command.CommandTimeout = 60;
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Batch {Current} executed successfully", i + 1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing batch {Current}: {Message}", i + 1, ex.Message);
                    _logger.LogDebug("Full batch content:\n--- START BATCH ---\n{Batch}\n--- END BATCH ---", batch);
                    throw;
                }
            }
        }
    }

    private string GetMasterConnectionString()
    {
        var connectionString = _configuration.GetConnectionString("Connection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Connection' not found in configuration.");
        }

        // Replace database with master for initial connection
        return connectionString.Replace("Database=life_pet", "Database=master");
    }

    private string GetScriptPath()
    {
        // Get the script path relative to the project root
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectRoot = FindProjectRoot(baseDirectory);
        return Path.Combine(projectRoot, "docker", "sql-scripts", "life_pet_script.sql");
    }

    private string FindProjectRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);
        
        while (directory != null)
        {
            // Look for solution file or compose.yaml as indicators of project root
            if (directory.GetFiles("*.slnx").Any() || 
                directory.GetFiles("compose.yaml").Any() ||
                directory.GetFiles("*.sln").Any())
            {
                return directory.FullName;
            }
            
            directory = directory.Parent;
        }
        
        // Fallback to hardcoded path if not found
        return "/home/bash/Desktop/Wio.Life-Pet";
    }
}