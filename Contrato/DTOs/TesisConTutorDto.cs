namespace Contrato.DTOs
{
    // Tesis con su tutor incluido (para buscar tesis y mostrar el tutor)
    public class TesisConTutorDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Estudiante { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public TutorDto? Tutor { get; set; }
    }
}
