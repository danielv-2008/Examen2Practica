using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Examen2Practica.DTOs
{
    // DTO para subir la imagen de una tesis (multipart/form-data)
    public class TesisImagenDto
    {
        [Required(ErrorMessage = "La imagen es requerida")]
        public IFormFile? Imagen { get; set; }
    }
}
