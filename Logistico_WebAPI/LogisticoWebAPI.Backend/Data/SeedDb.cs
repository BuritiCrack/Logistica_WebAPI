using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using System.Reflection.Metadata;

namespace LogisticoWebAPI.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUsersUnitOfWork _usersUnitOfWork;


        public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork)
        {
            _context = context;
            _usersUnitOfWork = usersUnitOfWork;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckStatesAsync();
            await CheckRolesAsync();
            await CheckUserAsync(
                "1010",
                "Jose",
                "Buritica",
                "jose@yopmail.com",
                "3003432323",
                GenderEnum.Male,
                "182",
                "22",
                "Ninguna",
                "Ninguna",
                BankName.Bancolombia,
                AccountType.Savings,
                "102012121",
                UserType.Admin
                );
        }

        private async Task<User> CheckUserAsync(string document, string firsName, string lastName, string email,string phone, GenderEnum gender, string height, string age, string experiecie, string skill, BankName bank, AccountType accountType, string accountNumber, UserType userType)
        {
           var user = await _usersUnitOfWork.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Document = document,
                    FirstName = firsName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Gender = gender,
                    Height = height,
                    Age = age,
                    Experience = experiecie,
                    Skills = skill,
                    Bank = bank,
                    AccountType = accountType,
                    AccountNumber = accountNumber,
                    UserType = userType,
                    City = _context.Cities.FirstOrDefault()
                };

                await _usersUnitOfWork.AddUserAsync(user, "123456");
                await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
            await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
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