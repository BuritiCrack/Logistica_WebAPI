using LogisticoWebAPI.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Document { get; set; } = null!;

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electrónico válido")]
        [MaxLength(256, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        public override string? Email { get; set; }

        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Género")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public GenderEnum Gender { get; set; }

        [Display(Name = "Altura (cm)")]
        [Range(120, 250, ErrorMessage = "El campo {0} debe estar entre {1} y {2}cm")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Height { get; set; } = null!;

        [Display(Name = "Años")]
        [Range(18, 70, ErrorMessage = "El campo {0} debe ser mayor o igual a {1}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Age { get; set; } = null!;

        [Display(Name = "Experiencia")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Experience { get; set; } = null!;

        [Display(Name = "Dirección")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Address { get; set; } = null!;

        [Display(Name = "Habilidades")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Skills { get; set; } = null!;

        [Display(Name = "Banco")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public BankName Bank { get; set; }

        [Display(Name = "Tipo de cuenta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public AccountType AccountType { get; set; }

        [Display(Name = "Número de cuenta")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string AccountNumber { get; set; } = null!;

        [Display(Name = "EPS")]
        [MaxLength(30, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Eps { get; set; } = null!;

        [Display(Name = "Fondo de pensiones")]
        [MaxLength(30, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string PensionFund { get; set; } = null!;

        [Display(Name = "Estado del usuario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool IsActive { get; set; } = false;

        [Display(Name = "Talla de Camiseta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public TShirtSize TShirtSize { get; set; }

        [Display(Name = "Foto")]
        public string? Photo { get; set; }

        [Display(Name = "Código QR")]
        public string? QrCode { get; set; }

        [Display(Name = "Tipo de usuario")]
        public UserType UserType { get; set; }

        public City? City { get; set; }

        [Display(Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una {0}.")]
        public int CityId { get; set; }

        [Display(Name = "Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<EventUser>? EventUsers { get; set; }
    }
}