using Contrato.DTOs;
using Examen2Practica.Interfaces;
using Examen2Practica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen2Practica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // TODO requiere JWT
    public class TutorController : ControllerBase
    {
        private readonly ITutor _tutorServicio;

        public TutorController(ITutor tutorServicio)
        {
            _tutorServicio = tutorServicio;
        }

        // GET api/tutor -> Listar tutores
        [HttpGet(Name = "ListarTutores")]
        public IActionResult Get()
        {
            var tutores = _tutorServicio.ObtenerTodos()
                .Select(ADto)
                .ToList();
            return Ok(tutores);
        }

        // GET api/tutor/{id} -> Obtener tutor por id
        [HttpGet("{id:guid}", Name = "ObtenerTutorPorId")]
        public IActionResult Get(Guid id)
        {
            var tutor = _tutorServicio.ObtenerPorId(id);
            if (tutor == null)
            {
                return NotFound($"No se encontró el tutor con el id: {id}");
            }
            return Ok(ADto(tutor));
        }

        // POST api/tutor -> Crear tutor (REGLA: no puede duplicarse)
        [Authorize]   // requiere JWT: crear tutor
        [HttpPost(Name = "GuardarTutor")]
        public IActionResult Post([FromBody] TutorDto tutorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // REGLA: no puede duplicarse el tutor
            if (_tutorServicio.ExisteNombre(tutorDto.Nombre))
            {
                return Conflict(new { mensaje = "Ya existe un tutor con ese nombre" });
            }

            if (_tutorServicio.ExisteCorreo(tutorDto.Correo))
            {
                return Conflict(new { mensaje = "Ya existe un tutor con ese correo" });
            }

            try
            {
                var nuevoTutor = _tutorServicio.Crear(tutorDto);
                return CreatedAtAction(nameof(Get), new { id = nuevoTutor.Id }, ADto(nuevoTutor));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al registrar el tutor: {ex.Message}");
            }
        }

        private TutorDto ADto(Tutor tutor) => new()
        {
            Id = tutor.Id,
            Nombre = tutor.Nombre,
            Especialidad = tutor.Especialidad,
            Correo = tutor.Correo
        };
    }
}
