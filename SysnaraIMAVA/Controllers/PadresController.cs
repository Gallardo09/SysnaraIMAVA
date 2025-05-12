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
    public class PadresController : Controller
    {
        private readonly DbimavaContext _context;

        public PadresController(DbimavaContext context)
        {
            _context = context;
        }

        // GET: Padres
        public async Task<IActionResult> Index(int? idAño)
        {
            // Obtener todos los años disponibles para el dropdown
            var años = await _context.Años.ToListAsync();
            ViewBag.Años = años;

            // Filtrar padres por año si se seleccionó uno
            if (idAño.HasValue)
            {
                var padres = await _context.Padres
                    .Include(p => p.IdañoNavigation)
                    .Where(p => p.Idaño == idAño.Value)
                    .ToListAsync();

                ViewBag.AñoSeleccionado = idAño.Value;
                return View(padres);
            }

            // Si no se seleccionó año, retornar vista sin datos
            return View(new List<Padre>());
        }

        [HttpGet]
        public IActionResult ObtenerPadresPorAño(int idAño)
        {
            var padres = _context.Padres
                .Where(p => p.Idaño == idAño)
                .Select(p => new {
                    p.Idimvencargado,
                    p.Idpadre,
                    p.NombrePadre,
                    p.Parentesco,
                    p.Profesion,
                    p.Genero,
                    p.TelefonoPadre,
                    p.CelPadre,
                    p.Correo,
                    p.DireccionPadre,
                    p.Observacion
                })
                .ToList();

            return Json(padres);
        }

        [HttpGet]
        public IActionResult ObtenerPadresSinAño()
        {
            var padres = _context.Padres
                .Where(p => p.Idaño == null)
                .Select(p => new {
                    p.Idimvencargado,
                    p.Idpadre,
                    p.NombrePadre,
                    p.Parentesco,
                    p.Profesion,
                    p.Genero,
                    p.TelefonoPadre,
                    p.CelPadre,
                    p.Correo,
                    p.DireccionPadre,
                    p.Observacion
                })
                .ToList();

            return Json(padres);
        }
        // GET: Padres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padre = await _context.Padres
                .FirstOrDefaultAsync(m => m.Idimvencargado == id);
            if (padre == null)
            {
                return NotFound();
            }

            return View(padre);
        }

        // GET: Padres/Create
        public IActionResult Create()
        {
            var años = _context.Años.ToList();
            ViewBag.Años = años;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idencargado,Idpadre,NombrePadre,Parentesco,Profesion,Genero,TelefonoPadre,CelPadre,Correo,DireccionPadre,Observacion,Idaño")] Padre padre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(padre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(padre);
        }

        // GET: Padres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padre = await _context.Padres.FindAsync(id);
            if (padre == null)
            {
                return NotFound();
            }
            return View(padre);
        }

        // POST: Padres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idencargado,Idpadre,NombrePadre,Parentesco,Profesion,Genero,TelefonoPadre,CelPadre,Correo,DireccionPadre,Observacion")] Padre padre)
        {
            if (id != padre.Idimvencargado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(padre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PadreExists(padre.Idimvencargado))
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
            return View(padre);
        }

        // GET: Padres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padre = await _context.Padres
                .FirstOrDefaultAsync(m => m.Idimvencargado == id);
            if (padre == null)
            {
                return NotFound();
            }

            return View(padre);
        }

        // POST: Padres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var padre = await _context.Padres.FindAsync(id);
            if (padre != null)
            {
                _context.Padres.Remove(padre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PadreExists(int id)
        {
            return _context.Padres.Any(e => e.Idimvencargado == id);
        }
    }
}
