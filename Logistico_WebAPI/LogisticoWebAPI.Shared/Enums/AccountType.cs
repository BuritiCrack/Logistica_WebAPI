using System.ComponentModel.DataAnnotations;

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