using Examen2Practica.Models;
using Contrato.DTOs;

namespace Examen2Practica.Interfaces
{
    public interface ITutor
    {
        IEnumerable<Tutor> ObtenerTodos();
        Tutor? ObtenerPorId(Guid id);
        bool ExisteNombre(string nombre);
        bool ExisteCorreo(string correo);
        Tutor Crear(TutorDto tutorDto);
    }
}
