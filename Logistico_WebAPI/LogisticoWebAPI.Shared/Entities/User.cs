using LogisticoWebAPI.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime;

namespace LogisticoWebAPI.Shared.Entities
{
    public class User : IdentityUser
    {
        [DisplayName("Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Document { get; set; } = null!;

        [DisplayName("Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string FirstName { get; set; } = null!;

        [DisplayName("Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string LastName { get; set; } = null!;

        [DisplayName("Género")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public GenderEnum Gender { get; set; }

        [DisplayName("Altura (cm)")]
        [Range(120, 250, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Height { get; set; } = null!;

        [DisplayName("Años")]
        [Range(18, 70, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Age { get; set; } = null!;

        [DisplayName("Experiencia")]
        [Range(0, 100, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Experience { get; set; } = null!;

        [DisplayName("Habilidades")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Skills { get; set; } = null!;

        [DisplayName("Banco")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public BankName Bank { get; set; }

        [DisplayName("Tipo de cuenta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public AccountType AccountType { get; set; }

        [DisplayName("Número de cuenta")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string AccountNumber { get; set; } = null!;

        [DisplayName("EPS")]
        [MaxLength(30, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Eps { get; set; } = null!;

        [DisplayName("Fondo de pensiones")]
        [MaxLength(30, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string PensionFund { get; set; } = null!;

        [DisplayName("Estado del usuario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool? IsActive { get; set; } = true;

        [DisplayName("Foto")]
        public string? Photo { get; set; }

        [DisplayName("Tipo de usuario")]
        public UserType UserType { get; set; }
        public City? City { get; set; }

        [DisplayName("Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una {0}.")]
        public int CityId { get; set; }

        [DisplayName("Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<EventUser>? EventUsers { get; set; }

    }
}