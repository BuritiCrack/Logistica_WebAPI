using System.ComponentModel;

namespace LogisticoWebAPI.Shared.Enums
{
    public enum GenderEnum
    {
        [Description("Masculino")]
        Male,

        [Description("Femenino")]
        Female,

        [Description("Otro")]
        Other
    }
}