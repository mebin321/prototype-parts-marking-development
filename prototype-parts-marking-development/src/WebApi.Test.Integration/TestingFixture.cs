namespace WebApi.Test.Integration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Autofac.Extensions.DependencyInjection;
    using Data;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Npgsql;
    using Respawn;
    using Respawn.Graph;
    using Xunit;

    public class TestingFixture : IAsyncLifetime
    {
        private readonly IConfigurationRoot configuration;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly Checkpoint checkpoint;

        public TestingFixture()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Testing.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            var startup = new Startup(configuration, new WebHostEnvironmentStub());
            startup.ConfigureServices(services);

            services.AddSingleton(Mock.Of<ILogger>());
            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            services.AddTransient<ILoggerFactory, NullLoggerFactory>();
            services.AddScoped<IHttpContextAccessor, StubHttpContextAccessor>();

            var providerFactory = new AutofacServiceProviderFactory();
            var builder = providerFactory.CreateBuilder(services);
            startup.ConfigureContainer(builder);
            var provider = providerFactory.CreateServiceProvider(builder);

            scopeFactory = provider.GetService<IServiceScopeFactory>();

            checkpoint = new Checkpoint
            {
                TablesToIgnore = new[]
                {
                    new Table("__EFMigrationsHistory"),
                },
                SchemasToInclude = new[]
                {
                    "public",
                },
                DbAdapter = DbAdapter.Postgres,
            };
            EnsureDatabase();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await ResetState();
        }

        public async Task ResetState()
        {
            await using var conn = new NpgsqlConnection(configuration.GetConnectionString("PPMT"));
            await conn.OpenAsync();

            await checkpoint.Reset(conn);
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = scopeFactory.CreateScope();

            return await scope.ServiceProvider.GetService<IMediator>().Send(request);
        }

        public async Task<TEntity> FindAsync<TEntity>(int id)
            where TEntity : class
        {
            return await ExecuteAsync(async c => await c.FindAsync<TEntity>(id));
        }

        public async Task<T> ExecuteAsync<T>(Func<PrototypePartsDbContext, Task<T>> func)
        {
            using var scope = scopeFactory.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetService<IDbContextFactory<PrototypePartsDbContext>>().CreateDbContext();

            return await func(dbContext);
        }

        public async Task ExecuteAsync(Func<PrototypePartsDbContext, Task> func)
        {
            using var scope = scopeFactory.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetService<IDbContextFactory<PrototypePartsDbContext>>().CreateDbContext();

            await func(dbContext);
        }

        public void Execute(Action<PrototypePartsDbContext> func)
        {
            using var scope = scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<IDbContextFactory<PrototypePartsDbContext>>().CreateDbContext();

            func(dbContext);
        }

        public async Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            await ExecuteAsync(async c =>
            {
                c.Add(entity);
                await c.SaveChangesAsync();
            });
        }

        public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            await ExecuteAsync(async c =>
            {
                c.AddRange(entities);
                await c.SaveChangesAsync();
            });
        }

        private void EnsureDatabase()
        {
            Execute(c =>
            {
                c.Database.Migrate();
            });
        }
    }
}