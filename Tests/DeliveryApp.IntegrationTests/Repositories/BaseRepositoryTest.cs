using DeliveryApp.Infrastructure.Adapters.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public abstract class BaseRepositoryTest : IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer;
    protected AppDbContext DbContext;
    protected IMediator Mediator;
    
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

        Mediator = Substitute.For<IMediator>();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        DbContext?.DisposeAsync().AsTask().Wait();
    }
}