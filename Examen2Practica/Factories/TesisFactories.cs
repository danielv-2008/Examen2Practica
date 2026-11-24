using Examen2Practica.Data;
using Examen2Practica.Models;

namespace Examen2Practica.Factories
{
    public class TesisFactories
    {
        public static void Seed(BaseDatos context)
        {
            //No sembrar si ya hay tesis (evita duplicados en cada arranque)
            if (context.tesis.Any())
            {
                return;
            }

            var tutores = context.tutores.OrderBy(t => t.Nombre).ToList();
            if (tutores.Count == 0)
            {
                return;
            }

            // 10 tesis con nombres reales, sin imagen (la tarjeta muestra el ícono)
            // Respetando la regla: un tutor solo puede tener 3 tesis
            var datos = new (string Titulo, string Estudiante, string Estado)[]
            {
                ("Sistema de recomendación de tesis con inteligencia artificial", "Lucía Fernández", "Aprobada"),
                ("Aplicación móvil para el seguimiento académico de estudiantes", "Martín Gómez", "En curso"),
                ("Plataforma de gestión de bibliotecas con arquitectura microservicios", "Sofía Ramírez", "Pendiente"),
                ("Reconocimiento de gestos para personas con discapacidad motora", "Diego Herrera", "En curso"),
                ("Sistema de alerta temprana para desastres naturales", "Camila Torres", "Aprobada"),
                ("Chatbot de atención al estudiante con procesamiento de lenguaje natural", "Juan Pablo Ríos", "Pendiente"),
                ("Dashboard de analítica para la gestión universitaria", "Valentina Cruz", "Rechazada"),
                ("Optimización de rutas de transporte público con algoritmos genéticos", "Andrés Molina", "En curso"),
                ("Aplicación de realidad aumentada para laboratorios virtuales", "Fernanda Díaz", "Pendiente"),
                ("Sistema de votación electrónica seguro y verificable", "Rodrigo Peña", "Aprobada")
            };

            var tesis = new List<Tesis>();
            var indiceTutor = 0;

            foreach (var (titulo, estudiante, estado) in datos)
            {
                var tutor = tutores[indiceTutor];

                tesis.Add(new Tesis
                {
                    Id = Guid.NewGuid(),
                    Titulo = titulo,
                    Estudiante = estudiante,
                    Estado = estado,
                    Imagen = "",
                    IdTutor = tutor.Id
                });

                // Cada tutor recibe hasta 3 tesis, después pasa al siguiente
                var tesisDeEsteTutor = tesis.Count(t => t.IdTutor == tutor.Id);
                if (tesisDeEsteTutor >= 3)
                {
                    indiceTutor = (indiceTutor + 1) % tutores.Count;
                }
            }

            context.tesis.AddRange(tesis);
            context.SaveChanges();
        }
    }
}
