using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wio.Life_Pet.Abstraction.IApplication;
using Wio.Life_Pet.Abstraction.IRepository;
using Wio.Life_Pet.Abstraction.IServices;
using Wio.Life_Pet.Application.User;
using Wio.Life_Pet.Repository.User;
using Wio.Life_Pet.Services.User;
using Wio.Life_Pet.Services.Database;

namespace Wio.Life_Pet.Repository;

public static class ScopeInjections
{
    public static IServiceCollection ModeInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        CreatingPersistence(services, configuration);

        return services;
    }

    private static void CreatingPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection") ??
                               throw new ArgumentNullException(nameof(configuration), "Connection string 'Connection' not found.");

        // Registra a connection string como um serviço singleton
        services.AddSingleton<string>(_ => connectionString);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserApplication, UserApplication>();
        services.AddScoped<IUserService, UserService>();
        
        // Serviço de inicialização do banco de dados
        services.AddScoped<DatabaseInitializationService>();
    }
}