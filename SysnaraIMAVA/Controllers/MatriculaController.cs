using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysnaraIMAVA.Models;

namespace SysnaraIMAVA.Controllers
{
    public class MatriculaController : Controller
    {
        private readonly DbimavaContext _context;

        public MatriculaController(DbimavaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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
                    m.Nacionalidad,
                    m.NombreEstudiante,
                    m.FechaNacimiento,
                    m.Genero,
                    m.Estado,
                    m.Grado,
                    m.EstadoIngreso,
                    m.Sistema // Añadimos el campo Sistema
                })
                .OrderBy(m => m.Genero) // Ordenar primero por género (niñas primero)
                .ThenBy(m => m.NombreEstudiante) // Luego por nombre (A-Z)
                .ToListAsync();

            return Json(matriculas);
        }

        // GET: Matricula/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.IdañoNavigation)
                .Include(m => m.IdencargadoNavigation)
                .Include(m => m.IdgradoNavigation)
                .FirstOrDefaultAsync(m => m.Idest == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }
        //**************************************************************** fin
        // GET: Matricula/Create
        public IActionResult Create()
        {
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño");
            ViewData["Idencargado"] = new SelectList(_context.Padres, "Idencargado", "Idencargado");
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado");
            return View();
        }
        //************************************************************************************ INICIO
        //GET : CREATE VALIDAR QUE LA IDENTIDAD DEL ESTUDIANTE NO SE REPITA 
        public IActionResult BuscarEstudiante(string ididentidad)
        {
            var estudiante = _context.Matriculas
                .Where(m => m.Ididentidad == ididentidad)
                .OrderByDescending(m => m.Idaño)
                .FirstOrDefault();

            if (estudiante != null)
            {
                var padre = _context.Padres.FirstOrDefault(p => p.Idpadre == estudiante.Idpadre);

                return Json(new
                {
                    // Datos del estudiante
                    nombreEstudiante = estudiante.NombreEstudiante,
                    nacionalidad = estudiante.Nacionalidad,
                    fechaNacimiento = estudiante.FechaNacimiento.ToString("yyyy-MM-dd"),
                    lugarNacimiento = estudiante.LugarNacimiento,
                    genero = estudiante.Genero,
                    tipoSangre = estudiante.TipoSangre,
                    alergia = estudiante.Alergia,
                    edad = estudiante.Edad,
                    telefonoEstudiante = estudiante.TelefonoEstudiante,
                    celularEstudiante = estudiante.CelularEstudiante,
                    correo = estudiante.Correo,
                    direccion = estudiante.Direccion,
                    estado = estudiante.Estado,
                    vacunasCovid = estudiante.VacunasCovid,

                    // Datos académicos
                    //idgrado = estudiante.Idgrado,
                    //grado = estudiante.Grado,
                    //seccion = estudiante.Seccion,
                    //jornada = estudiante.Jornada,
                    //nivelDescripcion = estudiante.NivelDescripcion,
                    proviene = estudiante.Proviene,
                    beca = estudiante.Beca,
                    repiteAño = estudiante.RepiteAño,
                    observacion = estudiante.Observacion,
                    estadoIngreso = estudiante.EstadoIngreso,
                    fechaIngreso = estudiante.FechaIngreso?.ToString("yyyy-MM-dd"),
                    sistema = estudiante.Sistema,
                    //sistemaTiempo = estudiante.SistemaTiempo,

                    // Datos del padre
                    idpadre = padre?.Idpadre,
                    nombrePadre = padre?.NombrePadre,
                    parentesco = padre?.Parentesco,
                    telefonoPadre = padre?.TelefonoPadre,
                    celPadre = padre?.CelPadre,
                    direccionPadre = padre?.DireccionPadre,

                    // Indicador de que el estudiante ya está registrado
                    estudianteRegistrado = true
                });
            }

            return Json(new { estudianteRegistrado = false });
        }
        //************************************************************************************ FIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Matricula matricula, IFormFile fotoInput)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Procesar la foto si se ha cargado
                    if (fotoInput != null && fotoInput.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await fotoInput.CopyToAsync(memoryStream);
                            matricula.FotoData = memoryStream.ToArray();
                        }
                    }

                    _context.Add(matricula);
                    await _context.SaveChangesAsync();

                    // Establecer el mensaje de éxito
                    //TempData["SuccessMessage"] = $"El estudiante {matricula.NombreEstudiante} ha sido matriculado en el grado: {matricula.Grado}, periodo escolar: {matricula.SistemaTiempo}.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Manejar errores
                    ModelState.AddModelError("", "Ocurrió un error al guardar la matrícula.");
                }
            }

            // Si el modelo no es válido, recargar la vista con los datos ingresados
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", matricula.Idaño);
            ViewData["Idencargado"] = new SelectList(_context.Padres, "Idencargado", "Idencargado", matricula.Idencargado);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", matricula.Idgrado);
            return View(matricula);
        }

        //******************************************************************************
        //GET: Matricula / Datos del Padre AUTORELLENAR (CREATE)
        [HttpGet]
        public IActionResult GetPadreInfo(string idpadre)
        {
            var padre = _context.Padres.FirstOrDefault(p => p.Idpadre == idpadre);

            if (padre != null)
            {
                return Json(new
                {
                    success = true,
                    nombrePadre = padre.NombrePadre,
                    parentesco = padre.Parentesco,
                    telefonoPadre = padre.TelefonoPadre,
                    celPadre = padre.CelPadre,
                    direccionPadre = padre.DireccionPadre
                });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        //GET: Matricula / AUTORELLENAR (CREATE) Cuando seleccione el año, se generen los IDGRADOS en el select.
        [HttpGet]
        public IActionResult GetGradosPorAnio(int idAnio)
        {
            var grados = _context.Grados
                .Where(g => g.Idaño == idAnio)
                .Select(g => new { g.Idgrado })
                .ToList();
            return Json(grados);
        }

        //GET MATRICULA / AUTORELLENAR (CREATE) Cuando seleccione el IDGRADO que se autorellenen los demas datos
        [HttpGet]
        public IActionResult GetDetallesGrado(string idGrado)
        {
            var detallesGrado = _context.Grados
                .Where(g => g.Idgrado == idGrado)
                .Select(g => new
                {
                    g.Idgrado,
                    g.Grado1,
                    g.Seccion,
                    g.Jornada,
                    g.NivelDescripcion
                })
                .FirstOrDefault();

            return Json(detallesGrado);
        }


        // GET: Matricula/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
            {
                return NotFound();
            }
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", matricula.Idaño);
            ViewData["Idencargado"] = new SelectList(_context.Padres, "Idencargado", "Idencargado", matricula.Idencargado);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", matricula.Idgrado);
            return View(matricula);
        }

        // POST: Matricula/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Idaño,Idencargado,Idpadre,NombrePadre,Parentesco,Idest,Idsi,Ididentidad,Nacionalidad,NombreEstudiante,FechaNacimiento,LugarNacimiento,Genero,TipoSangre,Alergia,Edad,TelefonoEstudiante,CelularEstudiante,Correo,Direccion,Estado,Idgrado,Grado,Seccion,Jornada,Proviene,Beca,RepiteAño,Observacion,NivelDescripcion,TelefonoPadre,CelPadre,DireccionPadre,VacunasCovid,EstadoIngreso,FechaIngreso,Sistema,SistemaClase,SistemaTiempo")] 
            Matricula matricula,
            IFormFile fotoInput) // Parámetro para manejar la foto
        {
            if (id != matricula.Idest)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Procesar la foto si se ha cargado
                    if (fotoInput != null && fotoInput.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await fotoInput.CopyToAsync(memoryStream); // Convertir la foto a bytes
                            matricula.FotoData = memoryStream.ToArray(); // Asignar los bytes a FotoData
                        }
                    }
                    else
                    {
                        // Si no se carga una nueva foto, mantener la foto existente
                        var existingMatricula = await _context.Matriculas.AsNoTracking().FirstOrDefaultAsync(m => m.Idest == id);
                        if (existingMatricula != null)
                        {
                            matricula.FotoData = existingMatricula.FotoData;
                        }
                    }

                    _context.Update(matricula);
                    await _context.SaveChangesAsync();

                    // Mensaje de confirmación con SweetAlert2
                    TempData["SuccessMessage"] = $"Los datos del estudiante {matricula.NombreEstudiante} han sido actualizados correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatriculaExists(matricula.Idest))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Si el modelo no es válido, recargar la vista con los datos ingresados
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", matricula.Idaño);
            ViewData["Idencargado"] = new SelectList(_context.Padres, "Idencargado", "Idencargado", matricula.Idencargado);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", matricula.Idgrado);
            return View(matricula);
        }

        // GET: Matricula/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.IdañoNavigation)
                .Include(m => m.IdencargadoNavigation)
                .Include(m => m.IdgradoNavigation)
                .FirstOrDefaultAsync(m => m.Idest == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // POST: Matricula/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula != null)
            {
                _context.Matriculas.Remove(matricula);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatriculaExists(int id)
        {
            return _context.Matriculas.Any(e => e.Idest == id);
        }

        public IActionResult MostrarFoto(int id)
        {
            var matricula = _context.Matriculas.FirstOrDefault(m => m.Idest == id);
            if (matricula?.FotoData != null)
            {
                return File(matricula.FotoData, "image/jpeg"); // Ajusta el tipo MIME según el formato de la imagen
            }
            return NotFound(); // Si no hay foto, devuelve un 404
        }
    }
}
