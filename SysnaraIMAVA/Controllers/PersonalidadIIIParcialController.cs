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

namespace SysnaraIMAVA.Controllers
{
    public class PersonalidadIiiParcialController : Controller
    {
        private readonly DbimavaContext _context;

        public PersonalidadIiiParcialController(DbimavaContext context)
        {
            _context = context;
        }

        // GET: PersonalidadIiiParcialController
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
        public async Task<JsonResult> GetPersonalidadesDelParcial(int idAño, string idGrado)
        {
            var query = _context.PersonalidadIiips
                .Where(m => m.Idaño == idAño);

            if (!string.IsNullOrEmpty(idGrado))
            {
                query = query.Where(m => m.Idgrado == idGrado);
            }

            var personalidades = await query
                .Select(m => new {
                    m.Idaño,
                    m.Idest,
                    m.Idestudiante,
                    nombreestudiante = m.NombreEstudiante,
                    m.Genero,
                    idpersonalidadiii = m.IdpersonalidadIii,
                    m.Puntualidad,
                    ordenypresentacion = m.OrdenYPresentacion,
                    m.Moralidad,
                    espiritudetrabajo = m.EspirituDeTrabajo,
                    m.Sociabilidad,
                    m.Inasistencias
                })
                .ToListAsync();

            if (personalidades == null || !personalidades.Any())
            {
                return Json(new { success = false, message = "Ops, no hay datos de personalidad registrados en este Grado y III Parcial." });
            }

            return Json(new { success = true, data = personalidades });
        }

        public IActionResult VerificarDatos(int? idAño, string idGrado)
        {
            if (idAño == null || string.IsNullOrEmpty(idGrado))
            {
                return Json(new { existeDatos = false });
            }

            var datosFiltrados = _context.PersonalidadIiips
                                          .Where(p => p.Idaño == idAño && p.Idgrado == idGrado)
                                          .ToList();

            return Json(new { existeDatos = datosFiltrados.Any() });
        }

