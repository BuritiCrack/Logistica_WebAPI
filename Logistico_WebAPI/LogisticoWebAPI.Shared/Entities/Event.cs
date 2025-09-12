using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticoWebAPI.Shared.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Display(Name = "Nombre del Evento")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(300, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Display(Name = "Descripción del Evento")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Display(Name = "Lugar del Evento")]
        public string Place { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:dddd/MMM/yyyy h:mm tt}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Fecha y Hora de Inicio")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dddd/MMM/yyyy h:mm tt}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Fecha y Hora de Fin")]
        public DateTime EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dddd/MMM/yyyy}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Fecha y de pago")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Display(Name = "Tipo de Comida")]
        public string? MealType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Pago por Persona")]
        [Range(0, double.MaxValue, ErrorMessage = "El campo {0} debe ser un número positivo.")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Payment { get; set; }

        [Display(Name = "Foto del Evento")]
        public string? Photo { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<EventUser>? EventUsers { get; set; }

        [Display(Name = "Número de Participantes")]
        public int NumberOfParticipants => EventUsers?.Count ?? 0;

    }
}