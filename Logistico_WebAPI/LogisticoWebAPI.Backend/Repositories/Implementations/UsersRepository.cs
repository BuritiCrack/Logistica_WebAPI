using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UsersRepository(DataContext context, UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
         
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.City!)
                .ThenInclude(c => c.State!)
                .FirstOrDefaultAsync(x => x.Id == userId.ToString());
            return user!;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if(!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.City!)
                .ThenInclude(c => c.State)
                .FirstOrDefaultAsync(u => u.Email == email);
            return user!;
        }

        public Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await  _context.Users
                .OrderByDescending(u => u.IsActive)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination)
        {
            var query = _context.Users.AsQueryable();

            //TODO: Implementar filtros de busqueda

            return new ActionResponse<IEnumerable<User>>
            {
                WasSuccess = true,
                Result = await query
                .OrderByDescending(u => u.UserType == UserType.Admin)
                .OrderByDescending(u => u.IsActive)
                .ThenBy(u => u.FirstName)
                .ThenBy(x => x.LastName)
                .Paginate(pagination)
                .ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var query = _context.Users.AsQueryable();

            double count = await query.CountAsync();
            double totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)totalPages
            };
        }
    }
}
