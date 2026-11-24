using System.Net.Http.Headers;

namespace ClienteBlazor.Services
{
    // Agrega el encabezado "Authorization: Bearer <token>" a TODAS las
    // llamadas que hace el cliente a la API, usando el token de la sesión.
    public class TokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString(SesionKeys.Token);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
