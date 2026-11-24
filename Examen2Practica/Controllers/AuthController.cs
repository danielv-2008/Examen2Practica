using Examen2Practica.Interfaces;
using Contrato.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen2Practica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _auth;

        public AuthController(IAuthInterface auth)
        {
            _auth = auth;
        }

        // POST api/auth/registrar -> ABIERTO (nadie tiene token todavía)
        [AllowAnonymous]
        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] LoginDto dto)
        {
            try
            {
                _auth.Registrar(dto);
                return Ok(new { mensaje = "Usuario registrado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        // POST api/auth/login -> ABIERTO (para poder conseguir el token)
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var respuesta = _auth.Login(dto);

            if (respuesta is null)
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
            }

            return Ok(respuesta);
        }

        // GET api/auth/yo -> EL PERFIL, requiere el token
        [HttpGet("yo")]
        [Authorize]
        public IActionResult Yo()
        {
            return Ok(new
            {
                usuario = User.Identity?.Name,
                rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
            });
        }
    }
}
