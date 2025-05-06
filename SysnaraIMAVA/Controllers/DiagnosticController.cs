using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SysnaraIMAVA.Controllers
{
    public class DiagnosticController : Controller
    {
        // GET: DiagnosticController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DiagnosticController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DiagnosticController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DiagnosticController/Create
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

        // GET: DiagnosticController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DiagnosticController/Edit/5
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

        // GET: DiagnosticController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DiagnosticController/Delete/5
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
