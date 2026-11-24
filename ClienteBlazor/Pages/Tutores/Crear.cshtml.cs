using ClienteBlazor.Services;
using Contrato.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http.Json;

namespace ClienteBlazor.Pages.Tutores
{
    public class CrearModel : PageModel
    {
        private readonly HttpClient _http;

        public CrearModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        [BindProperty]
        public TutorDto Tutor { get; set; } = new();

        public string? MensajeError { get; private set; }

        public IActionResult OnGet()
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                MensajeError = "Completá todos los campos correctamente";
                return Page();
            }

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/tutor", Tutor);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = $"Tutor '{Tutor.Nombre}' creado correctamente 🎉";
                    return RedirectToPage("/Tutores/Index");
                }

                var error = await respuesta.Content.ReadAsStringAsync();
                MensajeError = respuesta.StatusCode switch
                {
                    HttpStatusCode.Conflict => LimpiarMensaje(error),
                    _ => $"No se pudo crear el tutor: {LimpiarMensaje(error)}"
                };
                return Page();
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar a la API";
                return Page();
            }
        }

        private static string LimpiarMensaje(string json)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("mensaje", out var m))
                {
                    return m.GetString() ?? json;
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // no era JSON
            }
            return json;
        }
    }
}
