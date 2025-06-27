using LogisticoWebAPI.Shared.Entities;

namespace LogisticoWebAPI.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckStatesAsync();
        }

        private async Task CheckStatesAsync()
        {
            if (!_context.States.Any())
            {
                _context.States.Add(new State
                {
                    Name = "Amazonas",
                    Cities = [
                                new() { Name = "Leticia"},
                                new() { Name = "Puerto Nariño" },
                                new() { Name = "El Encanto" },
                             ]
                });
                _context.States.Add(new State
                {
                    Name = "Antioquia",
                    Cities = [
                                new() { Name = "Medellín" },
                                new() { Name = "Bello" },
                                new() { Name = "Itagüí" },
                                new() { Name = "Envigado" },
                             ]
                });
                _context.States.Add(new State { Name = "Arauca" });
                _context.States.Add(new State { Name = "Atlántico" });
                _context.States.Add(new State { Name = "Bolívar" });
                _context.States.Add(new State { Name = "Boyacá" });
                _context.States.Add(new State { Name = "Caldas" });
                _context.States.Add(new State { Name = "Caquetá" });
                _context.States.Add(new State { Name = "Casanare" });
                _context.States.Add(new State { Name = "Cauca" });
            }
            await _context.SaveChangesAsync();
        }
    }
}