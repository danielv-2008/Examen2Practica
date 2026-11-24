using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Examen2Practica.Swagger
{
    // Muestra el candado y exige token SOLO en los endpoints con [Authorize].
    // Los que no lo tienen quedan públicos en Swagger.
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var enAccion = context.MethodInfo
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Any();

            var enClase = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Any() == true;

            if (!enAccion && !enClase)
            {
                // Endpoint público: sin candado y sin token
                operation.Security = [];
                return;
            }

            // Endpoint protegido: candado + envío del token en el "Try it out"
            operation.Security =
            [
                new()
                {
                    [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
                }
            ];
        }
    }
}
