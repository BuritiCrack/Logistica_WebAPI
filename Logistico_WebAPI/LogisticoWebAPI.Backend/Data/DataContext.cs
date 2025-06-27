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
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<State>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Event>().HasIndex(e => e.Name).IsUnique();
            modelBuilder.Entity<City>().HasIndex(c => new {c.StateId, c.Name }).IsUnique(); // Esto asegura que no haya dos ciudades con el mismo nombre en el mismo estado.

        }
    }
}
