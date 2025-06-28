using System.ComponentModel;

namespace LogisticoWebAPI.Shared.Enums
{
    public enum UserType
    {
        [Description("Administrador")]
        Admin,

        [Description("Usuario")]
        User,
    }
}