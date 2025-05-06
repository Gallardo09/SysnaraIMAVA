using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysnaraIMAVA.Models;
using System.Linq;
using System.Threading.Tasks;
using FastReport;
using FastReport.Export.PdfSimple;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace SysnaraIMAVA.Controllers
{
    public class CuadroHonorController : Controller
    {
        private readonly DbimavaContext _context;

        public CuadroHonorController(DbimavaContext context)
        {
            _context = context;
        }

        // GET: CuadroHonorController
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
                .Select(g => new { g.Idgrado, GradoCompleto = g.Grado1 + " - " + g.Seccion })
                .ToListAsync();

            return Json(grados);
        }

        [HttpGet]
        public async Task<JsonResult> GetCuadroHonor(int idAño, string idGrado, int parcial)
        {
            // Primero obtenemos todas las notas con información de asignatura
            var query = _context.Notas
                .Include(n => n.IdasignaturaNavigation) // Incluir información de asignatura
                .Where(n => n.Idaño == idAño);

            if (!string.IsNullOrEmpty(idGrado))
            {
                query = query.Where(n => n.Idgrado == idGrado);
            }

            // Obtenemos los datos agrupados por estudiante
            var datosEstudiantes = await query
                .GroupBy(n => new { n.Ididentidad, n.NombreEstudiante, n.Genero, n.Grado, n.Seccion, n.Jornada })
                .ToListAsync();

            // Procesamos cada estudiante para calcular promedios considerando asignaturas que no aplican
            var cuadroHonor = new List<dynamic>();

            foreach (var grupo in datosEstudiantes)
            {
                // Obtenemos todas las notas del estudiante
                var notasEstudiante = grupo.ToList();

                // Filtramos las asignaturas que no aplican según el género
                var notasValidas = notasEstudiante
                    .Where(n => {
                        var nombreAsignatura = n.IdasignaturaNavigation?.Asignatura1?.ToUpper() ?? "";

                        // Si es FEMENINO, excluir ARTES MANUALES - MANUAL ARTS
                        if (grupo.Key.Genero?.ToUpper() == "FEMENINO" &&
                            (nombreAsignatura.Contains("ARTES MANUALES") || nombreAsignatura.Contains("MANUAL ARTS")))
                        {
                            return false;
                        }

                        // Si es MASCULINO, excluir EDUCACIÓN PARA EL HOGAR - HOME ECONOMICS
                        if (grupo.Key.Genero?.ToUpper() == "MASCULINO" &&
                            (nombreAsignatura.Contains("EDUCACIÓN PARA EL HOGAR") || nombreAsignatura.Contains("HOME ECONOMICS")))
                        {
                            return false;
                        }

                        return true;
                    })
                    .ToList();

                // Calculamos promedios solo con las notas válidas
                var cantidadAsignaturasValidas = notasValidas.Count;
                var promedioIParcial = cantidadAsignaturasValidas > 0 ? notasValidas.Average(n => n.Iparcial) : 0;
                var promedioIIParcial = cantidadAsignaturasValidas > 0 ? notasValidas.Average(n => n.Iiparcial) : 0;
                var promedioIIIParcial = cantidadAsignaturasValidas > 0 ? notasValidas.Average(n => n.Iiiparcial) : 0;
                var promedioIVParcial = cantidadAsignaturasValidas > 0 ? notasValidas.Average(n => n.Ivparcial) : 0;
                var promedioFinal = cantidadAsignaturasValidas > 0 ? notasValidas.Average(n => n.PromedioFinal) : 0;

                var item = new
                {
                    grupo.Key.Ididentidad,
                    grupo.Key.NombreEstudiante,
                    grupo.Key.Genero,
                    grupo.Key.Grado,
                    grupo.Key.Seccion,
                    grupo.Key.Jornada,
                    CantidadAsignaturas = cantidadAsignaturasValidas,
                    TotalAsignaturas = notasEstudiante.Count, // Total de asignaturas registradas
                    Promedio = parcial switch
                    {
                        1 => promedioIParcial,
                        2 => promedioIIParcial,
                        3 => promedioIIIParcial,
                        4 => promedioIVParcial,
                        _ => promedioFinal
                    }
                };

                // Log para depuración
                Console.WriteLine($"Estudiante: {grupo.Key.NombreEstudiante}, " +
                                 $"Género: {grupo.Key.Genero}, " +
                                 $"Asignaturas válidas: {cantidadAsignaturasValidas}, " +
                                 $"Total asignaturas: {notasEstudiante.Count}");

                cuadroHonor.Add(item);
            }

            // Ordenamos por promedio descendente
            var resultado = cuadroHonor
                .OrderByDescending(x => x.Promedio)
                .ToList();

            return Json(resultado);
        }
    }
}