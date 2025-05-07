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
                    m.EstadoMatricula
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
        public async Task<IActionResult> Create(Matricula matricula, IFormFile fotoInput)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //if (fotoInput != null && fotoInput.Length > 0)
                    //{
                    //    using (var memoryStream = new MemoryStream())
                    //    {
                    //        await fotoInput.CopyToAsync(memoryStream);
                    //        matricula.FotoData = memoryStream.ToArray();
                    //    }
                    //}

                    _context.Add(matricula);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar la matrícula.");
                }
            }

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
        public IActionResult GetGradosPorAnio(int idAnio)
        {
            var grados = _context.Grados
                .Where(g => g.Idaño == idAnio)
                .Select(g => new { g.Idgrado })
                .ToList();
            return Json(grados);
        }

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
            ViewData["Idimvencargado"] = new SelectList(_context.Padres, "Idimvencargado", "Idimvencargado", matricula.Idimvencargado);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", matricula.Idgrado);
            return View(matricula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Idaño,Idimvencargado,Idpadre,NombrePadre,Parentesco,Idest,Idimv,Idestudiante,Nacionalidad,NombreEstudiante,FechaNacimiento,LugarNacimiento,Genero,TipoSangre,Alergia,Edad,TelefonoEstudiante,CelularEstudiante,Correo,Direccion,Estado,Idgrado,Grado,Seccion,Jornada,Proviene,Beca,RepiteAño,Observacion,NivelDescripcion,TelefonoPadre,CelPadre,DireccionPadre,VacunasCovid,EstadoMatricula,FechaIngreso")]
            Matricula matricula,
            IFormFile fotoInput)
        {
            if (id != matricula.Idest)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //if (fotoInput != null && fotoInput.Length > 0)
                    //{
                    //    using (var memoryStream = new MemoryStream())
                    //    {
                    //        await fotoInput.CopyToAsync(memoryStream);
                    //        matricula.FotoData = memoryStream.ToArray();
                    //    }
                    //}
                    //else
                    //{
                    //    var existingMatricula = await _context.Matriculas.AsNoTracking().FirstOrDefaultAsync(m => m.Idest == id);
                    //    if (existingMatricula != null)
                    //    {
                    //        matricula.FotoData = existingMatricula.FotoData;
                    //    }
                    //}

                    _context.Update(matricula);
                    await _context.SaveChangesAsync();
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

        //public IActionResult MostrarFoto(int id)
        //{
        //    var matricula = _context.Matriculas.FirstOrDefault(m => m.Idest == id);
        //    if (matricula?.FotoData != null)
        //    {
        //        return File(matricula.FotoData, "image/jpeg");
        //    }
        //    return NotFound();
        //}
    }
}