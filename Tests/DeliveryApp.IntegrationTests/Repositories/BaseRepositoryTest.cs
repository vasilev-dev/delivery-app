using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public abstract class BaseRepositoryTest : IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer;
    protected AppDbContext DbContext;
    
    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase(Guid.NewGuid().ToString())
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

        await _dbContainer.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString(), options => options.MigrationsAssembly(typeof(AppDbContext).Assembly))
            .UseSnakeCaseNamingConvention()
            .Options;

        DbContext = new AppDbContext(options);
        
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        DbContext?.DisposeAsync().AsTask().Wait();
    }
}