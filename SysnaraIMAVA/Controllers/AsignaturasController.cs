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

            // Obtener los años
            var años = await _context.Años.ToListAsync();

            // Pasar tanto los grados como los años a la vista usando ViewBag
            ViewBag.Años = años;

            // Retornar la vista con la lista de grados
            //return View(await dbsmileContext.ToListAsync());
            return View(await dbsmileContext.ToListAsync());

        }

        // GET: Obtener asignaturas filtradas por Año, Grado y Sección
        [HttpGet]
        public async Task<IActionResult> GetAsignaturas(string idAño, string grado, string seccion)
        {
            var asignaturasQuery = _context.Asignaturas.AsQueryable();

            // Filtrar por Año
            if (!string.IsNullOrEmpty(idAño) && int.TryParse(idAño, out int idAñoParsed))
            {
                asignaturasQuery = asignaturasQuery.Where(a => a.Idaño == idAñoParsed);
            }

            // Filtrar por Grado
            if (!string.IsNullOrEmpty(grado))
            {
                asignaturasQuery = asignaturasQuery.Where(a => a.Grado.ToUpper() == grado.ToUpper());
            }

            // Filtrar por Sección
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
                    anio = a.Idaño, // Suponiendo que tienes una propiedad AñoDescripcion en la entidad Año
                    //Estado = a.Sistema, // O cualquier otra propiedad que describa el estado
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
            //ViewData["Idgrado"] = new SelectList(_context.Grados, "Idgrado", "Idgrado");
            return View();
        }
        //GET CREATE PARA PONER ASIGNATURAS SEGUN ID AÑO
        //---------- inicio ---------
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

        //GET ASIGNATURA / AUTORELLENAR (CREATE) Cuando seleccione el IDGRADO que se autorellenen los demas datos
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
                    g.NivelDescripcion,
                    g.Sistema
                })
                .FirstOrDefault();

            return Json(detallesGrado);
        }
        //GET ASIGNATURA UNA MANERA DE CONTAR TODAS LAS ASIGNATURAS DE LA BASE DE DATOS Y AUTOINCREMENTAR UN VALOR EN EL ID ASIGNATURA
        [HttpGet]
        public async Task<IActionResult> GetNextAsignaturaId()
        {
            int nextId = await _context.Asignaturas.CountAsync() + 1;
            return Json(nextId);
        }

        //---------- fin --------------

        // POST: Asignaturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idaño,Idgrado,Grado,Seccion,Jornada,NivelDescripcion,Idasignatura,Asignatura1,Periodo,Sistema,SistemaClase,SistemaTiempo")] Asignatura asignatura)
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

        // POST: Asignaturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Idaño,Idgrado,Grado,Seccion,Jornada,NivelDescripcion,Idasignatura,Asignatura1,Periodo,Sistema,SistemaClase,SistemaTiempo")] Asignatura asignatura)
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

        // POST: Asignaturas/Delete/5
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
