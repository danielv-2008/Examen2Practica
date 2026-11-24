using Examen2Practica.Data;
using Examen2Practica.Models;
using Examen2Practica.Services;

namespace Examen2Practica.Factories
{
    public class UsuariosFactories
    {
        public static void Seed(BaseDatos context)
        {
            //No sembrar si ya hay usuarios (evita duplicados en cada arranque)
            if (context.usuarios.Any())
            {
                return;
            }

            context.usuarios.Add(new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = "admin",
                PasswordHash = PasswordHasher.Hashear("admin123"),
                Rol = "Admin"
            });

            context.SaveChanges();
        }
    }
}
