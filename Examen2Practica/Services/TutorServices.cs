using Examen2Practica.Data;
using Examen2Practica.Models;
using Contrato.DTOs;
using Examen2Practica.Interfaces;

namespace Examen2Practica.Services
{
    public class TutorServices : ITutor
    {
        private readonly BaseDatos _context;

        public TutorServices(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Tutor> ObtenerTodos()
        {
            return _context.tutores.ToList();
        }

        public Tutor? ObtenerPorId(Guid id)
        {
            return _context.tutores.Find(id);
        }

        public bool ExisteNombre(string nombre)
        {
            var n = nombre.Trim().ToLower();
            return _context.tutores.Any(t => t.Nombre.ToLower() == n);
        }

        public bool ExisteCorreo(string correo)
        {
            var c = correo.Trim().ToLower();
            return _context.tutores.Any(t => t.Correo.ToLower() == c);
        }

        public Tutor Crear(TutorDto tutorDto)
        {
            // REGLA: no puede duplicarse el tutor (ni por nombre ni por correo)
            if (ExisteNombre(tutorDto.Nombre))
            {
                throw new InvalidOperationException("Ya existe un tutor con ese nombre");
            }

            if (ExisteCorreo(tutorDto.Correo))
            {
                throw new InvalidOperationException("Ya existe un tutor con ese correo");
            }

            var tutor = new Tutor
            {
                Id = Guid.NewGuid(),
                Nombre = tutorDto.Nombre.Trim(),
                Especialidad = tutorDto.Especialidad.Trim(),
                Correo = tutorDto.Correo.Trim().ToLower()
            };

            _context.tutores.Add(tutor);
            _context.SaveChanges();

            return tutor;
        }
    }
}
