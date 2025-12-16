using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace LogisticoWebAPI.Backend.Controllers
{
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly IMailHelper _mailHelper;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly string _container;
        private readonly string _containerQr;

        public AccountsController(IUsersUnitOfWork usersUnitOfWork, IConfiguration configuration,
            IFileStorage fileStorage, IMailHelper mailHelper, IEmailTemplateService emailTemplateService)
        {
            _usersUnitOfWork = usersUnitOfWork;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _mailHelper = mailHelper;
            _container = "users";
            _containerQr = "qrcodes";
            _emailTemplateService = emailTemplateService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _usersUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _usersUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            try
            {
                var users = await _usersUnitOfWork.GetAllUsersAsync();
                var usersDTO = users.Select(u => new
                {
                    u.Document,
                    u.FirstName,
                    u.LastName,
                    u.FullName,
                    u.PhoneNumber,
                    u.Email,
                    u.EmailConfirmed,
                    u.Photo,
                    u.Age,
                    u.Id,
                    u.PensionFund,
                    u.LockoutEnd,
                    u.UserType,
                    u.IsActive
                });

                return Ok(usersDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener los usuarios: {ex.Message}");
            }
        }

        [HttpGet("user/{id:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserAsync(string id)
        {
            var user = await _usersUnitOfWork.GetUserAsync(new Guid(id));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("currentuser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var currentUser = new CurrentUserDTO
            {
                UserId = user.Id.ToString(),
                FullName = user.FullName,
                UserType = user.UserType.ToString()
            };
            return Ok(currentUser);
        }

        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var myToken = await _usersUnitOfWork.GeneratePasswordResetTokenAsync(user);
            var tokenLink = Url.Action("ResetPassword", "accounts", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

            var htmlBody = _emailTemplateService.GeneratePasswordRecoveryTemplete(user, tokenLink!);

            var response = _mailHelper.SendEmail(user.FullName, user.Email!,
                "Sistema Logístico - Recuperación de contraseña", htmlBody);

            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _usersUnitOfWork.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }

        [HttpPost("ResedToken")]
        public async Task<IActionResult> ResedTokenAsync([FromBody] EmailDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var response = await SendConfirmationEmailAsync(user);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _usersUnitOfWork.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }

            return NoContent();
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutAsync(User user)
        {
            try
            {
                var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
                if (currentUser == null)
                {
                    return NotFound();
                }

                var oldQrCodePath = currentUser.QrCode;

                bool needsQrCodeUpdate = ShouldUpdateQrCode(currentUser, user);

                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser = Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
                }

                currentUser.Document = user.Document;
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Gender = user.Gender;
                currentUser.Height = user.Height;
                currentUser.Age = user.Age;
                currentUser.Experience = user.Experience;
                currentUser.Skills = user.Skills;
                currentUser.Bank = user.Bank;
                currentUser.AccountType = user.AccountType;
                currentUser.Eps = user.Eps;
                currentUser.PensionFund = user.PensionFund;
                currentUser.Address = user.Address;
                currentUser.PhoneNumber = user.PhoneNumber;
                currentUser.TShirtSize = user.TShirtSize;
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
                currentUser.CityId = user.CityId;

                if (needsQrCodeUpdate)
                {
                    await UpdateUserQrCodeAsync(currentUser, oldQrCodePath);
                }

                var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    return Ok(BuildToken(currentUser));
                }

                return BadRequest(result.Errors.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool ShouldUpdateQrCode(User currentUser, User user)
        {
            return currentUser.Document != user.Document ||
                   currentUser.FirstName != user.FirstName ||
                   currentUser.LastName != user.LastName ||
                   currentUser.Email != user.Email ||
                   currentUser.UserType != user.UserType;
        }

        private async Task UpdateUserQrCodeAsync(User currentUser, string? oldQrCodePath = null)
        {
            try
            {
                var qrCodeBase64 = GenerateUserQrCode(currentUser);
                currentUser.QrCode = await _fileStorage.SaveFileAsync(Convert.FromBase64String(qrCodeBase64), ".png", _containerQr);

                if (!string.IsNullOrEmpty(oldQrCodePath))
                {
                    try
                    {
                        await _fileStorage.RemoveFileAsync(oldQrCodePath, _containerQr);
                    }
                    catch (Exception ex)
                    {
                        BadRequest(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(oldQrCodePath))
                {
                    currentUser.QrCode = oldQrCodePath;
                }

                throw new InvalidOperationException("Error al generar código QR", ex);
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutState(string id)
        {
            try
            {
                var user = await _usersUnitOfWork.GetUserAsync(new Guid(id));
                if (user == null)
                {
                    return NotFound();
                }

                if (!user.EmailConfirmed)
                {
                    if (!user.IsActive)
                    {
                        return BadRequest("El usuario no ha confirmado su correo electrónico, no se puede activar.");
                    }
                    else
                    {
                        // Si está activo pero sin confirmación, forzar desactivación
                        user.IsActive = false;
                        await _usersUnitOfWork.UpdateUserAsync(user);
                        return BadRequest("Usuario desactivado por falta de verificación de correo");
                    }
                }

                user.IsActive = !user.IsActive;

                if (user.IsActive)
                {
                    user.LockoutEnd = DateTime.UtcNow;
                }

                var result = await _usersUnitOfWork.UpdateUserAsync(user);
                if (result.Succeeded)
                {
                    return NoContent();
                }
                return BadRequest(result.Errors.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("edituser/{id:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutAsync(string id, User user)
        {
            try
            {
                var currentUser = await _usersUnitOfWork.GetUserAsync(new Guid(id));
                if (currentUser == null)
                {
                    return NotFound();
                }

                var userMaster = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
                if (userMaster == null)
                {
                    return NotFound();
                }

                var oldQrCodePath = currentUser.QrCode;
                var oldUserType = currentUser.UserType;

                bool needsQrCodeUpdate = ShouldUpdateQrCode(currentUser, user);

                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser = Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
                }

                currentUser.Document = user.Document;
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Gender = user.Gender;
                currentUser.Height = user.Height;
                currentUser.Age = user.Age;
                currentUser.Experience = user.Experience;
                currentUser.Skills = user.Skills;
                currentUser.Bank = user.Bank;
                currentUser.AccountType = user.AccountType;
                currentUser.Eps = user.Eps;
                currentUser.PensionFund = user.PensionFund;
                currentUser.Address = user.Address;
                currentUser.PhoneNumber = user.PhoneNumber;
                currentUser.IsActive = user.IsActive;
                currentUser.TShirtSize = user.TShirtSize;
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
                currentUser.CityId = user.CityId;

                if (userMaster.UserType == UserType.SuperAdmin)
                {
                    currentUser.UserType = user.UserType;

                    if (oldUserType != user.UserType)
                    {
                        needsQrCodeUpdate = true;
                    }
                }

                if (needsQrCodeUpdate)
                {
                    await UpdateUserQrCodeAsync(currentUser, oldQrCodePath);
                }

                var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    if (currentUser.UserType != user.UserType)
                    {
                        await _usersUnitOfWork.AddUserToRoleAsync(currentUser, currentUser.UserType.ToString());
                    }
                    return NoContent();
                }

                return BadRequest(result.Errors.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!));
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;
            if (!string.IsNullOrEmpty(model.Photo))
            {
                var phothoUser = Convert.FromBase64String(model.Photo);
                model.Photo = await _fileStorage.SaveFileAsync(phothoUser, ".jpg", _container);
            }

            var qrCodeBase64 = GenerateUserQrCode(user);
            user.QrCode = await _fileStorage.SaveFileAsync(Convert.FromBase64String(qrCodeBase64), ".png", _containerQr);

            var result = await _usersUnitOfWork.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _usersUnitOfWork.AddUserToRoleAsync(user, user.UserType.ToString());
                var response = await SendConfirmationEmailAsync(user);
                if (response.WasSuccess)
                {
                    return NoContent();
                }

                return BadRequest(response.Message);
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }

        private string GenerateUserQrCode(User user)
        {
            var userQrData = new
            {
                Name = user.FullName,
                Document = user.Document,
                Email = user.Email,
                UserType = user.UserType.ToString(),
                GeneratedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
            };

            var qrCodeInfo = JsonSerializer.Serialize(userQrData, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            try
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrCodeInfo, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new BitmapByteQRCode(qrCodeData);

                string qrCodeImage = Convert.ToBase64String(qrCode.GetGraphic(10));
                return qrCodeImage;
            }
            catch (Exception)
            {
                var basicInfo = $"User: {user.Document} - {user.FullName}";
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(basicInfo, QRCodeGenerator.ECCLevel.L);
                var qrCode = new BitmapByteQRCode(qrCodeData);

                return Convert.ToBase64String(qrCode.GetGraphic(10));
            }
        }

        private async Task<ActionResponse<string>> SendConfirmationEmailAsync(User user)
        {
            var myToken = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "accounts", new
            {
                userId = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

            var htmlBody = _emailTemplateService.GenerateEmailConfirmationTemplate(user, tokenLink!);

            return _mailHelper.SendEmail(user.FullName!, user.Email!,
                "Sistema Logístico - Confirma tu correo electrónico", htmlBody);
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            token = token.Replace(" ", "+");
            var user = await _usersUnitOfWork.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            var result = await _usersUnitOfWork.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }

            return NoContent();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
        {
            var result = await _usersUnitOfWork.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _usersUnitOfWork.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }
            if (result.IsLockedOut)
            {
                return BadRequest("Usuario bloqueado, intente de nuevo en 5 minutos");
            }
            if (result.IsNotAllowed)
            {
                return BadRequest("Tu cuenta aún no ha sido activada. Por favor revisa tu correo electrónico y sigue el enlace de confirmación para completar el registro.");
            }

            return BadRequest("Email o contraseña incorrectos");
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email!),
                new(ClaimTypes.Role, user.UserType.ToString()),
                new("Document", user.Document),
                new("FirstName", user.FirstName),
                new("LastName", user.LastName),
                new("Address", user.Address),
                new("Photo", user.Photo ?? string.Empty),
                new("CityId", user.CityId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}