using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Helpers
{
    public interface IMailHelper
    {
        ActionResponses<string> SendEmail(string toName, string toEmail, string subject, string body);
    }
}
