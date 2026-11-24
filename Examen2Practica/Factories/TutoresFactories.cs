using Examen2Practica.Data;
using Examen2Practica.Models;

namespace Examen2Practica.Factories
{
    public class TutoresFactories
    {
        public static void Seed(BaseDatos context)
        {
            //No sembrar si ya hay tutores (evita duplicados en cada arranque)
            if (context.tutores.Any())
            {
                return;
            }

            var tutores = new List<Tutor>
            {
                new() { Id = Guid.NewGuid(), Nombre = "Dr. Carlos Mendoza", Especialidad = "Inteligencia Artificial", Correo = "cmendoza@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dra. Laura Sánchez", Especialidad = "Bases de Datos", Correo = "lsanchez@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dr. Roberto Díaz", Especialidad = "Redes y Seguridad", Correo = "rdiaz@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dra. Patricia Gómez", Especialidad = "Desarrollo de Software", Correo = "pgomez@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dr. Fernando Ruiz", Especialidad = "Visión por Computadora", Correo = "fruiz@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dra. Carmen López", Especialidad = "Ciberseguridad", Correo = "clopez@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dr. Miguel Ángel Vega", Especialidad = "Computación en la Nube", Correo = "mvega@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dra. Isabel Romero", Especialidad = "Procesamiento de Lenguaje Natural", Correo = "iromero@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dr. Jorge Castillo", Especialidad = "Sistemas Distribuidos", Correo = "jcastillo@universidad.edu" },
                new() { Id = Guid.NewGuid(), Nombre = "Dra. Elena Morales", Especialidad = "Robótica", Correo = "emorales@universidad.edu" }
            };

            context.tutores.AddRange(tutores);
            context.SaveChanges();
        }
    }
}
