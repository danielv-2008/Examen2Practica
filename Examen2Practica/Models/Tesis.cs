using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examen2Practica.Models
{
    public class Tesis
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estudiante es requerido")]
        [MaxLength(100)]
        public string Estudiante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido")]
        [MaxLength(50)]
        public string Estado { get; set; } = string.Empty;

        public string Imagen { get; set; } = string.Empty;

        public Guid IdTutor { get; set; }

        [ForeignKey(nameof(IdTutor))]
        public virtual Tutor? Tutor { get; set; }
    }
}