        public IActionResult DescargarExcel(int? idAño, string idGrado)
        {
            if (idAño == null || string.IsNullOrEmpty(idGrado))
            {
                return BadRequest("Debe proporcionar ambos parámetros: Año y Grado.");
            }

            try
            {
                var datosFiltrados = _context.PersonalidadIiips
                                              .Where(p => p.Idaño == idAño && p.Idgrado == idGrado)
                                              .ToList();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Personalidad");

                    var headerCell = worksheet.Cells["A1:N1"];
                    headerCell.Merge = true;
                    headerCell.Value = "PERSONALIDAD ESTUDIANTIL - III PARCIAL";
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Font.Size = 26;
                    headerCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    var classificationHeader = worksheet.Cells["L2:N2"];
                    classificationHeader.Merge = true;
                    classificationHeader.Value = "CLASIFICACIÓN";
                    classificationHeader.Style.Font.Bold = true;
                    classificationHeader.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    classificationHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    classificationHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    classificationHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    classificationHeader.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    classificationHeader.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    var classificationValues = new string[] { "SOBRESALIENTE", "MUY BUENO", "BUENO", "DEBE MEJORAR" };
                    for (int i = 0; i < classificationValues.Length; i++)
                    {
                        var cellRange = worksheet.Cells[$"L{i + 3}:N{i + 3}"];
                        cellRange.Merge = true;
                        cellRange.Value = classificationValues[i];
                        cellRange.Style.Font.Bold = true;
                        cellRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cellRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cellRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        cellRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        cellRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cellRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    var editandoCell = worksheet.Cells["A2:C2"];
                    editandoCell.Merge = true;
                    editandoCell.Value = "EDITANDO";
                    editandoCell.Style.Font.Bold = true;
                    editandoCell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    editandoCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    editandoCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    editandoCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    editandoCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    editandoCell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    worksheet.Cells[7, 1].Value = "Año";
                    worksheet.Cells[7, 2].Value = "ID Estudiante";
                    worksheet.Cells[7, 3].Value = "DNI";
                    worksheet.Cells[7, 4].Value = "Nombre Estudiante";
                    worksheet.Cells[7, 5].Value = "Género";
                    worksheet.Cells[7, 6].Value = "Grado";
                    worksheet.Cells[7, 7].Value = "Sección";
                    worksheet.Cells[7, 8].Value = "Jornada";
                    worksheet.Cells[7, 9].Value = "ID Personalidad";
                    worksheet.Cells[7, 10].Value = "Puntualidad";
                    worksheet.Cells[7, 11].Value = "Orden y Presentación";
                    worksheet.Cells[7, 12].Value = "Moralidad";
                    worksheet.Cells[7, 13].Value = "Espíritu de Trabajo";
                    worksheet.Cells[7, 14].Value = "Sociabilidad";
                    worksheet.Cells[7, 15].Value = "Inasistencias";

                    for (int i = 1; i <= 15; i++)
                    {
                        var headerCellRow7 = worksheet.Cells[7, i];
                        headerCellRow7.Style.Font.Bold = true;
                        headerCellRow7.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        headerCellRow7.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerCellRow7.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                        headerCellRow7.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        headerCellRow7.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        headerCellRow7.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    for (int i = 0; i < datosFiltrados.Count; i++)
                    {
                        var personalidad = datosFiltrados[i];
                        worksheet.Cells[i + 8, 1].Value = personalidad.Idaño;
                        worksheet.Cells[i + 8, 2].Value = personalidad.Idest;
                        worksheet.Cells[i + 8, 3].Value = personalidad.Idestudiante;
                        worksheet.Cells[i + 8, 4].Value = personalidad.NombreEstudiante;
                        worksheet.Cells[i + 8, 5].Value = personalidad.Genero;
                        worksheet.Cells[i + 8, 6].Value = personalidad.Idgrado;
                        worksheet.Cells[i + 8, 7].Value = personalidad.Seccion;
                        worksheet.Cells[i + 8, 8].Value = personalidad.Jornada;
                        worksheet.Cells[i + 8, 9].Value = personalidad.IdpersonalidadIii;
                        worksheet.Cells[i + 8, 10].Value = personalidad.Puntualidad;
                        worksheet.Cells[i + 8, 11].Value = personalidad.OrdenYPresentacion;
                        worksheet.Cells[i + 8, 12].Value = personalidad.Moralidad;
                        worksheet.Cells[i + 8, 13].Value = personalidad.EspirituDeTrabajo;
                        worksheet.Cells[i + 8, 14].Value = personalidad.Sociabilidad;
                        worksheet.Cells[i + 8, 15].Value = personalidad.Inasistencias;
                    }

                    for (int i = 1; i <= 15; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    worksheet.Cells.Style.Locked = true;

                    for (int i = 8; i < 8 + datosFiltrados.Count; i++)
                    {
                        worksheet.Cells[i, 10].Style.Locked = false;
                        worksheet.Cells[i, 11].Style.Locked = false;
                        worksheet.Cells[i, 12].Style.Locked = false;
                        worksheet.Cells[i, 13].Style.Locked = false;
                        worksheet.Cells[i, 14].Style.Locked = false;
                        worksheet.Cells[i, 15].Style.Locked = false;
                    }

                    worksheet.Protection.SetPassword("1234");
                    worksheet.Protection.IsProtected = true;
                    worksheet.Protection.AllowSelectLockedCells = false;
                    worksheet.Protection.AllowSelectUnlockedCells = true;

                    var excelFile = package.GetAsByteArray();
                    var fileName = $"Personalidad_{idAño}_{idGrado}_III.xlsx";
                    return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Ocurrió un error al generar el archivo Excel: " + ex.Message);
            }
        }

        public IActionResult DescargarExcelenBlancoPersonalidadII(int? idAño, string idGrado)
        {
            if (idAño == null || string.IsNullOrEmpty(idGrado))
            {
                return BadRequest("Debe proporcionar ambos parámetros: Año y Grado.");
            }

            try
            {
                var datosFiltrados = _context.PersonalidadIiips
                                              .Where(p => p.Idaño == idAño && p.Idgrado == idGrado && p.IdpersonalidadIii != null)
                                              .ToList();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Personalidad III Parcial");
                    worksheet.View.ShowGridLines = true;

                    var headerCell = worksheet.Cells["A1:N1"];
                    headerCell.Merge = true;
                    headerCell.Value = "PERSONALIDAD ESTUDIANTIL - III PARCIAL";
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Font.Size = 26;
                    headerCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    var classificationHeader = worksheet.Cells["L2:N2"];
                    classificationHeader.Merge = true;
                    classificationHeader.Value = "CLASIFICACIÓN";
                    classificationHeader.Style.Font.Bold = true;
                    classificationHeader.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    classificationHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    classificationHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    classificationHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    classificationHeader.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    classificationHeader.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    var classificationValues = new string[] { "SOBRESALIENTE", "MUY BUENO", "BUENO", "DEBE MEJORAR" };
                    for (int i = 0; i < classificationValues.Length; i++)
                    {
                        var cellRange = worksheet.Cells[$"L{i + 3}:N{i + 3}"];
                        cellRange.Merge = true;
                        cellRange.Value = classificationValues[i];
                        cellRange.Style.Font.Bold = true;
                        cellRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cellRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cellRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        cellRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        cellRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cellRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    var nuevoIngresoCell = worksheet.Cells["A2:C2"];
                    nuevoIngresoCell.Merge = true;
                    nuevoIngresoCell.Value = "NUEVOINGRESO";
                    nuevoIngresoCell.Style.Font.Bold = true;
                    nuevoIngresoCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    nuevoIngresoCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    nuevoIngresoCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    nuevoIngresoCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    nuevoIngresoCell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    string[] headers = new string[] {
                        "Año", "ID Estudiante", "DNI", "Nombre Estudiante", "Género",
                        "Grado", "Sección", "Jornada", "ID Personalidad", "Puntualidad",
                        "Orden y Presentación", "Moralidad", "Espíritu de Trabajo", "Sociabilidad",
                        "Inasistencias"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var headerCellRow7 = worksheet.Cells[7, i + 1];
                        headerCellRow7.Value = headers[i];
                        headerCellRow7.Style.Font.Bold = true;
                        headerCellRow7.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        headerCellRow7.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerCellRow7.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                        headerCellRow7.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        headerCellRow7.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        headerCellRow7.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    int lastRow = 7;
                    if (datosFiltrados.Count == 0)
                    {
                        var estudiantesMatriculados = _context.Matriculas
                                                             .Where(m => m.Idaño == idAño && m.Idgrado == idGrado)
                                                             .OrderBy(m => m.Genero)
                                                             .ThenBy(m => m.NombreEstudiante)
                                                             .ToList();

                        int row = 8;
                        foreach (var estudiante in estudiantesMatriculados)
                        {
                            worksheet.Cells[row, 1].Value = idAño;
                            worksheet.Cells[row, 2].Value = estudiante.Idest;
                            worksheet.Cells[row, 3].Value = estudiante.Idestudiante;
                            worksheet.Cells[row, 4].Value = estudiante.NombreEstudiante;
                            worksheet.Cells[row, 5].Value = estudiante.Genero;
                            worksheet.Cells[row, 6].Value = idGrado;
                            worksheet.Cells[row, 7].Value = estudiante.Seccion;
                            worksheet.Cells[row, 8].Value = estudiante.Jornada;
                            worksheet.Cells[row, 9].Value = 0;
                            worksheet.Cells[row, 10].Value = "";
                            worksheet.Cells[row, 11].Value = "";
                            worksheet.Cells[row, 12].Value = "";
                            worksheet.Cells[row, 13].Value = "";
                            worksheet.Cells[row, 14].Value = "";
                            worksheet.Cells[row, 15].Value = 0;

                            var rowRange = worksheet.Cells[row, 1, row, 15];
                            rowRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            rowRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            if (row % 2 == 0)
                            {
                                rowRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                rowRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            }

                            row++;
                            lastRow = row - 1;
                        }
                    }
                    else
                    {
                        int row = 8;
                        foreach (var dato in datosFiltrados)
                        {
                            var rowRange = worksheet.Cells[row, 1, row, 15];
                            rowRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            rowRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            if (row % 2 == 0)
                            {
                                rowRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                rowRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            }

                            worksheet.Cells[row, 1].Value = dato.Idaño;
                            worksheet.Cells[row, 2].Value = dato.Idest;
                            worksheet.Cells[row, 3].Value = dato.Idestudiante;
                            worksheet.Cells[row, 4].Value = dato.NombreEstudiante;
                            worksheet.Cells[row, 5].Value = dato.Genero;
                            worksheet.Cells[row, 6].Value = dato.Idgrado;
                            worksheet.Cells[row, 7].Value = dato.Seccion;
                            worksheet.Cells[row, 8].Value = dato.Jornada;
                            worksheet.Cells[row, 9].Value = dato.IdpersonalidadIii;
                            worksheet.Cells[row, 10].Value = dato.Puntualidad;
                            worksheet.Cells[row, 11].Value = dato.OrdenYPresentacion;
                            worksheet.Cells[row, 12].Value = dato.Moralidad;
                            worksheet.Cells[row, 13].Value = dato.EspirituDeTrabajo;
                            worksheet.Cells[row, 14].Value = dato.Sociabilidad;
                            worksheet.Cells[row, 15].Value = dato.Inasistencias;

                            row++;
                            lastRow = row - 1;
                        }
                    }

                    for (int i = 1; i <= 15; i++)
                    {
                        worksheet.Column(i).AutoFit();
                        if (worksheet.Column(i).Width < 8)
                        {
                            worksheet.Column(i).Width = 8;
                        }
                    }

                    var dataRange = worksheet.Cells[8, 1, lastRow, 15];
                    dataRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                    for (int row = 8; row <= lastRow; row++)
                    {
                        for (int col = 10; col <= 15; col++)
                        {
                            var cell = worksheet.Cells[row, col];
                            cell.Style.Locked = false;
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 240));
                            cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }

                    worksheet.Protection.IsProtected = true;
                    worksheet.Protection.AllowSelectLockedCells = false;
                    worksheet.Protection.AllowSelectUnlockedCells = true;
                    worksheet.View.FreezePanes(8, 1);

                    var fileName = $"Personalidad_III_{idAño}_{idGrado}.xlsx";
                    var fileContents = package.GetAsByteArray();
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al generar el archivo Excel: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubirPersonalidades(IFormFile archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.Length == 0)
            {
                return Json(new { error = "Por favor, selecciona un archivo." });
            }

            try
            {
                using (var package = new ExcelPackage(archivoExcel.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    string tipoIngreso = worksheet.Cells["A2"].Text?.ToUpper();

                    if (tipoIngreso != "NUEVOINGRESO" && tipoIngreso != "EDITANDO")
                    {
                        return Json(new { error = "El archivo debe contener un valor válido (NUEVOINGRESO o EDITANDO) en la celda A2." });
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    List<PersonalidadIiip> personalidades = new List<PersonalidadIiip>();

                    for (int row = 8; row <= rowCount; row++)
                    {
                        var personalidad = new PersonalidadIiip
                        {
                            Idaño = int.TryParse(worksheet.Cells[row, 1].Text, out var año) ? año : (int?)null,
                            Idest = int.TryParse(worksheet.Cells[row, 2].Text, out var idEst) ? idEst : (int?)null,
                            Idestudiante = worksheet.Cells[row, 3].Text,
                            NombreEstudiante = worksheet.Cells[row, 4].Text,
                            Genero = worksheet.Cells[row, 5].Text,
                            Idgrado = worksheet.Cells[row, 6].Text,
                            Grado = "",
                            Seccion = worksheet.Cells[row, 7].Text,
                            Jornada = worksheet.Cells[row, 8].Text,
                            IdpersonalidadIii = int.TryParse(worksheet.Cells[row, 9].Text, out var idPersonalidad) ? idPersonalidad : 0,
                            Puntualidad = string.IsNullOrEmpty(worksheet.Cells[row, 10].Text) ? null : worksheet.Cells[row, 10].Text,
                            OrdenYPresentacion = string.IsNullOrEmpty(worksheet.Cells[row, 11].Text) ? null : worksheet.Cells[row, 11].Text,
                            Moralidad = string.IsNullOrEmpty(worksheet.Cells[row, 12].Text) ? null : worksheet.Cells[row, 12].Text,
                            EspirituDeTrabajo = string.IsNullOrEmpty(worksheet.Cells[row, 13].Text) ? null : worksheet.Cells[row, 13].Text,
                            Sociabilidad = string.IsNullOrEmpty(worksheet.Cells[row, 14].Text) ? null : worksheet.Cells[row, 14].Text,
                            Inasistencias = int.TryParse(worksheet.Cells[row, 15].Text, out var inasistencias) ? inasistencias : (int?)null
                        };

                        if (tipoIngreso == "NUEVOINGRESO")
                        {
                            var existingRecord = _context.PersonalidadIiips
                                .FirstOrDefault(p => p.Idest == personalidad.Idest && p.Idaño == personalidad.Idaño);

                            if (existingRecord != null)
                            {
                                return Json(new { error = "Ya existe un registro con el mismo ID Estudiante y Año. No se puede subir nuevamente." });
                            }

                            _context.PersonalidadIiips.Add(personalidad);
                        }
                        else if (tipoIngreso == "EDITANDO")
                        {
                            var personalidadExistente = _context.PersonalidadIiips
                                .FirstOrDefault(p => p.Idest == personalidad.Idest && p.Idaño == personalidad.Idaño && p.IdpersonalidadIii == personalidad.IdpersonalidadIii);

                            if (personalidadExistente != null)
                            {
                                personalidadExistente.Puntualidad = personalidad.Puntualidad;
                                personalidadExistente.OrdenYPresentacion = personalidad.OrdenYPresentacion;
                                personalidadExistente.Moralidad = personalidad.Moralidad;
                                personalidadExistente.EspirituDeTrabajo = personalidad.EspirituDeTrabajo;
                                personalidadExistente.Sociabilidad = personalidad.Sociabilidad;
                                personalidadExistente.Inasistencias = personalidad.Inasistencias;

                                _context.PersonalidadIiips.Update(personalidadExistente);
                            }
                            else
                            {
                                return Json(new { error = $"No se encontró el registro para actualizar (IdEst: {personalidad.Idest}, Año: {personalidad.Idaño})." });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = "Personalidades subidas correctamente." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Ocurrió un error al procesar el archivo: {ex.Message}" });
            }
        }
    }
}