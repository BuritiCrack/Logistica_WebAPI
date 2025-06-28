using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
