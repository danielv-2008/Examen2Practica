using ClienteBlazor.Services;
using Contrato.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http.Json;

namespace ClienteBlazor.Pages.Tutores
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        public List<TutorDto> Tutores { get; private set; } = new();
        // Cuántas tesis tiene cada tutor (para mostrar el cupo de 3)
        public Dictionary<Guid, int> TesisPorTutor { get; private set; } = new();
        public string? MensajeExito { get; private set; }
        public string? MensajeError { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Login");
            }

            MensajeExito = TempData["Mensaje"] as string;

            try
            {
                Tutores = await _http.GetFromJsonAsync<List<TutorDto>>("api/tutor") ?? new();

                var tesis = await _http.GetFromJsonAsync<List<TesisConTutorDto>>("api/tesis") ?? new();
                TesisPorTutor = tesis
                    .Where(t => t.Tutor is not null)
                    .GroupBy(t => t.Tutor!.Id)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar a la API";
            }

            return Page();
        }

        // POST: crear tutor rápido desde la lista
        public async Task<IActionResult> OnPostAsync(TutorDto tutor)
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrWhiteSpace(tutor.Nombre))
            {
                TempData["Mensaje"] = "El nombre del tutor es requerido";
                return RedirectToPage();
            }

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/tutor", tutor);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = $"Tutor '{tutor.Nombre}' creado correctamente 🎉";
                }
                else
                {
                    var error = await respuesta.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"No se pudo crear el tutor: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Mensaje"] = "No se pudo conectar a la API";
            }

            return RedirectToPage();
        }
    }
}
