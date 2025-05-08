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
using System.Drawing;

namespace SysnaraIMAVA.Controllers
{
    public class NotasController : Controller
    {
        private readonly DbimavaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public NotasController(DbimavaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Inyectamos IWebHostEnvironment

            // Configurar el contexto de licencia de EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Cambiar a Commercial si aplica

        }

        // GET: NotasController
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
        // Nuevo método para obtener el IDASIGNATURA y las ASIGNATURAS***************** inicio
        [HttpGet]
        public async Task<JsonResult> GetMatriculas(int idAño, string idGrado)
        {
            // Obtiene todas las asignaturas para el grado seleccionado
            var asignaturas = await _context.Asignaturas
                .Where(a => a.Idgrado == idGrado)
                .Select(a => new
                {
                    a.Idasignatura,
                    a.Asignatura1
                })
                .ToListAsync();

            // Obtiene las notas para las asignaturas del año y grado seleccionados
            var notas = await _context.Notas
                .Where(n => n.Idaño == idAño && n.Idgrado == idGrado)
                .Select(n => new
                {
                    n.Idasignatura,
                    n.PromedioFinal,
                    n.Idnota,
                    n.Grado // Incluimos el nombre del grado
                })
                .ToListAsync();

            // Obtén el nombre del grado (asumiendo que todas las notas tienen el mismo grado)
            var nombreGrado = notas.FirstOrDefault()?.Grado ?? "Grado no encontrado";

            // Combina las asignaturas con las notas para que todas las asignaturas aparezcan
            var resultado = asignaturas.Select(a => new
            {
                idaño = idAño,
                idgrado = idGrado,
                grado = nombreGrado, // Usamos el nombre del grado obtenido
                idasignatura = a.Idasignatura,
                asignatura = a.Asignatura1,
                estado = notas.Any(n => n.Idasignatura == a.Idasignatura && n.Idnota != null) // Comprobamos si existe idnota
            }).ToList();

            return Json(resultado);
        }

        //***************************************************************Fin
        //VER NOTAS **************** INICIO
        public IActionResult GetNotas(int idAño, string idAsignatura, string asignatura)
        {
            var notas = _context.Notas
                .Where(n => n.Idaño == idAño && n.Idasignatura == idAsignatura)
                .Select(n => new {
                    n.Idestudiante,
                    n.NombreEstudiante,
                    n.Idnota,
                    n.Iparcial,
                    n.Iiparcial,
                    n.Iiiparcial,
                    n.Ivparcial,
                    n.Isemestre,
                    n.Iisemestre,
                    n.PromedioFinal,
                    n.RecuperacionI,
                })
                .ToList();

            // Agrega un log para verificar la salida
            Console.WriteLine($"Notas encontradas: {notas.Count}");

            return Json(notas);
        }
        //***************************** FIN
        [HttpGet]
        public IActionResult DescargarExcel(string idasignatura)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // O LicenseContext.Commercial si aplica

            var notas = _context.Notas
                .Where(n => n.Idasignatura == idasignatura)
                .OrderBy(n => n.Genero) // Ordenar por género
                .ThenBy(n => n.NombreEstudiante) // Luego por nombre
                .ToList();

            // Obtener el nombre de la asignatura y otros datos únicos
            var asignatura = notas.FirstOrDefault()?.Asignatura;
            var idgrado = notas.FirstOrDefault()?.Idgrado;
            var grado = notas.FirstOrDefault()?.Grado;
            var seccion = notas.FirstOrDefault()?.Seccion;
            var jornada = notas.FirstOrDefault()?.Jornada;
            //var sistema = notas.FirstOrDefault()?.Sistema;
            //var sistemaClase = notas.FirstOrDefault()?.SistemaClase;
            //var sistemaTiempo = notas.FirstOrDefault()?.SistemaTiempo;
            var idaño = notas.FirstOrDefault()?.Idaño; // Agregar id año

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Notas");

