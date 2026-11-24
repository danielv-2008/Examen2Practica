using Contrato.DTOs;
using Examen2Practica.DTOs;
using Examen2Practica.Interfaces;
using Examen2Practica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen2Practica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // TODO requiere JWT
    public class TesisController : ControllerBase
    {
        private static readonly string[] TiposImagenPermitidos = { "image/jpeg", "image/png", "image/webp", "image/gif" };
        private const int MaxTamanoImagen = 5 * 1024 * 1024; // 5 MB

        private readonly ITesis _tesisServicio;
        private readonly IWebHostEnvironment _entorno;

        public TesisController(ITesis tesisServicio, IWebHostEnvironment entorno)
        {
            _tesisServicio = tesisServicio;
            _entorno = entorno;
        }

        // GET api/tesis -> Listar tesis (con su tutor)
        [HttpGet(Name = "ListarTesis")]
        public IActionResult Get()
        {
            var tesis = _tesisServicio.ObtenerTodos()
                .Select(ADto)
                .ToList();
            return Ok(tesis);
        }

        // GET api/tesis/{id} -> Obtener tesis por id (con su tutor)
        [HttpGet("{id:guid}", Name = "ObtenerTesisPorId")]
        public IActionResult Get(Guid id)
        {
            var tesis = _tesisServicio.ObtenerPorId(id);
            if (tesis == null)
            {
                return NotFound($"No se encontró la tesis con el id: {id}");
            }
            return Ok(ADto(tesis));
        }

        // GET api/tesis/buscar?texto=... -> BUSCAR TESIS Y MOSTRAR TUTOR
        [HttpGet("buscar", Name = "BuscarTesis")]
        public IActionResult Buscar([FromQuery] string? texto)
        {
            var resultados = _tesisServicio.Buscar(texto)
                .Select(t => new TesisConTutorDto
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Estudiante = t.Estudiante,
                    Estado = t.Estado,
                    Imagen = UrlImagen(t.Imagen),
                    Tutor = t.Tutor
                })
                .ToList();
            return Ok(resultados);
        }

        // POST api/tesis -> Crear tesis (REGLA: no puede duplicarse, tutor máx 3)
        [Authorize]   // requiere JWT: crear tesis
        [HttpPost(Name = "GuardarTesis")]
        public IActionResult Post([FromBody] TesisDto tesisDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // REGLA: no puede duplicarse la tesis
            if (_tesisServicio.ExisteTitulo(tesisDto.Titulo))
            {
                return Conflict(new { mensaje = "Ya existe una tesis con ese título" });
            }

            try
            {
                var nuevaTesis = _tesisServicio.Crear(tesisDto);
                return CreatedAtAction(nameof(Get), new { id = nuevaTesis.Id }, ADto(nuevaTesis));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al registrar la tesis: {ex.Message}");
            }
        }

        // POST api/tesis/{id}/imagen -> SUBIR IMAGENES
        [Authorize]   // requiere JWT: subir imagen
        [HttpPost("{id:guid}/imagen", Name = "SubirImagenTesis")]
        public async Task<IActionResult> SubirImagen(Guid id, [FromForm] TesisImagenDto imagenDto)
        {
            var tesis = _tesisServicio.ObtenerPorId(id);
            if (tesis == null)
            {
                return NotFound($"No se encontró la tesis con el id: {id}");
            }

            if (imagenDto.Imagen is null || imagenDto.Imagen.Length == 0)
            {
                return BadRequest("Debes subir una imagen");
            }

            if (!TiposImagenPermitidos.Contains(imagenDto.Imagen.ContentType))
            {
                return BadRequest("Formato de imagen no válido. Usa JPG, PNG, WEBP o GIF");
            }

            if (imagenDto.Imagen.Length > MaxTamanoImagen)
            {
                return BadRequest("La imagen no puede superar los 5 MB");
            }

            try
            {
                var rutaImagen = await GuardarImagenAsync(imagenDto.Imagen);
                _tesisServicio.ActualizarImagen(id, rutaImagen);

                var tesisActualizada = _tesisServicio.ObtenerPorId(id);
                return Ok(ADto(tesisActualizada!));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al subir la imagen: {ex.Message}");
            }
        }

        private TesisConTutorDto ADto(Tesis tesis) => new()
        {
            Id = tesis.Id,
            Titulo = tesis.Titulo,
            Estudiante = tesis.Estudiante,
            Estado = tesis.Estado,
            Imagen = UrlImagen(tesis.Imagen),
            Tutor = tesis.Tutor is null ? null : new TutorDto
            {
                Id = tesis.Tutor.Id,
                Nombre = tesis.Tutor.Nombre,
                Especialidad = tesis.Tutor.Especialidad,
                Correo = tesis.Tutor.Correo
            }
        };

        // Devuelve la URL completa (http://localhost:7285/imagenes/archivo.jpg)
        private string UrlImagen(string? rutaRelativa)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa))
            {
                // Sin imagen: devolvemos vacío para que el frontend muestre su ícono
                return string.Empty;
            }

            if (rutaRelativa.StartsWith("http"))
            {
                return rutaRelativa;
            }

            return $"{Request.Scheme}://{Request.Host}{rutaRelativa}";
        }

        private async Task<string> GuardarImagenAsync(IFormFile archivo)
        {
            var carpeta = Path.Combine(_entorno.WebRootPath, "imagenes");
            Directory.CreateDirectory(carpeta);

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
            {
                extension = ".jpg";
            }

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await archivo.CopyToAsync(stream);

            return $"/imagenes/{nombreArchivo}";
        }
    }
}
