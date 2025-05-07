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
    public class AsignaturasController : Controller
    {
        private readonly DbimavaContext _context;

        public AsignaturasController(DbimavaContext context)
        {
            _context = context;
        }

        // GET: Asignaturas
        public async Task<IActionResult> Index()
        {
            var dbsmileContext = _context.Asignaturas.Include(a => a.IdañoNavigation).Include(a => a.IdgradoNavigation);
            var años = await _context.Años.ToListAsync();
            ViewBag.Años = años;
            return View(await dbsmileContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAsignaturas(string idAño, string grado, string seccion)
        {
            var asignaturasQuery = _context.Asignaturas.AsQueryable();

            if (!string.IsNullOrEmpty(idAño) && int.TryParse(idAño, out int idAñoParsed))
            {
                asignaturasQuery = asignaturasQuery.Where(a => a.Idaño == idAñoParsed);
            }

            if (!string.IsNullOrEmpty(grado))
            {
                asignaturasQuery = asignaturasQuery.Where(a => a.Grado.ToUpper() == grado.ToUpper());
            }

            if (!string.IsNullOrEmpty(seccion))
            {
                asignaturasQuery = asignaturasQuery.Where(a => a.Seccion.ToUpper() == seccion.ToUpper());
            }

            var asignaturas = await asignaturasQuery
                .Include(a => a.IdañoNavigation)
                .Include(a => a.IdgradoNavigation)
                .Select(a => new
                {
                    a.Idasignatura,
                    asignatura = a.Asignatura1,
                    idgrado = a.Idgrado,
                    a.Grado,
                    a.Seccion,
                    a.Jornada,
                    periodo = a.Periodo,
                    anio = a.Idaño
                })
                .ToListAsync();

            return Json(asignaturas);
        }

        // GET: Asignaturas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignatura = await _context.Asignaturas
                .Include(a => a.IdañoNavigation)
                .Include(a => a.IdgradoNavigation)
                .FirstOrDefaultAsync(m => m.Idasignatura == id);
            if (asignatura == null)
            {
                return NotFound();
            }

            return View(asignatura);
        }

        // GET: Asignaturas/Create
        public IActionResult Create()
        {
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño");
            return View();
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

        [HttpGet]
        public async Task<IActionResult> GetNextAsignaturaId()
        {
            int nextId = await _context.Asignaturas.CountAsync() + 1;
            return Json(nextId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idaño,Idgrado,Grado,Seccion,Jornada,NivelDescripcion,Idasignatura,Asignatura1,Periodo")] Asignatura asignatura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asignatura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", asignatura.Idaño);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", asignatura.Idgrado);
            return View(asignatura);
        }

        // GET: Asignaturas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignatura = await _context.Asignaturas.FindAsync(id);
            if (asignatura == null)
            {
                return NotFound();
            }
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", asignatura.Idaño);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", asignatura.Idgrado);
            return View(asignatura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Idaño,Idgrado,Grado,Seccion,Jornada,NivelDescripcion,Idasignatura,Asignatura1,Periodo")] Asignatura asignatura)
        {
            if (id != asignatura.Idasignatura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asignatura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsignaturaExists(asignatura.Idasignatura))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", asignatura.Idaño);
            ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado", asignatura.Idgrado);
            return View(asignatura);
        }

        // GET: Asignaturas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignatura = await _context.Asignaturas
                .Include(a => a.IdañoNavigation)
                .Include(a => a.IdgradoNavigation)
                .FirstOrDefaultAsync(m => m.Idasignatura == id);
            if (asignatura == null)
            {
                return NotFound();
            }

            return View(asignatura);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var asignatura = await _context.Asignaturas.FindAsync(id);
            if (asignatura != null)
            {
                _context.Asignaturas.Remove(asignatura);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsignaturaExists(string id)
        {
            return _context.Asignaturas.Any(e => e.Idasignatura == id);
        }
    }
}