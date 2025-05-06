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
    public class ReporteAnglosajonController : Controller
    {
        private readonly DbimavaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReporteAnglosajonController(DbimavaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Inyectamos IWebHostEnvironment
        }

        // GET: ReportesController
        public async Task<ActionResult> IndexAsync()
        {
            var años = await _context.Años.ToListAsync();
            ViewBag.Años = años;
            return View();
        }
        // Nuevo método para obtener grados por año ***************** inicio
        [HttpGet]
        public async Task<JsonResult> GetGrados(int idAño)
        {
            var grados = await _context.Grados
                .Where(g => g.Idaño == idAño)
                .Select(g => new { g.Idgrado, GradoCompleto = g.Idgrado + " - " + g.Grado1 })
                .ToListAsync();

            return Json(grados);
        }
        //**************************************************************** fin
        // Nuevo método para obtener matrículas filtradas  *************** inicio

        [HttpGet]
        public async Task<JsonResult> GetMatriculas(int idAño, string idGrado, string sistema)
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
                    m.Idest,
                    m.Idsi,
                    m.Ididentidad,
                    //m.Nacionalidad,
                    m.NombreEstudiante,
                    m.FechaNacimiento,
                    m.Genero,
                    //m.Estado,
                    //m.Grado,
                    //m.EstadoIngreso,
                    m.Sistema // Añadimos el campo Sistema
                })
                .ToListAsync();

            return Json(matriculas);
        }

        //FastReport - REPORTE CONTROL DE PAGOS
        public IActionResult GenerarReporteMuestra(string idGrado)
        {
                // Verificar si idGrado es null o vacío
                if (string.IsNullOrEmpty(idGrado))
                {
                    return BadRequest("Se requiere un ID de grado válido.");
                }

                // Ya no necesitamos extraer o limpiar el idGrado, usamos el valor completo
                var datos = _context.Matriculas
                    .Where(m => m.Idgrado == idGrado)
                    .Select(m => new
                    {
                        IDAño = m.Idaño,
                        IDEst = m.Idest,
                        IDIdentidad = m.Ididentidad,
                        Nombre_Estudiante = m.NombreEstudiante,
                        Genero = m.Genero,
                        IDGrado = m.Idgrado,
                        Grado = m.Grado,
                        Seccion = m.Seccion,
                        Jornada = m.Jornada,
                        NivelAcademico = m.NivelDescripcion,
                        Periodo = m.SistemaTiempo,
                        Sistema = m.Sistema
                    }).ToList();

                // Verificar si se encontraron datos
                if (!datos.Any())
                {
                    return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
                }

            Report report = new Report();
            string reportPath = $"{Directory.GetCurrentDirectory()}/Views/ReporteAnglosajon/FR.frx";
            try
            {
                report.Load(reportPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar el reporte: {ex.Message}");
            }

            // Registrar los datos en el reporte
            report.RegisterData(datos, "MATRICULA");
            report.GetDataSource("MATRICULA").Enabled = true;

            // Obtener el nombre del grado para el título del reporte
            var nombreGrado = datos.FirstOrDefault()?.Grado ?? "Desconocido";

            // Establecer un parámetro para el título del reporte
            report.SetParameterValue("ReportTitle", $"Reporte de Estudiantes - Grado {nombreGrado}");

            // Preparar el reporte
            report.Prepare();

            //PREVISUALIZAR el documento PDF y LUEGO poder descargarlo
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

        // FastReport - REPORTE DE MATRICULA
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
                    IDIdentidad = m.Ididentidad,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion,
                    Periodo = m.SistemaTiempo,
                    Sistema = m.Sistema
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();

            // Cambia esta línea para usar la ruta física correcta
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteAnglosajon", "FR_Rep_Matricula.frx");

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


        // FastReport - REPORTE DE CONTROL DE ACUMULADOS
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
                    IDIdentidad = m.Ididentidad,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion,
                    Periodo = m.SistemaTiempo,
                    Sistema = m.Sistema
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();

            // Cambia esta línea para usar la ruta física correcta
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteAnglosajon", "FR_Rep_Acumulados.frx");

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

        // FastReport - REPORTE DE CONTROL DE ASISTENCIA
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
                    IDIdentidad = m.Ididentidad,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion,
                    Periodo = m.SistemaTiempo,
                    Sistema = m.Sistema
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();

            // Cambia esta línea para usar la ruta física correcta
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteAnglosajon", "FR_Rep_Asistencia.frx");

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

        // FastReport - REPORTE DE PERSONALIDAD
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
                    IDIdentidad = m.Ididentidad,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Genero = m.Genero,
                    CelEst = m.CelularEstudiante,
                    IDGrado = m.Idgrado,
                    Grado = m.Grado,
                    Seccion = m.Seccion,
                    Jornada = m.Jornada,
                    NivelAcademico = m.NivelDescripcion,
                    Periodo = m.SistemaTiempo,
                    Sistema = m.Sistema
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron estudiantes para el grado con ID {idGrado}.");
            }

            Report report = new Report();

            // Cambia esta línea para usar la ruta física correcta
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteAnglosajon", "FR_Rep_Personalidad.frx");

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
        //×××××§§§§§§§§§§§§§ CONSTANCIAS §§§§§§§§§§§§××××××××××××××××××
        public IActionResult GenerarConstanciaMatricula(int idaño, string ididentidad)
        {
            if (idaño <= 0 || string.IsNullOrEmpty(ididentidad))
            {
                return BadRequest("Se requieren un ID de año y un ID de identidad válidos.");
            }

            var datos = _context.Matriculas
                .Where(m => m.Idaño == idaño && m.Ididentidad == ididentidad)
                .Select(m => new
                {
                    IDAño = m.Idaño,
                    IDEst = m.Idest,
                    IDIdentidad = m.Ididentidad,
                    Nombre_Estudiante = m.NombreEstudiante,
                    Fecha_Nacimiento = m.FechaNacimiento,
                    Genero = m.Genero,
                    CelularEstudiante = m.CelularEstudiante,
                    Sistema = m.Sistema,
                    Grado = m.Grado,
                    Seccion = m.Seccion
                }).ToList();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron datos para el estudiante con ID de identidad {ididentidad} en el año {idaño}.");
            }

            Report report = new Report();

            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "ReporteAnglosajon", "FR_Const_Matricula.frx");

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
                TempData["PdfFileName"] = $"Constancia_Matricula_{ididentidad}.pdf";

                return View("PrevisualizarPdf");
            }
        }

        public IActionResult ReporteAnglosajon()
        {
            return View();
        }
    }
}
