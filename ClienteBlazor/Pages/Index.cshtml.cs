using Contrato.DTOs;
using ClienteBlazor.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace ClienteBlazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        public int TotalTesis { get; private set; }
        public int TotalTutores { get; private set; }
        public bool HayDatos { get; private set; }
        public bool Logueado { get; private set; }

        public async Task OnGetAsync()
        {
            Logueado = HttpContext.Session.EstaLogueado();
            Mensaje = TempData["Mensaje"] as string;

            // Las estadísticas son públicas: si no hay sesión, mostramos la portada igual
            if (!Logueado)
            {
                return;
            }

            try
            {
                var tesis = await _http.GetFromJsonAsync<List<TesisConTutorDto>>("api/tesis");
                var tutores = await _http.GetFromJsonAsync<List<TutorDto>>("api/tutor");

                TotalTesis = tesis?.Count ?? 0;
                TotalTutores = tutores?.Count ?? 0;
                HayDatos = TotalTesis > 0 || TotalTutores > 0;
            }
            catch (HttpRequestException)
            {
                // Sin API no hay stats, la portada se muestra igual
            }
        }

        public string? Mensaje { get; private set; }
    }
}
