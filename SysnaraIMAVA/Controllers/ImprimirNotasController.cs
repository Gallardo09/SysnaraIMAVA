using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysnaraIMAVA.Models;
using FastReport.Export.PdfSimple;
using FastReport;
using Microsoft.AspNetCore.Hosting;

namespace SysnaraIMAVA.Controllers
{
    public class ImprimirNotasController : Controller
    {
        private readonly DbimavaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImprimirNotasController(DbimavaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ActionResult> IndexAsync()
        {
            var años = await _context.Años.ToListAsync();
            ViewBag.Años = años;
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetGrados(int idAño)
        {
            var grados = await _context.Grados
                .Where(g => g.Idaño == idAño)
                .Select(g => new { g.Idgrado, GradoCompleto = g.Idgrado + " - " + g.Grado1 })
                .ToListAsync();

            return Json(grados);
        }

        [HttpGet]
        public async Task<JsonResult> GetListaEstudiantesMatricula(int idAño, string idGrado)
        {
            var query = _context.Matriculas
                .Where(m => m.Idaño == idAño);

            if (!string.IsNullOrEmpty(idGrado))
            {
                query = query.Where(m => m.Idgrado == idGrado);
            }

            var matriculas = await query
                .Select(m => new {
                    m.Idaño,
                    m.Idest,
                    m.Idimv,
                    m.Idestudiante,
                    m.NombreEstudiante,
                    m.Genero,
                    m.Grado
                })
                .OrderBy(m => m.Genero)
                .ThenBy(m => m.NombreEstudiante)
                .ToListAsync();

            return Json(matriculas);
        }

        public IActionResult GenerarNotasIParcialPrimaria(int idaño, string ididentidad)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(ididentidad))
            {
                return BadRequest("Se requieren un ID de año y un ID de identidad válidos.");
            }

            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Idestudiante == ididentidad)
                .Select(n => new
                {
                    n.Idaño,
                    n.Idest,
                    n.Idestudiante,
                    n.NombreEstudiante,
                    n.Genero,
                    n.Idasignatura,
                    n.Asignatura,
                    n.Idgrado,
                    n.Grado,
                    n.Seccion,
                    n.Jornada,
                    n.Idnota,
                    n.Iparcial,
                    n.Iiparcial,
                    n.Iiiparcial,
                    n.Ivparcial,
                    n.Isemestre,
                    n.Iisemestre,
                    n.RecuperacionI,
                    n.RecuperacionIi,
                    n.PromedioFinal,
                    n.IndicePromocion
                }).ToList();

            var personalidad = _context.Personalidads
                .Where(p => p.Idaño == idaño && p.Idestudiante == ididentidad)
                .Select(p => new
                {
                    p.Idaño,
                    p.Idest,
                    p.Idestudiante,
                    p.NombreEstudiante,
                    p.Genero,
                    p.Idgrado,
                    p.Grado,
                    p.Seccion,
                    p.Jornada,
                    p.Idpersonalidad,
                    p.Puntualidad,
                    p.OrdenYPresentacion,
                    p.Moralidad,
                    p.EspirituDeTrabajo,
                    p.Sociabilidad,
                    p.Inasistencias
                }).ToList();

            if (!notas.Any())
            {
                return NotFound($"No se encontraron datos para el estudiante con ID de identidad {ididentidad} en el año {idaño}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotas", "FR_ImprimirNotas_Primaria_I_Parcial.frx");

            try
            {
                report.Load(reportPath);
                report.RegisterData(notas, "frImprimirNotasIParcialPrimariaRef");
                report.RegisterData(personalidad, "frImprimirPersonalidadRef");
                report.GetDataSource("NOTA").Enabled = true;
                report.GetDataSource("PERSONALIDAD").Enabled = true;
                report.SetParameterValue("ReportTitle", $"Notas I Parcial");
                report.Prepare();

                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    report.Export(pdfExport, ms);
                    ms.Flush();
                    ms.Position = 0;
                    TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                    TempData["PdfFileName"] = $"NotasIParcial_{ididentidad}.pdf";
                    return View("PrevisualizarPdf");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }
        }

        // Métodos similares para II, III y IV Parcial (eliminando referencias a Sistema)
        // ...
    }
}