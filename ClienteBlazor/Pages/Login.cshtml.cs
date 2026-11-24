using ClienteBlazor.Services;
using Contrato.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClienteBlazor.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _http;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        [BindProperty]
        public string NombreUsuario { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? MensajeError { get; private set; }
        public string? MensajeExito { get; private set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.EstaLogueado())
            {
                return RedirectToPage("/Tesis/Index");
            }

            MensajeExito = TempData["Mensaje"] as string;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(Password))
            {
                MensajeError = "Completá el usuario y la contraseña";
                return Page();
            }

            try
            {
                var dto = new LoginDto
                {
                    NombreUsuario = NombreUsuario.Trim(),
                    Password = Password
                };

                var respuesta = await _http.PostAsJsonAsync("api/auth/login", dto);

                if (!respuesta.IsSuccessStatusCode)
                {
                    MensajeError = "Usuario o contraseña incorrectos";
                    return Page();
                }

                var login = await respuesta.Content.ReadFromJsonAsync<LoginRespuestaDTO>();

                if (login is null || string.IsNullOrWhiteSpace(login.Token))
                {
                    MensajeError = "No se pudo iniciar sesión";
                    return Page();
                }

                // Guardamos el token en la sesión: a partir de ahí el
                // TokenHandler lo manda en cada llamada a la API
                HttpContext.Session.GuardarSesion(login.Token, login.NombreUsuario, login.Rol);

                return RedirectToPage("/Tesis/Index");
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar a la API";
                return Page();
            }
        }
    }
}
