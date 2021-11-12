using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvMazeScrapper.Domain.Models.Actors;

namespace TvMazeScraper.Infrastructure.DataAccess.Entities.Actors.EntityConfigurtions
{
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();
        }
    }
}
