using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysnaraIMAVA.Models;
using FastReport;
using FastReport.Export.PdfSimple;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace SysnaraIMAVA.Controllers
{
    public class ImprimirNotasTradicionalController : Controller
    {
        private readonly DbimavaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImprimirNotasTradicionalController(DbimavaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
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

        public IActionResult GenerarNotasIParcialPrimaria(int idaño, string idestudiante)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(idestudiante))
            {
                return BadRequest("Se requieren un ID de año y un ID de estudiante válidos.");
            }

            // Consulta para notas
            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Idestudiante == idestudiante)
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

            // Consulta para personalidad
            var personalidad = _context.Personalidads
                .Where(p => p.Idaño == idaño && p.Idestudiante == idestudiante)
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
                return NotFound($"No se encontraron datos para el estudiante con ID {idestudiante} en el año {idaño}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotasTradicional", "FR_ImprimirNotasTradicional_Primaria_I_Parcial.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

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
                TempData["PdfFileName"] = $"NotasIParcial_{idestudiante}.pdf";
                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarNotasIIParcialPrimaria(int idaño, string idestudiante)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(idestudiante))
            {
                return BadRequest("Se requieren un ID de año y un ID de estudiante válidos.");
            }

            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Idestudiante == idestudiante)
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

            var personalidadIis = _context.PersonalidadIips
                .Where(p => p.Idaño == idaño && p.Idestudiante == idestudiante)
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
                    p.IdpersonalidadIi,
                    p.Puntualidad,
                    p.OrdenYPresentacion,
                    p.Moralidad,
                    p.EspirituDeTrabajo,
                    p.Sociabilidad,
                    p.Inasistencias
                }).ToList();

            if (!notas.Any())
            {
                return NotFound($"No se encontraron datos para el estudiante con ID {idestudiante} en el año {idaño}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotasTradicional", "FR_ImprimirNotasTradicional_Primaria_II_Parcial.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            report.RegisterData(notas, "frImprimirNotasIIParcialPrimariaRef");
            report.RegisterData(personalidadIis, "frImprimirPersonalidadIIRef");

            report.GetDataSource("NOTA").Enabled = true;
            report.GetDataSource("PERSONALIDADII").Enabled = true;

            report.SetParameterValue("ReportTitle", $"Notas II Parcial");
            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;
                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"NotasIIParcial_{idestudiante}.pdf";
                return View("PrevisualizarPdf");
            }
        }

        // Los métodos restantes (III Parcial, IV Parcial, y los de BTP) deben seguir el mismo patrón:
        // 1. Cambiar parámetro de ididentidad por idestudiante
        // 2. Ajustar las consultas para usar Idestudiante
        // 3. Eliminar referencias a propiedades que no existen en el modelo

        private IActionResult GenerarReporte(int idaño, string idestudiante, string reportFileName, string reportType)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(idestudiante))
            {
                return BadRequest("Datos inválidos");
            }

            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Idestudiante == idestudiante)
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

            if (!notas.Any())
            {
                return NotFound("No se encontraron datos");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotasTradicional", reportFileName);

            try
            {
                report.Load(reportPath);
                report.RegisterData(notas, "Notas");
                report.Prepare();

                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    report.Export(pdfExport, ms);
                    ms.Flush();
                    ms.Position = 0;
                    TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                    TempData["PdfFileName"] = $"{reportType}_{idestudiante}.pdf";
                    return View("PrevisualizarPdf");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}