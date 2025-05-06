using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SysnaraIMAVA.Controllers
{
    public class RecursosController : Controller
    {
        // GET: RecursosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RecursosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RecursosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RecursosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RecursosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RecursosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RecursosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RecursosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
