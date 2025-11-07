using LogisticoWebAPI.Shared.Entities;

namespace LogisticoWebAPI.Backend.Helpers
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GenerateEmailConfirmationTemplate(User user, string tokenLink)
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

        public string GeneratePasswordRecoveryTemplete(User user, string tokenLink)
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
    }
}
