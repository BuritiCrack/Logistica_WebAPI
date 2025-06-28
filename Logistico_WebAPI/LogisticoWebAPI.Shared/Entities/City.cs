using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.Entities
{
    public class City
    {
        public int Id { get; set; }

        [MaxLength(40, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        public int StateId { get; set; }
        public State? State { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}