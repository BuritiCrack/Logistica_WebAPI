using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly string _container;

        public AccountsController(IUsersUnitOfWork usersUnitOfWork, IConfiguration configuration,
            IFileStorage fileStorage, IMailHelper mailHelper)
        {
            _usersUnitOfWork = usersUnitOfWork;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _mailHelper = mailHelper;
            _container = "users";
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

            var htmlBody = GeneratePasswordRecoveryTemplate(user, tokenLink!);

            var response = _mailHelper.SendEmail(user.FullName, user.Email!,
                "Sistema Logístico - Recuperación de contraseña", htmlBody);

            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        private string GeneratePasswordRecoveryTemplate(User user, string tokenLink)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='es'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Recuperación de Contraseña</title>
            <style>
                body {{
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    box-shadow: 0 0 10px rgba(0,0,0,0.1);
                    border-radius: 15px;
                    overflow: hidden;
                }}
                .header {{
                    background: linear-gradient(135deg, #dc3545 0%, #c82333 50%, #bd2130 100%);
                    color: white;
                    text-align: center;
                    padding: 40px 20px;
                    border-top-left-radius: 15px;
                    border-top-right-radius: 15px;
                }}
                .header h1 {{
                    margin: 0;
                    font-size: 28px;
                    font-weight: 600;
                }}
                .content {{
                    padding: 40px 30px;
                    text-align: center;
                }}
                .greeting {{
                    font-size: 18px;
                    color: #333;
                    margin-bottom: 20px;
                }}
                .message {{
                    font-size: 16px;
                    color: #666;
                    margin-bottom: 30px;
                    line-height: 1.8;
                }}
                .cta-button {{
                    display: inline-block;
                    background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
                    color: white;
                    text-decoration: none;
                    padding: 15px 30px;
                    border-radius: 8px;
                    font-weight: 600;
                    font-size: 16px;
                    margin: 20px 0;
                    transition: transform 0.2s ease;
                }}
                .cta-button:hover {{
                    transform: translateY(-2px);
                    color: white;
                    text-decoration: none;
                }}
                .warning-note {{
                    background-color: #fff3cd;
                    border-left: 4px solid #ffc107;
                    padding: 15px;
                    margin: 20px 0;
                    border-radius: 4px;
                }}
                .security-note {{
                    background-color: #f8d7da;
                    border-left: 4px solid #dc3545;
                    padding: 15px;
                    margin: 20px 0;
                    border-radius: 4px;
                }}
                .icon {{
                    font-size: 48px;
                    margin-bottom: 20px;
                }}
                /* Footer */
                .footer {{
                    background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
                    color: #cbd5e1;
                    padding: 60px 50px;
                    text-align: center;
                    position: relative;
                    border-bottom-left-radius: 15px;
                    border-bottom-right-radius: 15px;
                }}

                .footer::before {{
                    content: '';
                    position: absolute;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: url('data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100""><defs><pattern id=""footerdots"" width=""25"" height=""25"" patternUnits=""userSpaceOnUse""><circle cx=""12.5"" cy=""12.5"" r=""1"" fill=""rgba(255, 255, 255, 0.05)""/></pattern></defs><rect width=""100"" height=""100"" fill=""url(%23footerdots)""/></svg>') repeat;
                    opacity: 0.3;
                    border-bottom-left-radius: 15px;
                    border-bottom-right-radius: 15px;
                }}

                .footer > * {{
                    position: relative;
                    z-index: 1;
                }}

                .footer-title {{
                    font-size: 24px;
                    font-weight: 700;
                    color: #ffffff;
                    margin: 0 0 12px 0;
                }}

                .footer-description {{
                    font-size: 16px;
                    margin: 0 0 35px 0;
                    opacity: 0.9;
                }}

                .footer-links {{
                    margin-bottom: 35px;
                }}

                .footer-link {{
                    color: #94a3b8;
                    text-decoration: none;
                    font-size: 15px;
                    margin: 0 20px;
                    transition: all 0.3s ease;
                    padding: 8px 0;
                }}

                .footer-link:hover {{
                    color: #ffffff;
                    text-decoration: none;
                    text-shadow: 0 2px 4px rgba(255, 255, 255, 0.2);
                }}

                .footer-bottom {{
                    border-top: 1px solid #475569;
                    padding-top: 30px;
                    font-size: 14px;
                    opacity: 0.8;
                }}

                @media only screen and (max-width: 600px) {{
                    .container {{
                        width: 100% !important;
                    }}
                    .content {{
                        padding: 20px !important;
                    }}
                    .header {{
                        padding: 30px 20px !important;
                    }}
                }}
                /* Responsive Design */
                @media only screen and (max-width: 640px) {{
                    .footer {{ padding: 40px 25px; }}
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <div class='icon'>🔑</div>
                    <h1>Recuperación de Contraseña</h1>
                    <p>Sistema Logístico</p>
                </div>

                <div class='content'>
                    <h2 class='greeting'>¡Hola {user.FirstName}!</h2>

                    <p class='message'>
                        Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en nuestro sistema logístico.
                        Si no realizaste esta solicitud, puedes ignorar este correo de forma segura.
                    </p>

                    <div class='warning-note'>
                        <strong>⚠️ Importante:</strong><br>
                        Este enlace expirará en 24 horas por razones de seguridad. Si necesitas más tiempo,
                        puedes solicitar un nuevo enlace de recuperación.
                    </div>

                    <a href='{tokenLink}' class='cta-button'>
                        🔓 Restablecer mi contraseña
                    </a>

                    <div class='security-note'>
                        <strong>🚨 Medidas de seguridad:</strong><br>
                        • Solo tú puedes usar este enlace<br>
                        • El enlace es válido por 24 horas únicamente<br>
                        • Si no solicitaste este cambio, contacta a soporte inmediatamente
                    </div>

                    <p style='color: #666; font-size: 14px; margin-top: 30px;'>
                        <strong>¿Problemas con el botón?</strong><br>
                        Copia y pega este enlace en tu navegador:<br>
                        <span style='word-break: break-all; color: #dc3545; font-family: monospace;'>{tokenLink}</span>
                    </p>
                </div>

                <!-- Footer -->
                <div class='footer'>
                    <h4 class='footer-title'>Sistema Logístico Empresarial</h4>
                    <p class='footer-description'>Soluciones tecnológicas para la gestión logística moderna</p>

                    <div class='footer-links'>
                        <a href='#' class='footer-link'>Centro de Ayuda</a>
                        <a href='#' class='footer-link'>Términos de Servicio</a>
                        <a href='#' class='footer-link'>Política de Privacidad</a>
                        <a href='#' class='footer-link'>Contacto</a>
                    </div>

                    <div class='footer-bottom'>
                        <p>&copy; {DateTime.Now.Year} Sistema Logístico. Todos los derechos reservados.</p>
                        <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
                    </div>
                </div>
            </div>
        </body>
        </html>";
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
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
                currentUser.CityId = user.CityId;

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
                bool isEqual = userMaster.Email == "jose@yopmail.com";
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
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
                currentUser.CityId = user.CityId;

                if (isEqual)
                {
                    currentUser.UserType = user.UserType;
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

        private async Task<ActionResponse<string>> SendConfirmationEmailAsync(User user)
        {
            var myToken = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "accounts", new
            {
                userId = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

            var htmlBody = GenerateEmailConfirmationTemplate(user, tokenLink!);

            return _mailHelper.SendEmail(user.FullName!, user.Email!,
                "Sistema Logístico - Confirma tu correo electrónico", htmlBody);
        }

        private string GenerateEmailConfirmationTemplate(User user, string tokenLink)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='es'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Confirmación de Correo Electrónico</title>
            <style>
                body {{
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    box-shadow: 0 0 10px rgba(0,0,0,0.1);
                    border-radius: 15px;
                    overflow: hidden;
                }}
                .header {{
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    text-align: center;
                    padding: 40px 20px;
                    border-top-left-radius: 15px;
                    border-top-right-radius: 15px;
                }}
                .header h1 {{
                    margin: 0;
                    font-size: 28px;
                    font-weight: 600;
                }}
                .content {{
                    padding: 40px 30px;
                    text-align: center;
                }}
                .greeting {{
                    font-size: 18px;
                    color: #333;
                    margin-bottom: 20px;
                }}
                .message {{
                    font-size: 16px;
                    color: #666;
                    margin-bottom: 30px;
                    line-height: 1.8;
                }}
                .cta-button {{
                    display: inline-block;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    text-decoration: none;
                    padding: 15px 30px;
                    border-radius: 8px;
                    font-weight: 600;
                    font-size: 16px;
                    margin: 20px 0;
                    transition: transform 0.2s ease;
                }}
                .cta-button:hover {{
                    transform: translateY(-2px);
                    color: white;
                    text-decoration: none;
                }}
                /* Footer */
                .footer {{
                    background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
                    color: #cbd5e1;
                    padding: 60px 50px;
                    text-align: center;
                    position: relative;
                    border-bottom-left-radius: 15px;
                    border-bottom-right-radius: 15px;
                }}

                .footer::before {{
                    content: '';
                    position: absolute;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: url('data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100""><defs><pattern id=""footerdots"" width=""25"" height=""25"" patternUnits=""userSpaceOnUse""><circle cx=""12.5"" cy=""12.5"" r=""1"" fill=""rgba(255, 255, 255, 0.05)""/></pattern></defs><rect width=""100"" height=""100"" fill=""url(%23footerdots)""/></svg>') repeat;
                    opacity: 0.3;
                    border-bottom-left-radius: 15px;
                    border-bottom-right-radius: 15px;
                }}

                .footer > * {{
                    position: relative;
                    z-index: 1;
                }}

                .footer-title {{
                    font-size: 24px;
                    font-weight: 700;
                    color: #ffffff;
                    margin: 0 0 12px 0;
                }}

                .footer-description {{
                    font-size: 16px;
                    margin: 0 0 35px 0;
                    opacity: 0.9;
                }}

                .footer-links {{
                    margin-bottom: 35px;
                }}

                .footer-link {{
                    color: #94a3b8;
                    text-decoration: none;
                    font-size: 15px;
                    margin: 0 20px;
                    transition: all 0.3s ease;
                    padding: 8px 0;
                }}

                .footer-link:hover {{
                    color: #ffffff;
                    text-decoration: none;
                    text-shadow: 0 2px 4px rgba(255, 255, 255, 0.2);
                }}

                .footer-bottom {{
                    border-top: 1px solid #475569;
                    padding-top: 30px;
                    font-size: 14px;
                    opacity: 0.8;
                }}
                .security-note {{
                    background-color: #e3f2fd;
                    border-left: 4px solid #2196f3;
                    padding: 15px;
                    margin: 20px 0;
                    border-radius: 4px;
                }}
                .icon {{
                    font-size: 48px;
                    margin-bottom: 20px;
                }}
                @media only screen and (max-width: 600px) {{
                    .container {{
                        width: 100% !important;
                    }}
                    .content {{
                        padding: 20px !important;
                    }}
                    .header {{
                        padding: 30px 20px !important;
                    }}
                }}
                /* Responsive Design */
                @media only screen and (max-width: 640px) {{
                    .footer {{ padding: 40px 25px; }}
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <div class='icon'>📧</div>
                    <h1>Confirmación de Correo</h1>
                    <p>Sistema Logístico</p>
                </div>

                <div class='content'>
                    <h2 class='greeting'>¡Hola {user.FirstName}!</h2>

                    <p class='message'>
                        Gracias por registrarte en nuestro sistema logístico.
                        Para completar tu registro y activar tu cuenta, necesitamos
                        verificar tu dirección de correo electrónico.
                    </p>

                    <a href='{tokenLink}' class='cta-button'>
                        ✓ Confirmar mi correo electrónico
                    </a>

                    <div class='security-note'>
                        <strong>🔒 Nota de seguridad:</strong><br>
                        Si no te registraste en nuestro sistema, puedes ignorar este correo de forma segura.
                    </div>
                </div>

                <!-- Footer -->
                <div class='footer'>
                    <h4 class='footer-title'>Sistema Logístico Empresarial</h4>
                    <p class='footer-description'>Soluciones tecnológicas para la gestión logística moderna</p>

                    <div class='footer-links'>
                        <a href='#' class='footer-link'>Centro de Ayuda</a>
                        <a href='#' class='footer-link'>Términos de Servicio</a>
                        <a href='#' class='footer-link'>Política de Privacidad</a>
                        <a href='#' class='footer-link'>Contacto</a>
                    </div>

                    <div class='footer-bottom'>
                        <p>&copy; {DateTime.Now.Year} Sistema Logístico. Todos los derechos reservados.</p>
                        <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
                    </div>
                </div>
            </div>
        </body>
        </html>";
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