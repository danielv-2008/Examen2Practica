using ClienteBlazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClienteBlazor.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.CerrarSesion();
            TempData["Mensaje"] = "Cerraste la sesión correctamente";
            return RedirectToPage("/Index");
        }

        public IActionResult OnGet()
        {
            HttpContext.Session.CerrarSesion();
            TempData["Mensaje"] = "Cerraste la sesión correctamente";
            return RedirectToPage("/Index");
        }
    }
}
