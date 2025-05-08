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
    public class GradoController : Controller
    {
        private readonly DbimavaContext _context;

        public GradoController(DbimavaContext context)
        {
            _context = context;
        }

        // GET: Grado
        public async Task<IActionResult> Index()
        {
            var dbsmileContext = _context.Grados.Include(g => g.IdañoNavigation);
            var años = await _context.Años.ToListAsync();
            ViewBag.Años = años;
            return View(await dbsmileContext.ToListAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetGrados(int idAño)
        {
            if (idAño <= 0)
            {
                return Json(new List<object>());
            }

            var grados = await _context.Grados
                .Where(g => g.Idaño == idAño)
                .Select(g => new {
                    idgrado = g.Idgrado,
                    grado1 = g.Grado1,
                    seccion = g.Seccion,
                    jornada = g.Jornada,
                    niveldescripcion = g.NivelDescripcion,
                    idaño = g.Idaño
                })
                .ToListAsync();

            return Json(grados);
        }

        // GET: Grado/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grado = await _context.Grados
                .Include(g => g.IdañoNavigation)
                .FirstOrDefaultAsync(m => m.Idgrado == id);
            if (grado == null)
            {
                return NotFound();
            }

            return View(grado);
        }

        // GET: Grado/Create
        public IActionResult Create()
        {
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño");

            List<string> grades = new List<string>
            {
                "SÉPTIMO GRADO",
                "OCTAVO GRADO",
                "NOVENO GRADO",
                "DÉCIMO GRADO",
                "UNDÉCIMO GRADO",
                "DUODÉCIMO GRADO"
            };
            ViewBag.Grado1 = new SelectList(grades);
            return View();
        }

        // POST: Grado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idaño,Idgrado,Grado1,Seccion,Jornada,NivelDescripcion")] Grado grado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", grado.Idaño);

            List<string> grades = new List<string>
            {
                "SÉPTIMO GRADO",
                "OCTAVO GRADO",
                "NOVENO GRADO",
                "DÉCIMO GRADO",
                "UNDÉCIMO GRADO",
                "DUODÉCIMO GRADO"
            };
            ViewBag.Grado1 = new SelectList(grades, grado.Grado1);
            return View(grado);
        }

        // GET: Grado/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grado = await _context.Grados.FindAsync(id);
            if (grado == null)
            {
                return NotFound();
            }
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", grado.Idaño);
            return View(grado);
        }

        // POST: Grado/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Idaño,Idgrado,Grado1,Seccion,Jornada,NivelDescripcion")] Grado grado)
        {
            if (id != grado.Idgrado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradoExists(grado.Idgrado))
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
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", grado.Idaño);
            return View(grado);
        }

        // GET: Grado/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grado = await _context.Grados
                .Include(g => g.IdañoNavigation)
                .FirstOrDefaultAsync(m => m.Idgrado == id);
            if (grado == null)
            {
                return NotFound();
            }

            return View(grado);
        }

        // POST: Grado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var grado = await _context.Grados.FindAsync(id);
            if (grado != null)
            {
                _context.Grados.Remove(grado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradoExists(string id)
        {
            return _context.Grados.Any(e => e.Idgrado == id);
        }
    }
}