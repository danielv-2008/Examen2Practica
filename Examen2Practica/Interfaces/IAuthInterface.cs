using Contrato.DTOs;

namespace Examen2Practica.Interfaces
{
    public interface IAuthInterface
    {
        void Registrar(LoginDto dto);
        LoginRespuestaDTO? Login(LoginDto dto);
    }
}
