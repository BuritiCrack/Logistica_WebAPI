using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.Enums
{
    public enum ApplicationStatus
    {
        [Display(Name = "Pendiente")]
        Pending,

        [Display(Name = "Aceptado")]
        Accepted,

        [Display(Name = "Rechazado")]
        Rejected,

        [Display(Name = "Cancelado por el usuario")]
        CancelledByUser
    }
}