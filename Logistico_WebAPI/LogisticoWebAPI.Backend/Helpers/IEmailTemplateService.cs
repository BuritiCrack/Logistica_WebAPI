using LogisticoWebAPI.Shared.Entities;

namespace LogisticoWebAPI.Backend.Helpers
{
    public interface IEmailTemplateService
    {
        string GeneratePasswordRecoveryTemplete(User user, string tokenLink);
        string GenerateEmailConfirmationTemplate(User user, string tokenLink);
    }
}
