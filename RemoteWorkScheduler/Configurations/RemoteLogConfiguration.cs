using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Configurations
{
    public class RemoteLogConfiguration : IEntityTypeConfiguration<RemoteLog>
    {
        public void Configure(EntityTypeBuilder<RemoteLog> builder)
        {
            // Customize table name
            builder.ToTable("t_remote_logs");

            // Customize column names and properties
            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(r => r.Date)
                .HasColumnName("date")
                .IsRequired();

            builder.Property(r => r.EmployeeId)
                .HasColumnName("employee_id")
                .IsRequired();
        }
    }
}
