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
                    m.Idimv,
                    m.Idestudiante,
                    m.Nacionalidad,
                    m.NombreEstudiante,
                    m.FechaNacimiento,
                    m.Genero,
                    m.Estado,
                    m.Grado,
                    estadoIngreso = m.EstadoMatricula, // Cambiado de EstadoMatricula a estadoIngreso
                })
                .OrderBy(m => m.Genero)
                .ThenBy(m => m.NombreEstudiante)
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
                .Include(m => m.IdimvencargadoNavigation)
                .Include(m => m.IdgradoNavigation)
                .FirstOrDefaultAsync(m => m.Idest == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // GET: Matricula/Create
        public IActionResult Create()
        {
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño");
            ViewData["Idimvencargado"] = new SelectList(_context.Padres, "Idimvencargado", "Idimvencargado");
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado");
            return View();
        }

        //[HttpGet]
        //public IActionResult GetAnioActual()
        //{
        //    var anioActual = _context.Años
        //        .OrderByDescending(a => a.Idaño)
        //        .Select(a => new { idAño = a.Idaño })
        //        .FirstOrDefault();

        //    return Json(anioActual);
        //}

        [HttpGet]
        public IActionResult BuscarEstudiante(string ididentidad)
        {
            var estudiante = _context.Matriculas
                .Where(m => m.Idestudiante == ididentidad)
                .OrderByDescending(m => m.Idaño)
                .FirstOrDefault();

            if (estudiante != null)
            {
                var padre = _context.Padres.FirstOrDefault(p => p.Idpadre == estudiante.Idpadre);

                return Json(new
                {
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
                    proviene = estudiante.Proviene,
                    beca = estudiante.Beca,
                    repiteAño = estudiante.RepiteAño,
                    observacion = estudiante.Observacion,
                    estadoIngreso = estudiante.EstadoMatricula,
                    fechaIngreso = estudiante.FechaIngreso?.ToString("yyyy-MM-dd"),
                    idpadre = padre?.Idpadre,
                    nombrePadre = padre?.NombrePadre,
                    parentesco = padre?.Parentesco,
                    telefonoPadre = padre?.TelefonoPadre,
                    celPadre = padre?.CelPadre,
                    direccionPadre = padre?.DireccionPadre,
                    estudianteRegistrado = true
                });
            }

            return Json(new { estudianteRegistrado = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Matricula matricula)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(matricula);
                    await _context.SaveChangesAsync();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new
                        {
                            success = true,
                            message = "El alumno ha sido matriculado exitosamente"
                        });
                    }

                    TempData["SuccessMessage"] = "El alumno ha sido matriculado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Ocurrió un error al guardar: {ex.Message}"
                        });
                    }

                    ModelState.AddModelError("", $"Ocurrió un error al guardar: {ex.Message}");
                }
            }

            // Si es AJAX, devolver errores de validación
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new
                {
                    success = false,
                    message = "Error de validación",
                    errors = errors
                });
            }

            // Recargar ViewData para el formulario
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", matricula.Idaño);
            ViewData["Idimvencargado"] = new SelectList(_context.Padres, "Idimvencargado", "Idimvencargado", matricula.Idimvencargado);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", matricula.Idgrado);

            return View(matricula);
        }

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
        [HttpGet]
        public IActionResult BuscarPadres(string term)
        {
            var padres = _context.Padres
                .Where(p => p.Idpadre.Contains(term) || p.NombrePadre.Contains(term))
                .Select(p => new {
                    idPadre = p.Idpadre,
                    nombrePadre = p.NombrePadre,
                    parentesco = p.Parentesco,
                    telefonoPadre = p.TelefonoPadre,
                    celPadre = p.CelPadre,
                    direccionPadre = p.DireccionPadre
                })
                .Take(10)
                .ToList();

            return Json(padres);
        }

        [HttpGet]
        public async Task<JsonResult> GetGradosPorAnio(int idAnio)
        {
            var grados = await _context.Grados
                .Where(g => g.Idaño == idAnio)
                .Select(g => new { idGrado = g.Idgrado }) // Asegúrate que el nombre de la propiedad coincide
                .ToListAsync();

            return Json(grados);
        }

        [HttpGet]
        public IActionResult GetDetallesGrado(string idGrado)
        {
            var detallesGrado = _context.Grados
                .Where(g => g.Idgrado == idGrado)
                .Select(g => new
                {
                    grado1 = g.Grado1,  // Asegúrate que estos nombres coincidan
                    seccion = g.Seccion,
                    jornada = g.Jornada,
                    nivelDescripcion = g.NivelDescripcion
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
            ViewData["Idimvencargado"] = new SelectList(_context.Padres, "Idimvencargado", "Idimvencargado", matricula.Idimvencargado);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", matricula.Idgrado);
            return View(matricula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
     int id,
     [Bind("Idaño,Idimvencargado,Idpadre,NombrePadre,Parentesco,Idest,Idimv,Idestudiante,Nacionalidad,NombreEstudiante,FechaNacimiento,LugarNacimiento,Genero,TipoSangre,Alergia,Edad,TelefonoEstudiante,CelularEstudiante,Correo,Direccion,Estado,Idgrado,Grado,Seccion,Jornada,Proviene,Beca,RepiteAño,Observacion,NivelDescripcion,TelefonoPadre,CelPadre,DireccionPadre,VacunasCovid,EstadoMatricula,FechaIngreso")]
    Matricula matricula)
        {
            if (id != matricula.Idest)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "ID de matrícula no coincide" });
                }
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matricula);
                    await _context.SaveChangesAsync();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new
                        {
                            success = true,
                            message = "Los cambios se guardaron correctamente"
                        });
                    }

                    TempData["SuccessMessage"] = "Los cambios se guardaron correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!MatriculaExists(matricula.Idest))
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "La matrícula no existe" });
                        }
                        return NotFound();
                    }
                    else
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Error de concurrencia: " + ex.Message });
                        }
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Error al guardar: " + ex.Message
                        });
                    }

                    ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                }
            }

            // Si es AJAX, devolver errores de validación
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new
                {
                    success = false,
                    message = "Error de validación",
                    errors = errors
                });
            }

            // Recargar ViewData para el formulario
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", matricula.Idaño);
            ViewData["Idimvencargado"] = new SelectList(_context.Padres, "Idimvencargado", "Idimvencargado", matricula.Idimvencargado);
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
                .Include(m => m.IdimvencargadoNavigation)
                .Include(m => m.IdgradoNavigation)
                .FirstOrDefaultAsync(m => m.Idest == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, Matricula model)
        {
            try
            {
                var matricula = await _context.Matriculas.FindAsync(id);
                if (matricula != null)
                {
                    _context.Matriculas.Remove(matricula);
                    await _context.SaveChangesAsync();

                    // Pasar el modelo a la vista
                    ViewBag.DeleteSuccess = true;
                    return View(model); // Devuelve la vista con el modelo
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.DeleteError = "Ocurrio un error al eliminar la matricula, este alumno ya tiene registros en la Base de Datos.";
                return View(await _context.Matriculas.FindAsync(id));
            }
        }

        private bool MatriculaExists(int id)
        {
            return _context.Matriculas.Any(e => e.Idest == id);
        }
    }
}