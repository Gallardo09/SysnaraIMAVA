using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Humanizer;
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
            // Obtener los grados y sus relaciones
            var dbsmileContext = _context.Grados.Include(g => g.IdañoNavigation);

            // Obtener los años
            var años = await _context.Años.ToListAsync();

            // Pasar tanto los grados como los años a la vista usando ViewBag
            ViewBag.Años = años;

            // Retornar la vista con la lista de grados
            return View(await dbsmileContext.ToListAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetGrados(int idAño, string sistema)
        {
            // Only proceed if a year is selected
            if (idAño <= 0)
            {
                return Json(new List<object>());
            }

            var query = _context.Grados
                .Where(g => g.Idaño == idAño);

            // Case-insensitive sistema filtering
            if (!string.IsNullOrEmpty(sistema))
            {
                query = query.Where(g =>
                    g.Sistema != null &&
                    g.Sistema.ToUpper() == sistema.ToUpper()
                );
            }

            var grados = await query
                .Select(g => new {
                    idgrado = g.Idgrado,
                    grado1 = g.Grado1,
                    seccion = g.Seccion,
                    jornada = g.Jornada,
                    niveldescripcion = g.NivelDescripcion,
                    sistema = g.Sistema,
                    sistematiempo = g.SistemaTiempo,
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

            //---------------- Inicio----------------
            // Lista de grados
            List<string> grades = new List<string>
            {
                "KINDERGARTEN",
                "PREPARATORY",
                "FIRST GRADE",
                "SECOND GRADE",
                "THIRD GRADE",
                "FOURTH GRADE",
                "FIFTH GRADE",
                "SIXTH GRADE",
                "SEVENTH GRADE",
                "EIGHTH GRADE",
                "NINTH GRADE",
                "TENTH GRADE",
                "ELEVENTH GRADE",
                "TWELVE GRADE"
            };
            // Pasar la lista a la vista usando ViewBag
            ViewBag.Grado1 = new SelectList(grades);
            //---------------- fin----------------
            return View();
        }

        // POST: Grado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idaño,Idgrado,Grado1,Seccion,Jornada,NivelDescripcion,Sistema,SistemaClase,SistemaTiempo")] Grado grado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idaño"] = new SelectList(_context.Años, "Idaño", "Idaño", grado.Idaño);

            //PARA AGREGAR LA LISTA AUTOMATICA EN EL SELECT DE LA VISTA CREATE 
            //Inicio
            List<string> grades = new List<string>
            {
                "KINDERGARTEN",
                "PREPARATORY",
                "FIRST GRADE",
                "SECOND GRADE",
                "THIRD GRADE",
                "FOURTH GRADE",
                "FIFTH GRADE",
                "SIXTH GRADE",
                "SEVENTH GRADE",
                "EIGHTH GRADE",
                "NINTH GRADE",
                "TENTH GRADE",
                "ELEVENTH GRADE",
                "TWELVE GRADE"
            };
            ViewBag.Grado1 = new SelectList(grades, grado.Grado1);
            //Fin
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Idaño,Idgrado,Grado1,Seccion,Jornada,NivelDescripcion,Sistema,SistemaClase,SistemaTiempo")] Grado grado)
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
