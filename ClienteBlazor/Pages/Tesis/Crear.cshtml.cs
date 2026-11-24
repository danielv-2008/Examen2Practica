using ClienteBlazor.Services;
using Contrato.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http.Json;

namespace ClienteBlazor.Pages.Tesis
{
    public class CrearModel : PageModel
    {
        private readonly HttpClient _http;

        public CrearModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        [BindProperty]
        public TesisDto Tesis { get; set; } = new();

        public List<TutorDto> Tutores { get; private set; } = new();
        public string? MensajeError { get; private set; }

        public static readonly string[] Estados =
        {
            "Pendiente", "En curso", "Aprobada", "Rechazada", "Terminada"
        };

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Login");
            }

            await CargarTutoresAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Login");
            }

            await CargarTutoresAsync();

            if (Tutores.Count == 0)
            {
                MensajeError = "Primero creá un tutor: sin tutor no se puede crear una tesis";
                return Page();
            }

            if (!ModelState.IsValid || Tesis.IdTutor is null)
            {
                MensajeError = "Completá todos los campos correctamente";
                return Page();
            }

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/tesis", Tesis);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = $"Tesis '{Tesis.Titulo}' creada correctamente 🎉";
                    return RedirectToPage("/Tesis/Index");
                }

                // La API responde 409 (duplicada / tutor con 3 tesis) o 400
                var error = await respuesta.Content.ReadAsStringAsync();
                MensajeError = respuesta.StatusCode switch
                {
                    HttpStatusCode.Conflict => LimpiarMensaje(error),
                    _ => $"No se pudo crear la tesis: {LimpiarMensaje(error)}"
                };
                return Page();
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar a la API";
                return Page();
            }
        }

        private async Task CargarTutoresAsync()
        {
            try
            {
                Tutores = await _http.GetFromJsonAsync<List<TutorDto>>("api/tutor") ?? new();
            }
            catch (HttpRequestException)
            {
                Tutores = new();
            }
        }

        // Los errores vienen tipo {"mensaje":"..."}: nos quedamos con lo lindo
        private static string LimpiarMensaje(string json)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("mensaje", out var m))
                {
                    return m.GetString() ?? json;
                }
                if (doc.RootElement.TryGetProperty("title", out var t))
                {
                    return t.GetString() ?? json;
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // no era JSON, devolvemos tal cual
            }
            return json;
        }
    }
}
