namespace Contrato.DTOs
{
    public class LoginRespuestaDTO
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
    }
}
