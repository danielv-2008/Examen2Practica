namespace ClienteBlazor.Services
{
    // Claves donde se guarda la sesión del usuario
    public static class SesionKeys
    {
        public const string Token = "JwtToken";
        public const string Usuario = "Usuario";
        public const string Rol = "Rol";
    }

    // Helpers para manejar la sesión (token JWT) desde cualquier página
    public static class SesionExtensions
    {
        public static bool EstaLogueado(this ISession sesion)
            => !string.IsNullOrWhiteSpace(sesion.GetString(SesionKeys.Token));

        public static void GuardarSesion(this ISession sesion, string token, string usuario, string rol)
        {
            sesion.SetString(SesionKeys.Token, token);
            sesion.SetString(SesionKeys.Usuario, usuario);
            sesion.SetString(SesionKeys.Rol, rol);
        }

        public static void CerrarSesion(this ISession sesion)
        {
            sesion.Remove(SesionKeys.Token);
            sesion.Remove(SesionKeys.Usuario);
            sesion.Remove(SesionKeys.Rol);
        }

        public static string? ObtenerUsuario(this ISession sesion)
            => sesion.GetString(SesionKeys.Usuario);

        public static string? ObtenerRol(this ISession sesion)
            => sesion.GetString(SesionKeys.Rol);
    }
}
