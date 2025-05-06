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
    public class PersonalidadIvParcialController : Controller // IV Parcial
    {
        private readonly DbimavaContext _context;

        public PersonalidadIvParcialController(DbimavaContext context) // IV Parcial
        {
            _context = context;
        }

        // GET: PersonalidadIiParcialController // IV Parcial
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

        [HttpGet]
        public async Task<JsonResult> GetPersonalidadesDelParcial(int idAño, string idGrado)
        {
            var query = _context.PersonalidadIvs
                .Where(m => m.Idaño == idAño);

            if (!string.IsNullOrEmpty(idGrado))
            {
                query = query.Where(m => m.Idgrado == idGrado);
            }

            // Obtén los resultados de la consulta
            var personalidades = await query
                .Select(m => new {
                    m.Idaño,
                    m.Idest,
                    m.Ididentidad,
                    nombreestudiante = m.NombreEstudiante,
                    m.Genero,
                    idpersonalidadiv = m.IdpersonalidadIv,
                    m.Puntualidad,
                    ordenypresentacion = m.OrdenYPresentacion,
                    m.Moralidad,
                    espiritudetrabajo = m.EspirituDeTrabajo,
                    m.Sociabilidad,
                    m.Inasistencias
                })
                .ToListAsync();

            // Verifica si no se encontraron resultados
            if (personalidades == null || !personalidades.Any())
            {
                return Json(new { success = false, message = "Ops, no hay datos de personalidad registrados en este Grado y IV Parcial." });
            }

            // Retorna los resultados si hay datos
            return Json(new { success = true, data = personalidades });
        }

        // DESCARGAR EXCEL DE PERSONALIDAD *****************************************************************
        //VERIFICACIÓN
        public IActionResult VerificarDatos(int? idAño, string idGrado)
        {
            if (idAño == null || string.IsNullOrEmpty(idGrado))
            {
                return Json(new { existeDatos = false });
            }

            // Comprobamos si existen datos para ese año y grado
            var datosFiltrados = _context.PersonalidadIvs // IV Parcial
                                          .Where(p => p.Idaño == idAño && p.Idgrado == idGrado)
                                          .ToList();

            // Si hay datos, devolvemos true, de lo contrario, false
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
                // Obtener los datos filtrados
                var datosFiltrados = _context.PersonalidadIvs // IV Parcial
                                              .Where(p => p.Idaño == idAño && p.Idgrado == idGrado)
                                              .ToList();

                // Crear un paquete de Excel
                using (var package = new ExcelPackage())
                {
                    // Crear una hoja de trabajo
                    var worksheet = package.Workbook.Worksheets.Add("Personalidad");

                    // Establecer la fuente del encabezado principal (A1 a R1 combinadas y centradas)
                    var headerCell = worksheet.Cells["A1:R1"];
                    headerCell.Merge = true;
                    headerCell.Value = "PERSONALIDAD ESTUDIANTIL - IV PARCIAL";
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Font.Size = 26;
                    headerCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    // Añadir "CLASIFICACIÓN" en la fila 2, celdas P2 a R2
                    var classificationHeader = worksheet.Cells["P2:R2"];
                    classificationHeader.Merge = true;
                    classificationHeader.Value = "CLASIFICACIÓN";
                    classificationHeader.Style.Font.Bold = true;
                    classificationHeader.Style.Font.Color.SetColor(System.Drawing.Color.White); // Texto blanco
                    classificationHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    classificationHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue); // Fondo azul oscuro
                    classificationHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    classificationHeader.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    classificationHeader.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Bordes

                    // Añadir "SOBRESALIENTE", "MUY BUENO", "BUENO", "DEBE MEJORAR" en las filas 3 a 6, celdas P3 a R6
                    var classificationValues = new string[] { "SOBRESALIENTE", "MUY BUENO", "BUENO", "DEBE MEJORAR" };
                    for (int i = 0; i < classificationValues.Length; i++)
                    {
                        var cellRange = worksheet.Cells[$"P{i + 3}:R{i + 3}"];
                        cellRange.Merge = true;
                        cellRange.Value = classificationValues[i];
                        cellRange.Style.Font.Bold = true;
                        cellRange.Style.Font.Color.SetColor(System.Drawing.Color.Black); // Texto negro
                        cellRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cellRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White); // Fondo blanco
                        cellRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        cellRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cellRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Bordes
                    }

                    // Añadir texto "EDITANDO" en A2 a C2 con fondo azul y letra blanca
                    var editandoCell = worksheet.Cells["A2:C2"];
                    editandoCell.Merge = true;
                    editandoCell.Value = "EDITANDO";
                    editandoCell.Style.Font.Bold = true; // Negrita
                    editandoCell.Style.Font.Color.SetColor(System.Drawing.Color.White); // Texto blanco
                    editandoCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Fondo sólido
                    editandoCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue); // Fondo azul oscuro (como la fila 7)
                    editandoCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    editandoCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    editandoCell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Bordes

                    // Agregar los encabezados de las columnas en la fila 7 (de A7 a R7)
                    worksheet.Cells[7, 1].Value = "Año";
                    worksheet.Cells[7, 2].Value = "ID Estudiante";
                    worksheet.Cells[7, 3].Value = "ID Identidad";
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
                    worksheet.Cells[7, 16].Value = "Sistema";
                    worksheet.Cells[7, 17].Value = "Sistema Clase";
                    worksheet.Cells[7, 18].Value = "Sistema Tiempo";

                    // Aplicar formato a las celdas de la fila 7 (negrita, color blanco, fondo azul oscuro, y alineación al centro)
                    for (int i = 1; i <= 18; i++)
                    {
                        var headerCellRow7 = worksheet.Cells[7, i];
                        headerCellRow7.Style.Font.Bold = true;
                        headerCellRow7.Style.Font.Color.SetColor(System.Drawing.Color.White); // Texto blanco
                        headerCellRow7.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerCellRow7.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue); // Fondo azul oscuro
                        headerCellRow7.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        headerCellRow7.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        headerCellRow7.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Bordes
                    }

                    // Llenar las filas con los datos a partir de la fila 8
                    for (int i = 0; i < datosFiltrados.Count; i++)
                    {
                        var personalidad = datosFiltrados[i];
                        worksheet.Cells[i + 8, 1].Value = personalidad.Idaño;
                        worksheet.Cells[i + 8, 2].Value = personalidad.Idest;
                        worksheet.Cells[i + 8, 3].Value = personalidad.Ididentidad;
                        worksheet.Cells[i + 8, 4].Value = personalidad.NombreEstudiante;
                        worksheet.Cells[i + 8, 5].Value = personalidad.Genero;
                        worksheet.Cells[i + 8, 6].Value = personalidad.Idgrado;
                        worksheet.Cells[i + 8, 7].Value = personalidad.Seccion;
                        worksheet.Cells[i + 8, 8].Value = personalidad.Jornada;
                        worksheet.Cells[i + 8, 9].Value = personalidad.IdpersonalidadIv; // IV Parcial ID
                        worksheet.Cells[i + 8, 10].Value = personalidad.Puntualidad;
                        worksheet.Cells[i + 8, 11].Value = personalidad.OrdenYPresentacion;
                        worksheet.Cells[i + 8, 12].Value = personalidad.Moralidad;
                        worksheet.Cells[i + 8, 13].Value = personalidad.EspirituDeTrabajo;
                        worksheet.Cells[i + 8, 14].Value = personalidad.Sociabilidad;
                        worksheet.Cells[i + 8, 15].Value = personalidad.Inasistencias;
                        worksheet.Cells[i + 8, 16].Value = personalidad.Sistema;
                        worksheet.Cells[i + 8, 17].Value = personalidad.SistemaClase;
                        worksheet.Cells[i + 8, 18].Value = personalidad.SistemaTiempo;
                    }

                    // Ajustar el ancho de las columnas al contenido
                    for (int i = 1; i <= 18; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    // Bloquear todas las celdas por defecto
                    worksheet.Cells.Style.Locked = true;

                    // Desbloquear las celdas editables para todas las filas de datos
                    for (int i = 8; i < 8 + datosFiltrados.Count; i++)
                    {
                        worksheet.Cells[i, 10].Style.Locked = false; // Puntualidad
                        worksheet.Cells[i, 11].Style.Locked = false; // Orden y Presentación
                        worksheet.Cells[i, 12].Style.Locked = false; // Moralidad
                        worksheet.Cells[i, 13].Style.Locked = false; // Espíritu de Trabajo
                        worksheet.Cells[i, 14].Style.Locked = false; // Sociabilidad
                        worksheet.Cells[i, 15].Style.Locked = false; // Inasistencias
                    }

                    // Proteger la hoja con una contraseña opcional (por ejemplo, "1234")
                    worksheet.Protection.SetPassword("1234");
                    worksheet.Protection.IsProtected = true;
                    worksheet.Protection.AllowSelectLockedCells = false;
                    worksheet.Protection.AllowSelectUnlockedCells = true;

                    // Convertir el paquete de Excel a un byte array
                    var excelFile = package.GetAsByteArray();

                    // Crear un nombre de archivo dinámico usando idAño y idGrado
                    var fileName = $"Personalidad_{idAño}_{idGrado}_IV.xlsx"; // Nombre dinámico

                    // Devolver el archivo como una descarga
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
                // Obtener los datos filtrados para IV Parcial (cambiamos 'Parcial' por 'Parcial')
                var datosFiltrados = _context.PersonalidadIvs //IVParcial
                                              .Where(p => p.Idaño == idAño && p.Idgrado == idGrado && p.IdpersonalidadIv != null) //II Parcial ID
                                              .ToList();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Personalidad IV Parcial");

                    // Mostrar las líneas de cuadrícula
                    worksheet.View.ShowGridLines = true;

                    // Establecer la fuente del encabezado principal (A1 a R1 combinadas y centradas)
                    var headerCell = worksheet.Cells["A1:R1"];
                    headerCell.Merge = true;
                    headerCell.Value = "PERSONALIDAD ESTUDIANTIL - IV PARCIAL";
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Font.Size = 26;
                    headerCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    // Añadir "CLASIFICACIÓN" en la fila 2, celdas P2 a R2
                    var classificationHeader = worksheet.Cells["P2:R2"];
                    classificationHeader.Merge = true;
                    classificationHeader.Value = "CLASIFICACIÓN";
                    classificationHeader.Style.Font.Bold = true;
                    classificationHeader.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    classificationHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    classificationHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    classificationHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    classificationHeader.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    classificationHeader.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    // Añadir clasificaciones
                    var classificationValues = new string[] { "SOBRESALIENTE", "MUY BUENO", "BUENO", "DEBE MEJORAR" };
                    for (int i = 0; i < classificationValues.Length; i++)
                    {
                        var cellRange = worksheet.Cells[$"P{i + 3}:R{i + 3}"];
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

                    // Añadir "NUEVOINGRESO"
                    var nuevoIngresoCell = worksheet.Cells["A2:C2"];
                    nuevoIngresoCell.Merge = true;
                    nuevoIngresoCell.Value = "NUEVOINGRESO";
                    nuevoIngresoCell.Style.Font.Bold = true;
                    nuevoIngresoCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    nuevoIngresoCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    nuevoIngresoCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    nuevoIngresoCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    nuevoIngresoCell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    // Agregar encabezados de columnas
                    string[] headers = new string[] {
                "Año", "ID Estudiante", "ID Identidad", "Nombre Estudiante", "Género",
                "Grado", "Sección", "Jornada", "ID Personalidad", "Puntualidad",
                "Orden y Presentación", "Moralidad", "Espíritu de Trabajo", "Sociabilidad",
                "Inasistencias", "Sistema", "Sistema Clase", "Sistema Tiempo"
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
                    // Llenar datos
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
                            // Llenar datos
                            worksheet.Cells[row, 1].Value = idAño;
                            worksheet.Cells[row, 2].Value = estudiante.Idest;
                            worksheet.Cells[row, 3].Value = estudiante.Ididentidad;
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
                            worksheet.Cells[row, 16].Value = "";
                            worksheet.Cells[row, 17].Value = "";
                            worksheet.Cells[row, 18].Value = "";

                            // Aplicar formato a la fila
                            var rowRange = worksheet.Cells[row, 1, row, 18];
                            rowRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            rowRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            rowRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            // Color de fondo alternado
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
                            var rowRange = worksheet.Cells[row, 1, row, 18];
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
                            worksheet.Cells[row, 3].Value = dato.Ididentidad;
                            worksheet.Cells[row, 4].Value = dato.NombreEstudiante;
                            worksheet.Cells[row, 5].Value = dato.Genero;
                            worksheet.Cells[row, 6].Value = dato.Idgrado;
                            worksheet.Cells[row, 7].Value = dato.Seccion;
                            worksheet.Cells[row, 8].Value = dato.Jornada;
                            worksheet.Cells[row, 9].Value = dato.IdpersonalidadIv; // IV Parcial ID
                            worksheet.Cells[row, 10].Value = dato.Puntualidad;
                            worksheet.Cells[row, 11].Value = dato.OrdenYPresentacion;
                            worksheet.Cells[row, 12].Value = dato.Moralidad;
                            worksheet.Cells[row, 13].Value = dato.EspirituDeTrabajo;
                            worksheet.Cells[row, 14].Value = dato.Sociabilidad;
                            worksheet.Cells[row, 15].Value = dato.Inasistencias;
                            worksheet.Cells[row, 16].Value = dato.Sistema;
                            worksheet.Cells[row, 17].Value = dato.SistemaClase;
                            worksheet.Cells[row, 18].Value = dato.SistemaTiempo;

                            row++;
                            lastRow = row - 1;
                        }
                    }

                    // Ajustar columnas y formato general
                    for (int i = 1; i <= 18; i++)
                    {
                        worksheet.Column(i).AutoFit();
                        if (worksheet.Column(i).Width < 8)
                        {
                            worksheet.Column(i).Width = 8;
                        }
                    }

                    // Aplicar bordes a toda la tabla
                    var dataRange = worksheet.Cells[8, 1, lastRow, 18];
                    dataRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                    // Desbloquear y formatear celdas editables
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

                    // Proteger la hoja
                    worksheet.Protection.IsProtected = true;
                    worksheet.Protection.AllowSelectLockedCells = false;
                    worksheet.Protection.AllowSelectUnlockedCells = true;

                    // Configurar vista
                    worksheet.View.FreezePanes(8, 1);

                    var fileName = $"Personalidad_IV_{idAño}_{idGrado}.xlsx";
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

                    // Leer el tipo de operación (EDITANDO o NUEVOINGRESO) desde A2
                    string tipoIngreso = worksheet.Cells["A2"].Text?.ToUpper();

                    if (tipoIngreso != "NUEVOINGRESO" && tipoIngreso != "EDITANDO")
                    {
                        return Json(new { error = "El archivo debe contener un valor válido (NUEVOINGRESO o EDITANDO) en la celda A2." });
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    List<PersonalidadIv> personalidades = new List<PersonalidadIv>(); //IV Parcial

                    for (int row = 8; row <= rowCount; row++) // Comienza en la fila 8 (por la cabecera)
                    {
                        // Leer los datos de cada fila
                        var personalidad = new PersonalidadIv //IV Parcial
                        {
                            Idaño = int.TryParse(worksheet.Cells[row, 1].Text, out var año) ? año : (int?)null,
                            Idest = int.TryParse(worksheet.Cells[row, 2].Text, out var idEst) ? idEst : (int?)null,
                            Ididentidad = worksheet.Cells[row, 3].Text,
                            NombreEstudiante = worksheet.Cells[row, 4].Text,
                            Genero = worksheet.Cells[row, 5].Text,
                            Idgrado = worksheet.Cells[row, 6].Text,
                            Grado = "", // Grado se insertará en blanco
                            Seccion = worksheet.Cells[row, 7].Text,
                            Jornada = worksheet.Cells[row, 8].Text,
                            IdpersonalidadIv = int.TryParse(worksheet.Cells[row, 9].Text, out var idPersonalidad) ? idPersonalidad : 0, //IVParcial
                            Puntualidad = string.IsNullOrEmpty(worksheet.Cells[row, 10].Text) ? null : worksheet.Cells[row, 10].Text,
                            OrdenYPresentacion = string.IsNullOrEmpty(worksheet.Cells[row, 11].Text) ? null : worksheet.Cells[row, 11].Text,
                            Moralidad = string.IsNullOrEmpty(worksheet.Cells[row, 12].Text) ? null : worksheet.Cells[row, 12].Text,
                            EspirituDeTrabajo = string.IsNullOrEmpty(worksheet.Cells[row, 13].Text) ? null : worksheet.Cells[row, 13].Text,
                            Sociabilidad = string.IsNullOrEmpty(worksheet.Cells[row, 14].Text) ? null : worksheet.Cells[row, 14].Text,  // Columna correcta
                            Inasistencias = int.TryParse(worksheet.Cells[row, 15].Text, out var inasistencias) ? inasistencias : (int?)null,  // Columna correcta
                            Sistema = worksheet.Cells[row, 16].Text,
                            SistemaClase = worksheet.Cells[row, 17].Text,
                            SistemaTiempo = worksheet.Cells[row, 18].Text
                        };

                        if (tipoIngreso == "NUEVOINGRESO")
                        {
                            // Verificar si ya existe un registro con el mismo IdEst y Año
                            var existingRecord = _context.PersonalidadIvs //IVParcial
                                .FirstOrDefault(p => p.Idest == personalidad.Idest && p.Idaño == personalidad.Idaño);

                            if (existingRecord != null)
                            {
                                return Json(new { error = "Ya existe un registro con el mismo ID Estudiante y Año. No se puede subir nuevamente." });
                            }

                            // Insertar nuevo registro
                            _context.PersonalidadIvs.Add(personalidad); //IVParcial
                        }
                        else if (tipoIngreso == "EDITANDO")
                        {
                            // Buscar y actualizar el registro existente
                            var personalidadExistente = _context.PersonalidadIvs //IVParcial
                                .FirstOrDefault(p => p.Idest == personalidad.Idest && p.Idaño == personalidad.Idaño && p.IdpersonalidadIv == personalidad.IdpersonalidadIv); //IIParcial ID

                            if (personalidadExistente != null)
                            {
                                personalidadExistente.Puntualidad = personalidad.Puntualidad;
                                personalidadExistente.OrdenYPresentacion = personalidad.OrdenYPresentacion;
                                personalidadExistente.Moralidad = personalidad.Moralidad;
                                personalidadExistente.EspirituDeTrabajo = personalidad.EspirituDeTrabajo;
                                personalidadExistente.Sociabilidad = personalidad.Sociabilidad;
                                personalidadExistente.Inasistencias = personalidad.Inasistencias;
                                personalidadExistente.Sistema = personalidad.Sistema;
                                personalidadExistente.SistemaClase = personalidad.SistemaClase;
                                personalidadExistente.SistemaTiempo = personalidad.SistemaTiempo;

                                _context.PersonalidadIvs.Update(personalidadExistente); //IIParcial
                            }
                            else
                            {
                                return Json(new { error = $"No se encontró el registro para actualizar (IdEst: {personalidad.Idest}, Año: {personalidad.Idaño})." });
                            }
                        }
                    }

                    // Guardar cambios en la base de datos
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