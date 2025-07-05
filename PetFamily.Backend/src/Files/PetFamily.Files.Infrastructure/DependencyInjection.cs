using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using PetFamily.Core.Messaging;
using PetFamily.Core.Providers;
using PetFamily.Files.Application;
using PetFamily.Files.Infrastructure.BackgroundServices;
using PetFamily.Files.Infrastructure.BackgroundServices.Services;
using PetFamily.Files.Infrastructure.MessageQueues;
using PetFamily.Files.Infrastructure.Options;
using PetFamily.Files.Infrastructure.Providers;

namespace PetFamily.Files.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFileInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        return services.AddBackgroundService()
            .AddFileProvider(configuration);
    }
    
    private static IServiceCollection AddBackgroundService(
        this IServiceCollection services)
    {
        services.AddHostedService<FilesCleanerBackgroundService>();
        services.AddScoped<IFilesCleanerService, FilesCleanerService>();
        services.AddSingleton<IMessageQueue<IEnumerable<FileIdentifier>>,
            InMemoryMessageQueue<IEnumerable<FileIdentifier>>>();

        return services;
    }

    private static IServiceCollection AddFileProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.MINIO));

        services.AddMinio(options =>
        {
            var minioOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
                               ?? throw new ApplicationException("Missing minio configuration");

            options.WithEndpoint(minioOptions.Endpoint);
            options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);
            options.WithSSL(minioOptions.UseSsl);
        });

        services.AddScoped<IFileProvider, MinioProvider>();

        return services;
    }
}