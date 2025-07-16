using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Helpers
{
    public interface IMailHelper
    {
        ActionResponse<string> SendEmail(string toName, string toEmail, string subject, string body);
    }
}
