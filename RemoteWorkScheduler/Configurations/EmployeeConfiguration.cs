using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            // Customize table name
            builder.ToTable("t_employees");

            // Customize column names and properties
            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .IsRequired();

            builder.Property(e => e.TeamId)
                .HasColumnName("team_id")
                .IsRequired();
        }
    }
}
