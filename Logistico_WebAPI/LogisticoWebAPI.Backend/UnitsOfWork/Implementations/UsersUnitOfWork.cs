using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
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

        public async Task<SignInResult> LoginAsync(LoginDTO model)
            => await _usersRepository.LoginAsync(model);

        public async Task LogoutAsync()
            => await _usersRepository.LogoutAsync();

        public async Task<User> GetUserAsync(Guid userId)
            => await _usersRepository.GetUserAsync(userId);

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
            => await _usersRepository.ChangePasswordAsync(user, currentPassword, newPassword);

        public async Task<IdentityResult> UpdateUserAsync(User user)
            => await _usersRepository.UpdateUserAsync(user);

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user) 
            => await _usersRepository.GenerateEmailConfirmationTokenAsync(user);

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token) 
            => await _usersRepository.ConfirmEmailAsync(user, token);

    }
}