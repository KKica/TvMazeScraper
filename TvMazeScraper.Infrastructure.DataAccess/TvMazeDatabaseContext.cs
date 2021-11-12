using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TvMazeScraper.Domain.Models.Shows;
using TvMazeScrapper.Domain.Models.Actors;

namespace TvMazeScraper.Infrastructure.DataAccess
{
    public class TvMazeDatabaseContext : DbContext
    {
        private static readonly Assembly _configurationsAssembly = Assembly.GetExecutingAssembly();
        public TvMazeDatabaseContext(DbContextOptions<TvMazeDatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(_configurationsAssembly);
        }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<Show> Shows { get; set; }
    }
}
