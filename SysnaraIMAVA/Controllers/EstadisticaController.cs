using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SysnaraIMAVA.Models;

namespace SysnaraIMAVA.Controllers
{
    public class EstadisticaController : Controller
    {
        private readonly DbimavaContext _context;

        public EstadisticaController(DbimavaContext context)
        {
            _context = context;
        }

        // GET: EstadisticaController
        public async Task<IActionResult> Index()
        {
            // Obtener la lista de años para el filtro
            var años = await _context.Años.ToListAsync();
            ViewBag.Años = años;

            return View();
        }

        // Método para obtener datos estadísticos filtrados por año
        [HttpGet]
        public async Task<JsonResult> ObtenerDatosEstadisticos(int? filtroAño)
        {
            var query = _context.Matriculas.AsQueryable();

            if (filtroAño.HasValue)
            {
                query = query.Where(m => m.Idaño == filtroAño.Value);
            }

            var totalMatriculados = await query.CountAsync();
            var alumnosFemeninos = await query.CountAsync(m => m.Genero == "FEMENINO");
            var alumnosMasculinos = await query.CountAsync(m => m.Genero == "MASCULINO");

            var estadisticasPorGrado = await query
                .GroupBy(m => new { m.Idgrado, m.Grado, m.Seccion })
                .Select(g => new
                {
                    Idgrado = g.Key.Idgrado,
                    Grado = g.Key.Grado,
                    Seccion = g.Key.Seccion,
                    Total = g.Count(),
                    Femenino = g.Count(m => m.Genero == "FEMENINO"),
                    Masculino = g.Count(m => m.Genero == "MASCULINO")
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                totalMatriculados,
                alumnosFemeninos,
                alumnosMasculinos,
                estadisticasPorGrado
            });
        }
    }
}