            // Encabezados generales
            worksheet.Cells[1, 1].Value = "INFORMACIÓN GENERAL";
            worksheet.Cells[1, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[1, 1, 1, 9].Merge = true; // Combina columnas A a I de la fila 1
            worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Centrar texto
            worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Centrar verticalmente

            worksheet.Cells[2, 1].Value = "ID Año:";
            worksheet.Cells[2, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[2, 2].Value = idaño;

            worksheet.Cells[3, 1].Value = "ID Asignatura:";
            worksheet.Cells[3, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[3, 2].Value = idasignatura;

            worksheet.Cells[4, 1].Value = "Asignatura:";
            worksheet.Cells[4, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[4, 2].Value = asignatura;
            worksheet.Cells[4, 2, 4, 4].Merge = true; // Combina columnas B a D de la fila 4

            worksheet.Cells[5, 1].Value = "ID Grado:";
            worksheet.Cells[5, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[5, 2].Value = idgrado;

            worksheet.Cells[6, 1].Value = "Grado:";
            worksheet.Cells[6, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[6, 2].Value = grado;

            worksheet.Cells[7, 1].Value = "Sección:";
            worksheet.Cells[7, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[7, 2].Value = seccion;

            worksheet.Cells[8, 1].Value = "Jornada:";
            worksheet.Cells[8, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[8, 2].Value = jornada;

            //worksheet.Cells[9, 1].Value = "Sistema:";
            //worksheet.Cells[9, 1].Style.Font.Bold = true; // Negrita
            //worksheet.Cells[9, 2].Value = sistema;

            //worksheet.Cells[10, 1].Value = "Sistema Clase:";
            //worksheet.Cells[10, 1].Style.Font.Bold = true; // Negrita
            //worksheet.Cells[10, 2].Value = sistemaClase;

            //worksheet.Cells[11, 1].Value = "Sistema Tiempo:";
            //worksheet.Cells[11, 1].Style.Font.Bold = true; // Negrita
            //worksheet.Cells[11, 2].Value = sistemaTiempo;

            worksheet.Cells[12, 1].Value = "EDITANDO";
            worksheet.Cells[12, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[12, 1].Style.Font.Color.SetColor(System.Drawing.Color.White); // Texto blanco
            worksheet.Cells[12, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Define un color sólido
            worksheet.Cells[12, 1].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue); // Establece el color de fondo como azul

            // CUADRO DE NOTA / OBSERVACIÓN
            worksheet.Cells[3, 6, 9, 9].Merge = true; // F3 a I9
            worksheet.Cells[3, 6].Value = "NOTA: NO EDITAR EL FORMATO, MÁRGENES, INFORMACIÓN, LIMITARSE A SOLO AGREGAR NOTAS EN EL PARCIAL O SEMESTRE CORRESPONDIENTE PARA EVITAR ERRORES AL SUBIR LA INFORMACIÓN AL SISTEMA.";
            worksheet.Cells[3, 6].Style.WrapText = true; // Ajustar texto
            worksheet.Cells[3, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Alinear horizontalmente al centro
            worksheet.Cells[3, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Alinear verticalmente al centro
            worksheet.Cells[3, 6].Style.Font.Bold = true; // Texto en negrita

            // Espacio entre encabezados generales y datos
            worksheet.Cells[12, 6].Value = "I Semestre";
            worksheet.Cells[12, 6, 12, 7].Merge = true; // Combina celdas F12 y G12
            worksheet.Cells[12, 8].Value = "II Semestre";
            worksheet.Cells[12, 8, 12, 9].Merge = true; // Combina celdas H12 e I12

            worksheet.Cells[13, 1].Value = "Id Estudiante";
            worksheet.Cells[13, 2].Value = "ID Identidad";
            worksheet.Cells[13, 3].Value = "Nombre Estudiante";
            worksheet.Cells[13, 4].Value = "Género";
            worksheet.Cells[13, 5].Value = "ID Nota";
            worksheet.Cells[13, 6].Value = "I Parcial";
            worksheet.Cells[13, 7].Value = "II Parcial";
            worksheet.Cells[13, 8].Value = "III Parcial";
            worksheet.Cells[13, 9].Value = "IV Parcial";

            // Aplicar estilo a las celdas del encabezado
            for (int col = 1; col <= 9; col++)
            {
                worksheet.Cells[13, col].Style.Font.Bold = true; // Negrita
                worksheet.Cells[13, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Establecer patrón sólido
                worksheet.Cells[13, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue); // Fondo azul oscuro
                worksheet.Cells[13, col].Style.Font.Color.SetColor(System.Drawing.Color.White); // Texto blanco
                worksheet.Cells[13, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Centrar texto
                worksheet.Cells[13, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Centrar verticalmente
            }

            // Agregar datos
            for (int i = 0; i < notas.Count; i++)
            {
                var nota = notas[i];
                worksheet.Cells[i + 14, 1].Value = nota.Idest;
                worksheet.Cells[i + 14, 2].Value = nota.Idestudiante;
                worksheet.Cells[i + 14, 3].Value = nota.NombreEstudiante;
                worksheet.Cells[i + 14, 4].Value = nota.Genero;
                worksheet.Cells[i + 14, 5].Value = nota.Idnota;
                worksheet.Cells[i + 14, 6].Value = nota.Iparcial;
                worksheet.Cells[i + 14, 7].Value = nota.Iiparcial;
                worksheet.Cells[i + 14, 8].Value = nota.Iiiparcial;
                worksheet.Cells[i + 14, 9].Value = nota.Ivparcial;
            }

            // Ajustar el tamaño de las columnas
            worksheet.Cells.AutoFitColumns();

            // Bloquear todas las celdas (por defecto todas están bloqueadas)
            worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Locked = true;

            // Desbloquear las celdas de los parciales (F14 a I de cada fila)
            for (int row = 14; row <= notas.Count + 13; row++)
            {
                worksheet.Cells[row, 6].Style.Locked = false; // I Parcial
                worksheet.Cells[row, 7].Style.Locked = false; // II Parcial
                worksheet.Cells[row, 8].Style.Locked = false; // III Parcial
                worksheet.Cells[row, 9].Style.Locked = false; // IV Parcial
            }

            // Proteger la hoja con una contraseña opcional
            worksheet.Protection.IsProtected = true;
            worksheet.Protection.AllowSelectLockedCells = true;  // Permitir seleccionar celdas bloqueadas
            worksheet.Protection.AllowEditObject = false;  // No permitir la edición de objetos (como gráficos)
            worksheet.Protection.AllowFormatCells = false; // No permitir cambiar el formato de las celdas

            // Crear el archivo Excel en memoria
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            // Retornar el archivo como descarga
            var fileName = $"Notas_{idasignatura}_{asignatura}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [HttpGet]
        public IActionResult DescargarExcelEnBlanco(string idasignatura)
        {
            // Obtener la asignatura a partir del idasignatura
            var asignatura = _context.Asignaturas
                .Where(a => a.Idasignatura == idasignatura)
                .FirstOrDefault();

            // Verificar que la asignatura exista
            if (asignatura == null)
            {
                return NotFound("Asignatura no encontrada.");
            }

            // Obtener los estudiantes matriculados en el grado correspondiente y ordenarlos por Género y Nombre
            var matriculas = _context.Matriculas
                .Where(m => m.Idgrado == asignatura.Idgrado)
                .OrderBy(m => m.Genero)         // Ordenar por Género
                .ThenBy(m => m.NombreEstudiante) // Luego por Nombre Estudiante
                .ToList();

            // Obtener los datos adicionales necesarios para el archivo
            var grado = _context.Grados
                .Where(g => g.Idgrado == asignatura.Idgrado)
                .FirstOrDefault()?.Grado1;

            var idaño = matriculas.FirstOrDefault()?.Idaño; // Supone que todos los estudiantes son del mismo año
            var seccion = asignatura.Seccion;
            var jornada = asignatura.Jornada;
            //var sistema = asignatura.Sistema;
            //var sistemaClase = asignatura.SistemaClase;
            //var sistemaTiempo = asignatura.SistemaTiempo;

            // Crear el archivo Excel
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Estudiantes");

            // Encabezados
            worksheet.Cells[1, 1].Value = "INFORMACIÓN GENERAL";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 9].Merge = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Información general del archivo
            worksheet.Cells[2, 1].Value = "ID Año:";
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 2].Value = idaño;

            worksheet.Cells[3, 1].Value = "ID Asignatura:";
            worksheet.Cells[3, 1].Style.Font.Bold = true;
            worksheet.Cells[3, 2].Value = idasignatura;

            worksheet.Cells[4, 1].Value = "Asignatura:";
            worksheet.Cells[4, 1].Style.Font.Bold = true;
            worksheet.Cells[4, 2].Value = asignatura.Asignatura1;
            worksheet.Cells[4, 2, 4, 4].Merge = true;

            worksheet.Cells[5, 1].Value = "ID Grado:";
            worksheet.Cells[5, 1].Style.Font.Bold = true;
            worksheet.Cells[5, 2].Value = asignatura.Idgrado;

            worksheet.Cells[6, 1].Value = "Grado:";
            worksheet.Cells[6, 1].Style.Font.Bold = true;
            worksheet.Cells[6, 2].Value = grado;

            worksheet.Cells[7, 1].Value = "Sección:";
            worksheet.Cells[7, 1].Style.Font.Bold = true;
            worksheet.Cells[7, 2].Value = seccion;

            worksheet.Cells[8, 1].Value = "Jornada:";
            worksheet.Cells[8, 1].Style.Font.Bold = true;
            worksheet.Cells[8, 2].Value = jornada;

            //worksheet.Cells[9, 1].Value = "Sistema:";
            //worksheet.Cells[9, 1].Style.Font.Bold = true;
            //worksheet.Cells[9, 2].Value = sistema;

            //worksheet.Cells[10, 1].Value = "Sistema Clase:";
            //worksheet.Cells[10, 1].Style.Font.Bold = true;
            //worksheet.Cells[10, 2].Value = sistemaClase;

            //worksheet.Cells[11, 1].Value = "Sistema Tiempo:";
            //worksheet.Cells[11, 1].Style.Font.Bold = true;
            //worksheet.Cells[11, 2].Value = sistemaTiempo;

            worksheet.Cells[12, 1].Value = "NUEVOINGRESO";
            worksheet.Cells[12, 1].Style.Font.Bold = true; // Negrita
            worksheet.Cells[12, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Define un color sólido
            worksheet.Cells[12, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen); // Establece el color de fondo como verde claro

            // Cuadro de nota/observación
            worksheet.Cells[3, 6, 9, 9].Merge = true;
            worksheet.Cells[3, 6].Value = "NOTA: NO EDITAR EL FORMATO, MÁRGENES, INFORMACIÓN, LIMITARSE A SOLO AGREGAR NOTAS EN EL PARCIAL O SEMESTRE CORRESPONDIENTE PARA EVITAR ERRORES AL SUBIR LA INFORMACIÓN AL SISTEMA.";
            worksheet.Cells[3, 6].Style.WrapText = true;
            worksheet.Cells[3, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[3, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Cells[3, 6].Style.Font.Bold = true;

            // Ajustar bordes
            var border = worksheet.Cells[3, 6, 9, 9].Style.Border;
            border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            // Espacio entre encabezados generales y datos
            worksheet.Cells[12, 6].Value = "I Semestre";
            worksheet.Cells[12, 6, 12, 7].Merge = true;

            worksheet.Cells[12, 8].Value = "II Semestre";
            worksheet.Cells[12, 8, 12, 9].Merge = true;

            // Encabezados para los estudiantes
            worksheet.Cells[13, 1].Value = "Id Estudiante";
            worksheet.Cells[13, 2].Value = "ID Identidad";
            worksheet.Cells[13, 3].Value = "Nombre Estudiante";
            worksheet.Cells[13, 4].Value = "Género";
            worksheet.Cells[13, 5].Value = "ID Nota";
            worksheet.Cells[13, 6].Value = "I Parcial";
            worksheet.Cells[13, 7].Value = "II Parcial";
            worksheet.Cells[13, 8].Value = "III Parcial";
            worksheet.Cells[13, 9].Value = "IV Parcial";

            // Aplicar estilo a las celdas del encabezado
            for (int col = 1; col <= 9; col++)
            {
                worksheet.Cells[13, col].Style.Font.Bold = true;
                worksheet.Cells[13, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[13, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                worksheet.Cells[13, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[13, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[13, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }

            // Agregar los estudiantes matriculados
            for (int i = 0; i < matriculas.Count; i++)
            {
                var matricula = matriculas[i];
                worksheet.Cells[i + 14, 1].Value = matricula.Idest;
                worksheet.Cells[i + 14, 2].Value = matricula.Idestudiante;
                worksheet.Cells[i + 14, 3].Value = matricula.NombreEstudiante;
                worksheet.Cells[i + 14, 4].Value = matricula.Genero;
                worksheet.Cells[i + 14, 5].Value = 0; // Sin nota
                worksheet.Cells[i + 14, 6].Value = 0; // I Parcial
                worksheet.Cells[i + 14, 7].Value = 0; // II Parcial
                worksheet.Cells[i + 14, 8].Value = 0; // III Parcial
                worksheet.Cells[i + 14, 9].Value = 0; // IV Parcial
            }

            // Ajustar el tamaño de las columnas
            worksheet.Cells.AutoFitColumns();

            // Bloquear todas las celdas (por defecto todas están bloqueadas)
            worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Locked = true;

            // Desbloquear las celdas de los parciales (F14 a I de cada fila)
            for (int row = 14; row <= matriculas.Count + 13; row++)
            {
                worksheet.Cells[row, 6].Style.Locked = false; // I Parcial
                worksheet.Cells[row, 7].Style.Locked = false; // II Parcial
                worksheet.Cells[row, 8].Style.Locked = false; // III Parcial
                worksheet.Cells[row, 9].Style.Locked = false; // IV Parcial
            }

            // Proteger la hoja con una contraseña opcional
            worksheet.Protection.IsProtected = true;
            worksheet.Protection.AllowSelectLockedCells = true;  // Permitir seleccionar celdas bloqueadas
            worksheet.Protection.AllowEditObject = false;  // No permitir la edición de objetos (como gráficos)
            worksheet.Protection.AllowFormatCells = false; // No permitir cambiar el formato de las celdas

            // Crear el archivo Excel en memoria
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            // Retornar el archivo como descarga
            var fileName = $"Notas_{idasignatura}_{asignatura.Asignatura1}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [HttpPost]
        public async Task<IActionResult> SubirNotas(IFormFile archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.Length == 0)
            {
                return BadRequest("No se ha proporcionado ningún archivo.");
            }

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    string tipoIngreso = worksheet.Cells[12, 1].Text?.Trim().ToUpper();

                    // Leer información general
                    var idAño = int.Parse(worksheet.Cells[2, 2].Value?.ToString() ?? "0");
                    var idAsignatura = worksheet.Cells[3, 2].Value?.ToString();
                    var asignatura = worksheet.Cells[4, 2].Value?.ToString();
                    var idGrado = worksheet.Cells[5, 2].Value?.ToString();
                    var grado = worksheet.Cells[6, 2].Value?.ToString();
                    var seccion = worksheet.Cells[7, 2].Value?.ToString();
                    var jornada = worksheet.Cells[8, 2].Value?.ToString();
                    var sistema = worksheet.Cells[9, 2].Value?.ToString();
                    var sistemaClase = worksheet.Cells[10, 2].Value?.ToString();
                    var sistemaTiempo = worksheet.Cells[11, 2].Value?.ToString();

                    try
                    {
                        // Primero, verificamos si ya existen registros para la asignatura en el año y grado
                        if (tipoIngreso == "NUEVOINGRESO")
                        {
                            var notasExistentes = await _context.Notas
                                .Where(n => n.Idasignatura == idAsignatura && n.Idaño == idAño && n.Idgrado == idGrado)
                                .AnyAsync();

                            if (notasExistentes)
                            {
                                return BadRequest("OOPS! Ya existen nuevos registros para esta asignatura. Por favor actualice la página.");
                            }
                        }

                        // En la sección de EDITANDO, reemplazar los cálculos por:
                        if (tipoIngreso == "EDITANDO")
                        {
                            for (int row = 14; row <= rowCount; row++)
                            {
                                var idNota = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0");
                                if (idNota > 0)
                                {
                                    var nota = await _context.Notas.FindAsync(idNota);
                                    if (nota != null)
                                    {
                                        // Leer los valores de los parciales
                                        var ip = int.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var tempIp) ? tempIp : (int?)null;
                                        var iip = int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out var tempIip) ? tempIip : (int?)null;
                                        var iiip = int.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var tempIiip) ? tempIiip : (int?)null;
                                        var ivp = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var tempIvp) ? tempIvp : (int?)null;

                                        // Actualizar los parciales
                                        nota.Iparcial = ip;
                                        nota.Iiparcial = iip;
                                        nota.Iiiparcial = iiip;
                                        nota.Ivparcial = ivp;

                                        // Cálculos de promedios usando decimal para mantener la precisión
                                        // Cálculo de ISemestre = (IParcial + IIParcial) / 2
                                        if (nota.Iparcial.HasValue && nota.Iiparcial.HasValue)
                                        {
                                            decimal promedio = (nota.Iparcial.Value + nota.Iiparcial.Value) / 2.0m;
                                            nota.Isemestre = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                        }

                                        // Cálculo de IISemestre = (IIIParcial + IVParcial) / 2
                                        if (nota.Iiiparcial.HasValue && nota.Ivparcial.HasValue)
                                        {
                                            decimal promedio = (nota.Iiiparcial.Value + nota.Ivparcial.Value) / 2.0m;
                                            nota.Iisemestre = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                        }

                                        // Cálculo de PromedioFinal = (ISemestre + IISemestre) / 2
                                        if (nota.Isemestre.HasValue && nota.Iisemestre.HasValue)
                                        {
                                            decimal promedio = (nota.Isemestre.Value + nota.Iisemestre.Value) / 2.0m;
                                            nota.PromedioFinal = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                        }

                                        // Cálculo de IndicePromocion = (IParcial + IIParcial + IIIParcial + IVParcial) / 4
                                        if (nota.Iparcial.HasValue && nota.Iiparcial.HasValue &&
                                            nota.Iiiparcial.HasValue && nota.Ivparcial.HasValue)
                                        {
                                            decimal promedio = (nota.Iparcial.Value + nota.Iiparcial.Value +
                                                              nota.Iiiparcial.Value + nota.Ivparcial.Value) / 4.0m;
                                            nota.IndicePromocion = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                        }

                                        _context.Notas.Update(nota);
                                    }
                                }
                            }
                        }
                        else if (tipoIngreso == "NUEVOINGRESO")
                        {
                            var ultimaIdNota = await _context.Notas
                                .MaxAsync(n => (int?)n.Idnota) ?? 0;

                            var nuevasNotas = new List<Nota>();

                            for (int row = 14; row <= rowCount; row++)
                            {
                                ultimaIdNota++;

                                var nuevaNota = new Nota
                                {
                                    Idaño = idAño,
                                    Idest = int.Parse(worksheet.Cells[row, 1].Value?.ToString() ?? "0"),
                                    Idestudiante = worksheet.Cells[row, 2].Value?.ToString(),
                                    NombreEstudiante = worksheet.Cells[row, 3].Value?.ToString(),
                                    Genero = worksheet.Cells[row, 4].Value?.ToString(),
                                    Idasignatura = idAsignatura,
                                    Asignatura = asignatura,
                                    Idgrado = idGrado,
                                    Grado = grado,
                                    Seccion = seccion,
                                    Jornada = jornada,
                                    Iparcial = int.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var ip) ? ip : (int?)null,
                                    Iiparcial = int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out var iip) ? iip : (int?)null,
                                    Iiiparcial = int.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var iiip) ? iiip : (int?)null,
                                    Ivparcial = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var ivp) ? ivp : (int?)null,
                                    //Sistema = sistema,
                                    //SistemaClase = sistemaClase,
                                    //SistemaTiempo = sistemaTiempo,
                                };

                                // Cálculos de promedios usando decimal
                                if (nuevaNota.Iparcial.HasValue && nuevaNota.Iiparcial.HasValue)
                                {
                                    decimal promedio = (nuevaNota.Iparcial.Value + nuevaNota.Iiparcial.Value) / 2.0m;
                                    nuevaNota.Isemestre = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                }

                                if (nuevaNota.Iiiparcial.HasValue && nuevaNota.Ivparcial.HasValue)
                                {
                                    decimal promedio = (nuevaNota.Iiiparcial.Value + nuevaNota.Ivparcial.Value) / 2.0m;
                                    nuevaNota.Iisemestre = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                }

                                if (nuevaNota.Isemestre.HasValue && nuevaNota.Iisemestre.HasValue)
                                {
                                    decimal promedio = (nuevaNota.Isemestre.Value + nuevaNota.Iisemestre.Value) / 2.0m;
                                    nuevaNota.PromedioFinal = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                }

                                if (nuevaNota.Iparcial.HasValue && nuevaNota.Iiparcial.HasValue &&
                                    nuevaNota.Iiiparcial.HasValue && nuevaNota.Ivparcial.HasValue)
                                {
                                    decimal promedio = (nuevaNota.Iparcial.Value + nuevaNota.Iiparcial.Value +
                                                      nuevaNota.Iiiparcial.Value + nuevaNota.Ivparcial.Value) / 4.0m;
                                    nuevaNota.IndicePromocion = (int)Math.Round(promedio, 0, MidpointRounding.AwayFromZero);
                                }

                                nuevasNotas.Add(nuevaNota);
                            }

                        if (nuevasNotas.Any())
                            {
                                await _context.Notas.AddRangeAsync(nuevasNotas);
                            }
                        }

                        await _context.SaveChangesAsync();
                        return Ok($"{tipoIngreso} - Notas procesadas correctamente.");
                    }
                    catch (Exception ex)
                    {
                        var mensaje = $"Error al procesar las notas: {ex.Message}";
                        if (ex.InnerException != null)
                        {
                            mensaje += $" Inner Exception: {ex.InnerException.Message}";
                        }
                        return StatusCode(500, mensaje);
                    }
                }
            }
        }



    }
}
