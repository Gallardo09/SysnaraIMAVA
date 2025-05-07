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

namespace SysnaraIMAVA.Controllers
{
    public class ReporteTradicionalController : Controller
    {
        private readonly DbimavaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReporteTradicionalController(DbimavaContext context, IWebHostEnvironment webHostEnvironment)
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
        public async Task<JsonResult> GetMatriculas(int idAño, string idGrado)
        {
            var query = _context.Matriculas
                .Where(m => m.Idaño == idAño);

            if (!string.IsNullOrEmpty(idGrado))
            {
                query = query.Where(m => m.Idgrado == idGrado);
            }

            var matriculas = await query
                .Select(m => new {
                    m.Idest,
                    m.Idestudiante,
                    m.NombreEstudiante,
                    m.FechaNacimiento,
                    m.Genero
                })
                .ToListAsync();

            return Json(matriculas);
        }

        public IActionResult GenerarReporteMuestra(string idGrado)
        {
            if (string.IsNullOrEmpty(idGrado))
            {
                return BadRequest("Se requiere un ID de grado válido.");
            }

            var datos = _context.Matriculas
                .Where(m => m.Idgrado == idGrado)
                .Select(m => new
                {
                    IDAño = m.Idaño,
                    IDEst = m.Idest,
                    IDEstudiante = m.Idestudiante,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteTradicional", "FR_Control_Pagos.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            report.RegisterData(datos, "MATRICULA");
            report.GetDataSource("MATRICULA").Enabled = true;

            var nombreGrado = datos.FirstOrDefault()?.Grado ?? "Desconocido";
            report.SetParameterValue("ReportTitle", $"Reporte de Estudiantes - Grado {nombreGrado}");

            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;

                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"ControlDePagos_{idGrado}.pdf";

                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarReporteMatricula(string idGrado)
        {
            if (string.IsNullOrEmpty(idGrado))
            {
                return BadRequest("Se requiere un ID de grado válido.");
            }

            var datos = _context.Matriculas
                .Where(m => m.Idgrado == idGrado)
                .Select(m => new
                {
                    IDAño = m.Idaño,
                    IDEst = m.Idest,
                    IDEstudiante = m.Idestudiante,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteTradicional", "FR_Rep_Matricula.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            report.RegisterData(datos, "frMatriculaRef");
            report.GetDataSource("MATRICULA").Enabled = true;

            var nombreGrado = datos.FirstOrDefault()?.Grado ?? "Desconocido";
            report.SetParameterValue("ReportTitle", $"Reporte de Estudiantes - Grado {nombreGrado}");

            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;

                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"ReporteMatricula_{idGrado}.pdf";

                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarReporteAcumulados(string idGrado)
        {
            if (string.IsNullOrEmpty(idGrado))
            {
                return BadRequest("Se requiere un ID de grado válido.");
            }

            var datos = _context.Matriculas
                .Where(m => m.Idgrado == idGrado)
                .Select(m => new
                {
                    IDAño = m.Idaño,
                    IDEst = m.Idest,
                    IDEstudiante = m.Idestudiante,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteTradicional", "FR_Rep_Acumulados.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            report.RegisterData(datos, "frAcumuladosRef");
            report.GetDataSource("MATRICULA").Enabled = true;

            var nombreGrado = datos.FirstOrDefault()?.Grado ?? "Desconocido";
            report.SetParameterValue("ReportTitle", $"Reporte de Estudiantes - Grado {nombreGrado}");

            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;

                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"Acumulados_{idGrado}.pdf";

                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarReporteAsistencia(string idGrado)
        {
            if (string.IsNullOrEmpty(idGrado))
            {
                return BadRequest("Se requiere un ID de grado válido.");
            }

            var datos = _context.Matriculas
                .Where(m => m.Idgrado == idGrado)
                .Select(m => new
                {
                    IDAño = m.Idaño,
                    IDEst = m.Idest,
                    IDEstudiante = m.Idestudiante,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteTradicional", "FR_Rep_Asistencia.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            report.RegisterData(datos, "frAsistenciaRef");
            report.GetDataSource("MATRICULA").Enabled = true;

            var nombreGrado = datos.FirstOrDefault()?.Grado ?? "Desconocido";
            report.SetParameterValue("ReportTitle", $"Reporte de Estudiantes - Grado {nombreGrado}");

            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;

                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"Asistencia_{idGrado}.pdf";

                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarReportePersonalidad(string idGrado)
        {
            if (string.IsNullOrEmpty(idGrado))
            {
                return BadRequest("Se requiere un ID de grado válido.");
            }

            var datos = _context.Matriculas
                .Where(m => m.Idgrado == idGrado)
                .Select(m => new
                {
                    IDAño = m.Idaño,
                    IDEst = m.Idest,
                    IDEstudiante = m.Idestudiante,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteTradicional", "FR_Rep_Personalidad.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            report.RegisterData(datos, "frPersonalidadRef");
            report.GetDataSource("MATRICULA").Enabled = true;

            var nombreGrado = datos.FirstOrDefault()?.Grado ?? "Desconocido";
            report.SetParameterValue("ReportTitle", $"Reporte de Personalidad - Grado {nombreGrado}");

            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;

                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"Personalidad_{idGrado}.pdf";

                return View("PrevisualizarPdf");
            }
        }

        public IActionResult GenerarConstanciaMatricula(int idaño, string idestudiante)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(idestudiante))
            {
                return BadRequest("Se requieren un ID de año y un ID de estudiante válidos.");
            }

            var datos = _context.Matriculas
                .Where(m => m.Idaño == idaño && m.Idestudiante == idestudiante)
                .Select(m => new
                {
                    IDAño = m.Idaño,
                    IDEst = m.Idest,
                    IDEstudiante = m.Idestudiante,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Fecha_Nacimiento = m.FechaNacimiento,
                    Genero = m.Genero,
                    CelularEstudiante = m.CelularEstudiante,
                    Grado = m.Grado,
                    Seccion = m.Seccion
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron datos para el estudiante con ID {idestudiante} en el año {idaño}.");
            }

            Report report = new Report();
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteTradicional", "FR_Const_Matricula.frx");

            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            report.RegisterData(datos, "frConstMatriculaRef");
            report.GetDataSource("MATRICULA").Enabled = true;

            var nombreEstudiante = datos.FirstOrDefault()?.Nombre_Estudiante ?? "Desconocido";
            report.SetParameterValue("ReportTitle", $"Constancia de Matrícula - {nombreEstudiante}");

            report.Prepare();

            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                ms.Flush();
                ms.Position = 0;

                TempData["PdfStream"] = Convert.ToBase64String(ms.ToArray());
                TempData["PdfFileName"] = $"Constancia_Matricula_{idestudiante}.pdf";

                return View("PrevisualizarPdf");
            }
        }

        public IActionResult ReporteTradicional()
        {
            return View();
        }
    }
}