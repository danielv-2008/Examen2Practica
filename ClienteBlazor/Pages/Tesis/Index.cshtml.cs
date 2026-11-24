using ClienteBlazor.Services;
using Contrato.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;

namespace ClienteBlazor.Pages.Tesis
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        public List<TesisConTutorDto> Resultados { get; private set; } = new();
        public string? TextoBusqueda { get; private set; }
        public string? MensajeError { get; private set; }
        public string? MensajeExito { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string? q { get; set; }

        public async Task OnGetAsync()
        {
            MensajeExito = TempData["Mensaje"] as string;

            if (!HttpContext.Session.EstaLogueado())
            {
                return;
            }

            await BuscarAsync(q);
        }

        public async Task OnPostAsync()
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return;
            }

            await BuscarAsync(q);
        }

        private async Task BuscarAsync(string? texto)
        {
            TextoBusqueda = texto;
            MensajeError = null;

            try
            {
                // Con texto -> buscar (devuelve tesis CON su tutor)
                // Sin texto -> listado completo
                var url = string.IsNullOrWhiteSpace(texto)
                    ? "api/tesis"
                    : $"api/tesis/buscar?texto={Uri.EscapeDataString(texto)}";

                var respuesta = await _http.GetAsync(url);

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return;
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    MensajeError = "No se pudo consultar la API";
                    return;
                }

                var json = await respuesta.Content.ReadAsStringAsync();
                Resultados = JsonSerializer.Deserialize<List<TesisConTutorDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar a la API";
            }
        }

        // POST: subir imagen de una tesis (punto 4 del pizarrón)
        public async Task<IActionResult> OnPostSubirImagenAsync(Guid id, IFormFile? imagen)
        {
            if (!HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Login");
            }

            if (imagen is null || imagen.Length == 0)
            {
                TempData["Mensaje"] = "Elegí una imagen antes de subirla";
                return RedirectToPage("/Tesis/Index");
            }

            try
            {
                using var contenido = new MultipartFormDataContent();
                using var stream = imagen.OpenReadStream();
                var archivo = new StreamContent(stream);
                archivo.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(imagen.ContentType);
                contenido.Add(archivo, "Imagen", imagen.FileName);

                var respuesta = await _http.PostAsync($"api/tesis/{id}/imagen", contenido);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = $"Imagen '{imagen.FileName}' subida correctamente 🎉";
                }
                else
                {
                    var error = await respuesta.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"No se pudo subir la imagen: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Mensaje"] = "No se pudo conectar a la API";
            }

            return RedirectToPage("/Tesis/Index", new { q });
        }
    }
}
