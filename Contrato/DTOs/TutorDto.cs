using System.ComponentModel.DataAnnotations;

namespace Contrato.DTOs
{
    public class TutorDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del tutor es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad del tutor es requerida")]
        [MaxLength(100, ErrorMessage = "La especialidad no puede tener más de 100 caracteres")]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo del tutor es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [MaxLength(150, ErrorMessage = "El correo no puede tener más de 150 caracteres")]
        public string Correo { get; set; } = string.Empty;
    }
}
