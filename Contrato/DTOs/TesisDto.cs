using System.ComponentModel.DataAnnotations;

namespace Contrato.DTOs
{
    public class TesisDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El título de la tesis es requerido")]
        [MaxLength(200, ErrorMessage = "El título no puede tener más de 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estudiante es requerido")]
        [MaxLength(100, ErrorMessage = "El estudiante no puede tener más de 100 caracteres")]
        public string Estudiante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido")]
        [MaxLength(50, ErrorMessage = "El estado no puede tener más de 50 caracteres")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tutor es requerido")]
        public Guid? IdTutor { get; set; }
    }
}
