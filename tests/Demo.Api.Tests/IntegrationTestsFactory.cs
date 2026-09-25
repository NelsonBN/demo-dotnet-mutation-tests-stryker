using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySql.Data.MySqlClient;
using System.Data;
using Testcontainers.MySql;

namespace Demo.Api.Tests;

public sealed class IntegrationTestsFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _container;

    public IntegrationTestsFactory()
        => _container = new MySqlBuilder("mysql:9.7.2")
            .WithDatabase("demo")
            .WithResourceMapping(Path.GetFullPath("./Data"), "/docker-entrypoint-initdb.d")
            .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder
            .ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IDbConnection));
                services.AddScoped<IDbConnection>(sp =>
                {
                    var connection = new MySqlConnection(_container.GetConnectionString());
                    connection.Open();

                    return connection;
                });
            });

    public async Task InitializeAsync()
        => await _container.StartAsync();

    Task IAsyncLifetime.DisposeAsync()
        => _container.StopAsync();

}

[CollectionDefinition(nameof(CollectionIntegrationTests))]
public sealed class CollectionIntegrationTests : ICollectionFixture<IntegrationTestsFactory> { }
