using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> context) : base(context)
        {
        }

        public DbSet<State> States { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<WorkGroup> WorkGroups { get; set; }
        public DbSet<WorkGroupMember> WorkGroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<State>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Event>().HasIndex(e => new { e.StartDate, e.Name }).IsUnique();
            modelBuilder.Entity<City>().HasIndex(c => new { c.StateId, c.Name }).IsUnique(); // Esto asegura que no haya dos ciudades con el mismo nombre en el mismo estado.
            modelBuilder.Entity<EventUser>().HasIndex(eu => new { eu.EventId, eu.UserId }).IsUnique(); // Esto asegura que un usuario no pueda registrarse más de una vez al mismo evento.
            modelBuilder.Entity<WorkGroup>().HasIndex(wg => new { wg.EventId, wg.Name }).IsUnique();
            modelBuilder.Entity<WorkGroupMember>().HasIndex(wgm => new { wgm.WorkGroupId, wgm.UserId }).IsUnique();


            // Relaciones
            modelBuilder.Entity<WorkGroup>()
                .HasOne(wg => wg.Event)
                .WithMany(e => e.WorkGroups)
                .HasForeignKey(wg => wg.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar grupos si se elimina el evento

            modelBuilder.Entity<WorkGroup>()
                .HasOne(wg => wg.Coordinator)
                .WithMany()
                .HasForeignKey(wg => wg.CoordinatorId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar coordinador si tiene grupos

            modelBuilder.Entity<WorkGroupMember>()
                .HasOne(wgm => wgm.WorkGroup)
                .WithMany(wg => wg.Members)
                .HasForeignKey(wgm => wgm.WorkGroupId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar miembros si se elimina el grupo

            modelBuilder.Entity<WorkGroupMember>()
                .HasOne(wgm => wgm.User)
                .WithMany(u => u.Members)
                .HasForeignKey(wgm => wgm.UserId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar usuario si es miembro de un grupo
        } 
    }
}
