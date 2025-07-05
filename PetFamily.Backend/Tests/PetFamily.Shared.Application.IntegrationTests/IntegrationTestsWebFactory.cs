using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PetFamily.Web;
using Respawn;
using Testcontainers.PostgreSql;
using VolunteerWriteDbContext = PetFamily.Volunteers.Infrastructure.Database.WriteDbContext;
using VolunteerIReadDbContext = PetFamily.Volunteers.Application.Interfaces.IReadDbContext;
using VolunteerReadDbContext = PetFamily.Volunteers.Infrastructure.Database.ReadDbContext;
using SpeciesWriteDbContext = PetFamily.Species.Infrastructure.Database.WriteDbContext;
using SpeciesIReadDbContext = PetFamily.Species.Application.Interfaces.IReadDbContext;
using SpeciesReadDbContext = PetFamily.Species.Infrastructure.Database.ReadDbContext;


namespace PetFamily.Shared.Application.IntegrationTests;
//TODO подтянуть secretes.json
public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected readonly PostgreSqlContainer DbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("pet_family_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner = null!;
    private DbConnection _dbConnection = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        ReconfigureVolunteerServices(services);
        
        ReconfigureSpeciesServices(services);
    }

    private void ReconfigureVolunteerServices(IServiceCollection services)
    {
        var writeDbContext = services.SingleOrDefault(s => 
            s.ServiceType == typeof(VolunteerWriteDbContext));

        var readDbContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(VolunteerIReadDbContext));
        
        if (writeDbContext is not null)
            services.Remove(writeDbContext);
        
        if (readDbContext is not null)
            services.Remove(readDbContext);

        services.AddScoped<VolunteerWriteDbContext>(_ =>
            new VolunteerWriteDbContext(DbContainer.GetConnectionString()));
        
        services.AddScoped<VolunteerIReadDbContext, VolunteerReadDbContext>(_ =>
            new VolunteerReadDbContext(DbContainer.GetConnectionString()));
    }

    private void ReconfigureSpeciesServices(IServiceCollection services)
    {
        var writeDbContext = services.SingleOrDefault(s => 
            s.ServiceType == typeof(SpeciesWriteDbContext));

        var readDbContext = services.SingleOrDefault(s =>
            s.ServiceType == typeof(SpeciesIReadDbContext));
        
        if (writeDbContext is not null)
            services.Remove(writeDbContext);
        
        if (readDbContext is not null)
            services.Remove(readDbContext);

        services.AddScoped<SpeciesWriteDbContext>(_ =>
            new SpeciesWriteDbContext(DbContainer.GetConnectionString()));
        
        services.AddScoped<SpeciesIReadDbContext, SpeciesReadDbContext>(_ =>
            new SpeciesReadDbContext(DbContainer.GetConnectionString()));
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = []
            }
        );
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var volunteerDbContext = scope.ServiceProvider.GetRequiredService<VolunteerWriteDbContext>();
        var speciesDbContext = scope.ServiceProvider.GetRequiredService<SpeciesWriteDbContext>();
        await volunteerDbContext.Database.EnsureCreatedAsync();
        await speciesDbContext.Database.EnsureCreatedAsync();

        _dbConnection = new NpgsqlConnection(DbContainer.GetConnectionString());
        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await DbContainer.StopAsync();
        await DbContainer.DisposeAsync();
    }
}