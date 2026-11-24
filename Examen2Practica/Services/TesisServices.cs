using Examen2Practica.Data;
using Examen2Practica.Models;
using Contrato.DTOs;
using Examen2Practica.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Examen2Practica.Services
{
    public class TesisServices : ITesis
    {
        // REGLA DE NEGOCIO: un tutor solo puede tener 3 tesis
        public const int MaxTesisPorTutor = 3;

        private readonly BaseDatos _context;

        public TesisServices(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Tesis> ObtenerTodos()
        {
            return _context.tesis.Include(t => t.Tutor).ToList();
        }

        public Tesis? ObtenerPorId(Guid id)
        {
            return _context.tesis.Include(t => t.Tutor).FirstOrDefault(t => t.Id == id);
        }

        public bool ExisteTitulo(string titulo)
        {
            var t = titulo.Trim().ToLower();
            return _context.tesis.Any(x => x.Titulo.ToLower() == t);
        }

        public int ContarTesisDelTutor(Guid idTutor)
        {
            return _context.tesis.Count(t => t.IdTutor == idTutor);
        }

        public Tesis Crear(TesisDto tesisDto)
        {
            // REGLA: no puede duplicarse la tesis (título único)
            if (ExisteTitulo(tesisDto.Titulo))
            {
                throw new InvalidOperationException("Ya existe una tesis con ese título");
            }

            if (tesisDto.IdTutor is null || tesisDto.IdTutor == Guid.Empty)
            {
                throw new ArgumentException("El tutor es requerido");
            }

            var tutor = _context.tutores.Find(tesisDto.IdTutor.Value);
            if (tutor is null)
            {
                throw new ArgumentException("El tutor indicado no existe");
            }

            // REGLA: un tutor solo puede tener 3 tesis
            if (ContarTesisDelTutor(tutor.Id) >= MaxTesisPorTutor)
            {
                throw new InvalidOperationException(
                    $"El tutor '{tutor.Nombre}' ya tiene {MaxTesisPorTutor} tesis y no puede tener más");
            }

            var tesis = new Tesis
            {
                Id = Guid.NewGuid(),
                Titulo = tesisDto.Titulo.Trim(),
                Estudiante = tesisDto.Estudiante.Trim(),
                Estado = tesisDto.Estado.Trim(),
                IdTutor = tutor.Id,
                Imagen = string.Empty
            };

            _context.tesis.Add(tesis);
            _context.SaveChanges();

            return tesis;
        }

        public void ActualizarImagen(Guid id, string rutaImagen)
        {
            var tesis = _context.tesis.Find(id);
            if (tesis is null)
            {
                throw new ArgumentException("La tesis indicada no existe");
            }

            tesis.Imagen = rutaImagen;
            _context.SaveChanges();
        }

        public IEnumerable<TesisConTutorDto> Buscar(string? texto)
        {
            var query = _context.tesis.Include(t => t.Tutor).AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var t = texto.Trim().ToLower();
                query = query.Where(x =>
                    x.Titulo.ToLower().Contains(t) ||
                    x.Estudiante.ToLower().Contains(t) ||
                    x.Estado.ToLower().Contains(t));
            }

            return query
                .ToList()
                .Select(x => new TesisConTutorDto
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Estudiante = x.Estudiante,
                    Estado = x.Estado,
                    Imagen = x.Imagen,
                    Tutor = x.Tutor is null ? null : new TutorDto
                    {
                        Id = x.Tutor.Id,
                        Nombre = x.Tutor.Nombre,
                        Especialidad = x.Tutor.Especialidad,
                        Correo = x.Tutor.Correo
                    }
                })
                .ToList();
        }
    }
}
