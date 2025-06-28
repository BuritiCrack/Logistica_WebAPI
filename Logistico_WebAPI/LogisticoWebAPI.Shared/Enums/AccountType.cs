using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticoWebAPI.Shared.Enums
{
    public enum AccountType
    {
        [Display(Name = "Ahorros")]
        Savings,
        [Display(Name = "Corriente")]
        Checking,
    }
}
