using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using TvMazeScraper.Infrastructure.DataAccess;

namespace TvMazeScraper.Infrastructure.Migrations
{
    public class TvMazeScraperDbContextFactory : IDesignTimeDbContextFactory<TvMazeDatabaseContext>
    {
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        private static readonly string _assemblyName = "TvMazeScraper.Infrastructure.Migrations";

        public TvMazeDatabaseContext CreateDbContext(string[] args)
        {
            string server = configuration["DbServer"];
            string port = configuration["DbPort"];
            string user = configuration["DbUser"];
            string password = configuration["DbPassword"];
            string database = configuration["Db"];

            string connectionString = $"Server={server},{port};Initial Catalog={database};User Id={user}; Password={password}";

            var builder = new DbContextOptionsBuilder<TvMazeDatabaseContext>();

            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(_assemblyName));

            return new TvMazeDatabaseContext(builder.Options);
        }
    }
}
