using Examen2Practica.Models;
using Contrato.DTOs;

namespace Examen2Practica.Interfaces
{
    public interface ITesis
    {
        IEnumerable<Tesis> ObtenerTodos();
        Tesis? ObtenerPorId(Guid id);
        bool ExisteTitulo(string titulo);
        int ContarTesisDelTutor(Guid idTutor);
        Tesis Crear(TesisDto tesisDto);
        void ActualizarImagen(Guid id, string rutaImagen);
        // Buscar tesis (por título, estudiante o estado) devolviendo el tutor incluido
        IEnumerable<TesisConTutorDto> Buscar(string? texto);
    }
}
