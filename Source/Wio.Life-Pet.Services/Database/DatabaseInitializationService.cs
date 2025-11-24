using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Wio.Life_Pet.Services.Database;

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
            
            // Testar conexão com o SQL Server existente
            _logger.LogInformation("Testando conexão com o SQL Server existente...");
            await TestConnectionAsync();
            
            // Executar script SQL sempre (forçado)
            _logger.LogInformation("Executando script SQL (FORÇADO)...");
            await ExecuteSqlScriptAsync();
            
            _logger.LogInformation("Banco de dados inicializado com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar o banco de dados: {Message}", ex.Message);
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
                _logger.LogInformation("Conectado com sucesso ao SQL Server!");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Tentativa de conexão {Attempt}/{MaxRetries} falhou: {Message}", i + 1, maxRetries, ex.Message);
                if (i < maxRetries - 1)
                {
                    await Task.Delay(10000);
                }
            }
        }
        
        throw new Exception("Falha ao conectar ao SQL Server após todas as tentativas");
    }

    private async Task ExecuteSqlScriptAsync()
    {
        var connectionString = GetMasterConnectionString();
        var scriptPath = GetScriptPath();
        
        var script = await File.ReadAllTextAsync(scriptPath);
        _logger.LogInformation("Script carregado, comprimento: {Length} caracteres", script.Length);
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        // Dividir script por instruções GO com melhor tratamento de regex
        var batches = Regex.Split(script, @"^\s*GO\s*$", 
            RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Where(batch => !string.IsNullOrWhiteSpace(batch))
            .ToArray();
            
        _logger.LogInformation("Script dividido em {BatchCount} lotes", batches.Length);
        
        for (int i = 0; i < batches.Length; i++)
        {
            var batch = batches[i].Trim();
            if (!string.IsNullOrWhiteSpace(batch))
            {
                try
                {
                    _logger.LogInformation("Executando lote {Current}/{Total}", i + 1, batches.Length);
                    _logger.LogDebug("Visualização do lote: {Preview}...", batch.Substring(0, Math.Min(100, batch.Length)));
                    
                    using var command = new SqlCommand(batch, connection);
                    command.CommandTimeout = 60;
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Lote {Current} executado com sucesso", i + 1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar lote {Current}: {Message}", i + 1, ex.Message);
                    _logger.LogDebug("Conteúdo completo do lote:\n--- INÍCIO DO LOTE ---\n{Batch}\n--- FIM DO LOTE ---", batch);
                    throw;
                }
            }
        }
    }

    private string GetMasterConnectionString()
    {
        var connectionString = _configuration.GetConnectionString("Connection");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Conexão 'Connection' não encontrada na configuração.");

        // Substituir o banco de dados por master para a conexão inicial
        return connectionString.Replace("Database=life_pet", "Database=master");
    }

    private string GetScriptPath()
    {
        // Obter o caminho do script relativo à raiz do projeto
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectRoot = FindProjectRoot(baseDirectory);
        return Path.Combine(projectRoot, "docker", "sql-scripts", "life_pet_script.sql");
    }

    private string FindProjectRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);
        
        while (directory != null)
        {
            // Procurar arquivo de solução ou compose.yaml como indicadores da raiz do projeto
            if (directory.GetFiles("*.slnx").Any() || 
                directory.GetFiles("compose.yaml").Any() ||
                directory.GetFiles("*.sln").Any())
            {
                return directory.FullName;
            }
            
            directory = directory.Parent;
        }
        
        // Se não encontrar, retornar um caminho padrão ou lançar uma exceção
        return "/home/bash/Desktop/Wio.Life-Pet";
    }
}