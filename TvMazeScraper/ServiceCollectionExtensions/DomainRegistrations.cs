using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TvMazeScraper.Domain.DataAccess.Contracts.Shows;
using TvMazeScraper.Infrastructure.DataAccess;
using TvMazeScraper.Infrastructure.DataAccess.Entities.Shows.Repositories;
using TvMazeScrapper.Domain.Shows.Queries;

namespace TvMazeScraper.ServiceCollectionExtensions
{
    public static class DomainRegistrations
    {
        public static void RegisterDomain(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDatabaseContext(configuration);
            services.RegisterRepositories();
            services.RegisterMediatr();
        }

        private static void RegisterDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            string server = configuration["DbServer"];
            string user = configuration["DbUser"];
            string password = configuration["DbPassword"];
            string database = configuration["Db"];

            services.AddDbContext<TvMazeDatabaseContext>(
                options =>
                {
                    options.UseSqlServer($"Server={server};Initial Catalog={database};User Id={user}; Password={password}");
                    options.UseLoggerFactory(LoggerFactory.Create(c => c.AddConsole()));
                }
            );
        }

        private static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IShowRepository, ShowRepository>();
        }

        private static void RegisterMediatr(this IServiceCollection services)
        {
            services.AddMediatR(typeof(GetShowsWithCastQuery).GetTypeInfo().Assembly);
        }
    }
}
