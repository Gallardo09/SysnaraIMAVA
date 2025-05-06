using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysnaraIMAVA.Models;
using FastReport.Export.PdfSimple;
using FastReport.Export;
using FastReport;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using OfficeOpenXml;
using System.Linq;
using System.Threading.Tasks;
using FastReport.Web;

namespace SysnaraIMAVA.Controllers
{
    public class ImprimirNotasController : Controller
    {
        private readonly DbimavaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImprimirNotasController(DbimavaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Inyectamos IWebHostEnvironment
        }
        // GET: ImprimirNotasController
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
        public async Task<JsonResult> GetListaEstudiantesMatricula(int idAño, string idGrado, string sistema)
        {
            var query = _context.Matriculas
                .Where(m => m.Idaño == idAño);

            if (!string.IsNullOrEmpty(idGrado))
            {
                query = query.Where(m => m.Idgrado == idGrado);
            }

            if (!string.IsNullOrEmpty(sistema))
            {
                query = query.Where(m => m.Sistema == sistema);
            }

            var matriculas = await query
                .Select(m => new {
                    m.Idaño,
                    m.Idest,
                    m.Idsi,
                    m.Ididentidad,
                    m.NombreEstudiante,
                    m.Genero,
                    m.Grado,
                    m.Sistema
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

            // Consulta para notas
            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Ididentidad == ididentidad)
                .Select(n => new
                {
                    n.Idaño,
                    n.Idest,
                    n.Ididentidad,
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
                    n.IndicePromocion,
                    n.Sistema,
                    n.SistemaClase,
                    n.SistemaTiempo
                }).ToList();

            // Consulta para personalidad
            var personalidad = _context.Personalidads
                .Where(p => p.Idaño == idaño && p.Ididentidad == ididentidad)
                .Select(p => new
                {
                    p.Idaño,
                    p.Idest,
                    p.Ididentidad,
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
                    p.Inasistencias,
                    p.Sistema,
                    p.SistemaClase,
                    p.SistemaTiempo
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
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            // Registrar ambas fuentes de datos
            report.RegisterData(notas, "frImprimirNotasIParcialPrimariaRef");
            report.RegisterData(personalidad, "frImprimirPersonalidadRef");

            // Habilitar ambas fuentes de datos
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


        public IActionResult GenerarNotasIIParcialPrimaria(int idaño, string ididentidad)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(ididentidad))
            {
                return BadRequest("Se requieren un ID de año y un ID de identidad válidos.");
            }

            // Consulta para notas
            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Ididentidad == ididentidad)
                .Select(n => new
                {
                    n.Idaño,
                    n.Idest,
                    n.Ididentidad,
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
                    n.IndicePromocion,
                    n.Sistema,
                    n.SistemaClase,
                    n.SistemaTiempo
                }).ToList();

            // Consulta para personalidad
            var personalidadIis = _context.PersonalidadIis
                .Where(p => p.Idaño == idaño && p.Ididentidad == ididentidad)
                .Select(p => new
                {
                    p.Idaño,
                    p.Idest,
                    p.Ididentidad,
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
                    p.Inasistencias,
                    p.Sistema,
                    p.SistemaClase,
                    p.SistemaTiempo
                }).ToList();

            if (!notas.Any())
            {
                return NotFound($"No se encontraron datos para el estudiante con ID de identidad {ididentidad} en el año {idaño}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotas", "FR_ImprimirNotas_Primaria_II_Parcial.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            // Registrar ambas fuentes de datos
            report.RegisterData(notas, "frImprimirNotasIIParcialPrimariaRef");
            report.RegisterData(personalidadIis, "frImprimirPersonalidadIIRef");

            // Habilitar ambas fuentes de datos
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
                TempData["PdfFileName"] = $"NotasIIParcial_{ididentidad}.pdf";
                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarNotasIIIParcialPrimaria(int idaño, string ididentidad)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(ididentidad))
            {
                return BadRequest("Se requieren un ID de año y un ID de identidad válidos.");
            }

            // Consulta para notas
            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Ididentidad == ididentidad)
                .Select(n => new
                {
                    n.Idaño,
                    n.Idest,
                    n.Ididentidad,
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
                    n.IndicePromocion,
                    n.Sistema,
                    n.SistemaClase,
                    n.SistemaTiempo
                }).ToList();

            // Consulta para personalidad
            var personalidadIiis = _context.PersonalidadIiis
                .Where(p => p.Idaño == idaño && p.Ididentidad == ididentidad)
                .Select(p => new
                {
                    p.Idaño,
                    p.Idest,
                    p.Ididentidad,
                    p.NombreEstudiante,
                    p.Genero,
                    p.Idgrado,
                    p.Grado,
                    p.Seccion,
                    p.Jornada,
                    p.IdpersonalidadIii,
                    p.Puntualidad,
                    p.OrdenYPresentacion,
                    p.Moralidad,
                    p.EspirituDeTrabajo,
                    p.Sociabilidad,
                    p.Inasistencias,
                    p.Sistema,
                    p.SistemaClase,
                    p.SistemaTiempo
                }).ToList();

            if (!notas.Any())
            {
                return NotFound($"No se encontraron datos para el estudiante con ID de identidad {ididentidad} en el año {idaño}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotas", "FR_ImprimirNotas_Primaria_III_Parcial.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            // Registrar ambas fuentes de datos
            report.RegisterData(notas, "frImprimirNotasIIIParcialPrimariaRef");
            report.RegisterData(personalidadIiis, "frImprimirPersonalidadIIIRef");

            // Habilitar ambas fuentes de datos
            report.GetDataSource("NOTA").Enabled = true;
            report.GetDataSource("PERSONALIDADIII").Enabled = true;

            report.SetParameterValue("ReportTitle", $"Notas III Parcial");
            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;
                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"NotasIIIParcial_{ididentidad}.pdf";
                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarNotasIVParcialPrimaria(int idaño, string ididentidad)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(ididentidad))
            {
                return BadRequest("Se requieren un ID de año y un ID de identidad válidos.");
            }

            // Consulta para notas
            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Ididentidad == ididentidad)
                .Select(n => new
                {
                    n.Idaño,
                    n.Idest,
                    n.Ididentidad,
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
                    n.IndicePromocion,
                    n.Sistema,
                    n.SistemaClase,
                    n.SistemaTiempo
                }).ToList();

            // Consulta para personalidad
            var personalidadIvs = _context.PersonalidadIvs
                .Where(p => p.Idaño == idaño && p.Ididentidad == ididentidad)
                .Select(p => new
                {
                    p.Idaño,
                    p.Idest,
                    p.Ididentidad,
                    p.NombreEstudiante,
                    p.Genero,
                    p.Idgrado,
                    p.Grado,
                    p.Seccion,
                    p.Jornada,
                    p.IdpersonalidadIv,
                    p.Puntualidad,
                    p.OrdenYPresentacion,
                    p.Moralidad,
                    p.EspirituDeTrabajo,
                    p.Sociabilidad,
                    p.Inasistencias,
                    p.Sistema,
                    p.SistemaClase,
                    p.SistemaTiempo
                }).ToList();

            if (!notas.Any())
            {
                return NotFound($"No se encontraron datos para el estudiante con ID de identidad {ididentidad} en el año {idaño}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotas", "FR_ImprimirNotas_Primaria_IV_Parcial.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            // Registrar ambas fuentes de datos
            report.RegisterData(notas, "frImprimirNotasIVParcialPrimariaRef");
            report.RegisterData(personalidadIvs, "frImprimirPersonalidadIVRef");

            // Habilitar ambas fuentes de datos
            report.GetDataSource("NOTA").Enabled = true;
            report.GetDataSource("PERSONALIDADIV").Enabled = true;

            report.SetParameterValue("ReportTitle", $"Notas IV Parcial");
            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;
                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"NotasIVParcial_{ididentidad}.pdf";
                return View("PrevisualizarPdf");
            }
        }

        private IActionResult GenerarReporte(int idaño, string ididentidad, string reportFileName, string reportType)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(ididentidad))
            {
                return BadRequest("Datos inválidos");
            }

            var notas = _context.Notas
                .Where(n => n.Idaño == idaño && n.Ididentidad == ididentidad && n.Sistema == "TRADICIONAL")
                .Select(n => new
                {
                    n.Idaño,
                    n.Idest,
                    n.Ididentidad,
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
                    n.IndicePromocion,
                    n.Sistema,
                    n.SistemaClase,
                    n.SistemaTiempo
                }).ToList();

            if (!notas.Any())
            {
                return NotFound("No se encontraron datos");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ImprimirNotas", reportFileName);

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
                    TempData["PdfFileName"] = $"{reportType}_{ididentidad}.pdf";
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