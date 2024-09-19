using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            // Customize table name
            builder.ToTable("t_teams");

            // Customize column names and properties
            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(t => t.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasColumnName("description")
                .HasMaxLength(500);
        }
    }
}
