using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SysnaraIMAVA.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace SysnaraIMAVA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbimavaContext _context;

        public HomeController(ILogger<HomeController> logger, DbimavaContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Acción para mostrar la vista de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult IndexTeacher()
        {
            // Lógica para la vista de 'Twelfth'
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Verificar las credenciales
            if (username == "imava" && password == "09021994")
            {
                // Asignar rol de Administrador
                HttpContext.Session.SetString("Role", "Administrador");
                return RedirectToAction("Index");
            }
            else if (username == "teacher" && password == "maestro1234")
            {
                // Asignar rol de Teacher
                HttpContext.Session.SetString("Role", "Teacher");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "kindergarten" && password == "Kinder789")
            {
                // Asignar rol de Kinder
                HttpContext.Session.SetString("Role", "Kindergarten");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "preparatory" && password == "Prep101")
            {
                // Asignar rol de Preparatoria
                HttpContext.Session.SetString("Role", "Preparatory");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "first" && password == "First112")
            {
                // Asignar rol de Primero
                HttpContext.Session.SetString("Role", "First");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "second" && password == "Second131")
            {
                // Asignar rol de Segundo
                HttpContext.Session.SetString("Role", "Second");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "third" && password == "Third415")
            {
                // Asignar rol de Tercero
                HttpContext.Session.SetString("Role", "Third");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "fourth" && password == "Fourth161")
            {
                // Asignar rol de Cuarto
                HttpContext.Session.SetString("Role", "Fourth");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "fifth" && password == "Fifth718")
            {
                // Asignar rol de Quinto
                HttpContext.Session.SetString("Role", "Fifth");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "sixth" && password == "Sixth192")
            {
                // Asignar rol de Sexto
                HttpContext.Session.SetString("Role", "Sixth");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "seventh" && password == "Seventh213")
            {
                // Asignar rol de Séptimo
                HttpContext.Session.SetString("Role", "Seventh");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "eighth" && password == "Eighth141")
            {
                // Asignar rol de Octavo
                HttpContext.Session.SetString("Role", "Eighth");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "ninth" && password == "Ninth516")
            {
                // Asignar rol de Noveno
                HttpContext.Session.SetString("Role", "Ninth");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "tenth" && password == "Tenth171")
            {
                // Asignar rol de Décimo
                HttpContext.Session.SetString("Role", "Tenth");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "eleventh" && password == "Eleventh819")
            {
                // Asignar rol de Undécimo
                HttpContext.Session.SetString("Role", "Eleventh");
                return RedirectToAction("IndexTeacher");
            }
            else if (username == "twelfth" && password == "Twelfth021")
            {
                // Asignar rol de Duodécimo
                HttpContext.Session.SetString("Role", "Twelfth");
                return RedirectToAction("IndexTeacher");
            }
            else
            {
                // Si las credenciales son incorrectas, mostrar un mensaje de error
                ViewData["ErrorMessage"] = "Usuario o contraseña incorrectos";
                return View();
            }
        }

        // Acción principal del controlador (Index)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index(int? filtroAño, string? filtroSistema)
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login");
            }

            // Proceder con la lógica normal de la página principal
            var años = await _context.Años.ToListAsync();
            ViewBag.Años = años;
            ViewBag.FiltroAño = filtroAño;
            ViewBag.FiltroSistema = filtroSistema;

            var query = _context.Matriculas.AsQueryable();


            //// Pasar el rol de usuario a la vista para ajustar el navbar
            //ViewBag.UserRole = userRole;

            return View();
        }

        // Acción Logout para cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Role"); // Eliminar el rol de la sesión
            return RedirectToAction("Login"); // Redirigir al login
        }

        //Endpoint
        public IActionResult CheckSession()
        {
            var userRole = HttpContext.Session.GetString("Role");
            return Json(new { isAuthenticated = !string.IsNullOrEmpty(userRole) });
        }

        // Página de privacidad
        public IActionResult Privacy()
        {
            return View();
        }

        // Página de error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
