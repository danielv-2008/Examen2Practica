using System.ComponentModel.DataAnnotations;

namespace Examen2Practica.Models
{
    public class Tutor
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es requerida")]
        [MaxLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        // Relación: un tutor tiene muchas tesis (máximo 3 por regla de negocio)
        public virtual ICollection<Tesis> Tesis { get; set; } = new List<Tesis>();
    }
}
