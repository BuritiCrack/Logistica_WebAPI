using LogisticoWebAPI.Shared.Responses;
using MailKit.Net.Smtp;
using MimeKit;

namespace LogisticoWebAPI.Backend.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ActionResponse<string> SendEmail(string toName, string toEmail, string subject, string body)
        {
            try 
            {
                var from = _configuration["Mail:From"];
                var name = _configuration["Mail:Name"];
                var smtp = _configuration["Mail:Smtp"];
                var port = _configuration["Mail:Port"];
                var password = _configuration["Mail:Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(name, from));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port!),false /*MailKit.Security.SecureSocketOptions.StartTls*/); //el codigo comentado es para usar TLS, pero puede que no sea necesario dependiendo del servidor SMTP
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new ActionResponse<string> { WasSuccess = true };
           
            }
            catch (Exception ex)
            {
                return new ActionResponse<string>
                {
                    WasSuccess = false,
                    Message = $"Error al enviar el correo: {ex.Message}"
                };
            }
        }
        
    }
}
