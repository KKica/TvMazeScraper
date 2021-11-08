using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvMazeScraper.Domain.Models.ActorShows;
using TvMazeScraper.Domain.Models.Shows;

namespace TvMazeScraper.Infrastructure.DataAccess.Entities.Shows.EntityConfigurations
{
    public class ShowConfiguration : IEntityTypeConfiguration<Show>
    {
        public void Configure(EntityTypeBuilder<Show> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.HasMany(x => x.Cast)
                .WithMany(x => x.Shows)
                .UsingEntity<ActorShow>(
                    j => j
                        .HasOne(sa => sa.Actor)
                        .WithMany(a => a.ActorShows)
                        .HasForeignKey(sa => sa.ActorId),
                    j => j
                        .HasOne(sa => sa.Show)
                        .WithMany(s => s.CastShows)
                        .HasForeignKey(sa => sa.ShowId)
                );
        }
    }
}
