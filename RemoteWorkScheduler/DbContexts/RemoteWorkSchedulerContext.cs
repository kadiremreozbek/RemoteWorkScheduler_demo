using Microsoft.EntityFrameworkCore;
using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.DbContexts
{
    public class RemoteWorkSchedulerContext : DbContext
    {
        public RemoteWorkSchedulerContext(DbContextOptions<RemoteWorkSchedulerContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<RemoteLog> RemoteLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations in the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RemoteWorkSchedulerContext).Assembly);
        }
    }
}
