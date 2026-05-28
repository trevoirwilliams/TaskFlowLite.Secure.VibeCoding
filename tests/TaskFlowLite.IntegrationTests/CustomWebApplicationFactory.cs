using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskFlowLite.Infrastructure.Persistence;

namespace TaskFlowLite.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TaskFlowLiteDbContext>>();

            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            services.AddSingleton(connection);

            services.AddDbContext<TaskFlowLiteDbContext>(options =>
                options.UseSqlite(connection));

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskFlowLiteDbContext>();
            dbContext.Database.EnsureCreated();
            DbSeeder.SeedAsync(dbContext).GetAwaiter().GetResult();
        });
    }
}
