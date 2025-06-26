using LogisticoWebAPI.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> context) : base(context)
        {
        }

        public DbSet<State> States { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<State>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Event>().HasIndex(e => e.Name).IsUnique();

        }
    }
}
