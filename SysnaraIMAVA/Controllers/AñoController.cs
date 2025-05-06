using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysnaraIMAVA.Models;
using System.Web;

namespace SysnaraIMAVA.Controllers
{
    public class AñoController(DbimavaContext context) : Controller
    {
        private readonly DbimavaContext _context = context;

        // GET: Año
        public async Task<IActionResult> Index()
        {
            return View(await _context.Años.ToListAsync());
        }

        // GET: Año/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var año = await _context.Años
                .FirstOrDefaultAsync(m => m.Idaño == id);
            if (año == null)
            {
                return NotFound();
            }

            return View(año);
        }

        // GET: Año/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Año/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idaño,AñoInicio,AñoFin,Observacion,Sistema,SistemaPeriodo")] Año año)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el Idaño ya existe en la base de datos
                var añoExistente = await _context.Años
                                                  .FirstOrDefaultAsync(a => a.Idaño == año.Idaño);
                if (añoExistente != null)
                {
                    // Si el año ya existe, mostrar un mensaje de advertencia
                    TempData["WarningMessage"] = "ADVERTENCIA: Este ID Año ya está registrado en la base de datos. Registre uno nuevo.";
                    return RedirectToAction(nameof(Create));
                }

                _context.Add(año);
                await _context.SaveChangesAsync();

                // Si el año se crea correctamente, mostrar un mensaje de éxito
                TempData["SuccessMessage"] = "El año escolar se ha creado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            return View(año);
        }

        // GET: Año/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var año = await _context.Años.FindAsync(id);
            if (año == null)
            {
                return NotFound();
            }
            return View(año);
        }

        // POST: Año/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idaño,AñoInicio,AñoFin,Observacion,Sistema,SistemaPeriodo")] Año año)
        {
            if (id != año.Idaño)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Convertir el valor de Sistema a mayúsculas antes de guardarlo
                   // año.Sistema = año.Sistema.ToUpper();

                    _context.Update(año);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AñoExists(año.Idaño))
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
            return View(año);
        }

        // GET: Año/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var año = await _context.Años
                .FirstOrDefaultAsync(m => m.Idaño == id);
            if (año == null)
            {
                return NotFound();
            }

            return View(año);
        }

        // POST: Año/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var año = await _context.Años.FindAsync(id);
            if (año != null)
            {
                _context.Años.Remove(año);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AñoExists(int id)
        {
            return _context.Años.Any(e => e.Idaño == id);
        }
    }
}
