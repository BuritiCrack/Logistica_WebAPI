using System.ComponentModel;

namespace LogisticoWebAPI.Shared.Enums
{
    public enum UserType
    {
        [Description("Administrador Principal")]
        SuperAdmin,

        [Description("Administrador")]
        Admin,

        [Description("Coordinador")]
        Coordinator,

        [Description("Usuario")]
        User
    }
}