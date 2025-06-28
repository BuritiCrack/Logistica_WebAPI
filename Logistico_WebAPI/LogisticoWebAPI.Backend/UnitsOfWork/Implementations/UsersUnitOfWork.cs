using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Implementations
{
    public class UsersUnitOfWork : IUsersUnitOfWork
    {
        private readonly IUsersRepository _usersRepository;

        public UsersUnitOfWork(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public virtual async Task<IdentityResult> AddUserAsync(User user, string password) 
            => await _usersRepository.AddUserAsync(user, password);

        public virtual async Task AddUserToRoleAsync(User user, string roleName) 
            => await _usersRepository.AddUserToRoleAsync(user, roleName);

        public virtual async Task CheckRoleAsync(string roleName) 
            => await _usersRepository.CheckRoleAsync(roleName);

        public virtual async Task<User> GetUserAsync(string email) 
            => await _usersRepository.GetUserAsync(email);

        public virtual async Task<bool> IsUserInRoleAsync(User user, string roleName) 
            => await _usersRepository.IsUserInRoleAsync(user, roleName);
    }
}

