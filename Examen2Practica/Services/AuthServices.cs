using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Examen2Practica.Data;
using Examen2Practica.Interfaces;
using Examen2Practica.Models;
using Contrato.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace Examen2Practica.Services
{
    public class AuthServices : IAuthInterface
    {
        private readonly BaseDatos _context;
        private readonly IConfiguration _config;

        public AuthServices(BaseDatos context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public void Registrar(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
            {
                throw new ArgumentException("El nombre de usuario es requerido");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("La contraseña es requerida");
            }

            var nombre = dto.NombreUsuario.Trim();

            if (_context.usuarios.Any(u => u.NombreUsuario == nombre))
            {
                throw new InvalidOperationException("Ese nombre de usuario ya existe");
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = nombre,
                PasswordHash = PasswordHasher.Hashear(dto.Password),
                Rol = "Usuario"
            };

            _context.usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public LoginRespuestaDTO? Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return null;
            }

            var nombre = dto.NombreUsuario.Trim();
            var usuario = _context.usuarios.FirstOrDefault(u => u.NombreUsuario == nombre);

            if (usuario is null || !PasswordHasher.Verificar(dto.Password, usuario.PasswordHash))
            {
                return null;
            }

            var minutos = int.TryParse(_config["Jwt:ExpiraMinutos"], out var m) ? m : 60;
            var expira = DateTime.UtcNow.AddMinutes(minutos);

            return new LoginRespuestaDTO
            {
                Token = GenerarToken(usuario, expira),
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                Expira = expira
            };
        }

        private string GenerarToken(Usuario usuario, DateTime expira)
        {
            var secreta = _config["Jwt:Secret"]
                ?? throw new InvalidOperationException("Falta Jwt:Secret en appsettings.json");

            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreta));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, usuario.NombreUsuario),
                new(ClaimTypes.Role, usuario.Rol),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
