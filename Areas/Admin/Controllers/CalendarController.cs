using BlueDream.Data;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlueDream.Areas.Admin.Controllers
{
 
    public class CalendarController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Calendar
        public async Task<IActionResult> Index()
        {
            var slots = await _context.Calendars
                .OrderBy(c => c.Date)
                .ThenBy(c => c.StartTime)
                .ToListAsync();

            return View(slots); // ویو Index.cshtml در مسیر /Areas/Admin/Views/Calendar/Index.cshtml
        }

        // GET: Admin/Calendar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Calendar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Calendar model)
        {
            if (ModelState.IsValid)
            {
                _context.Calendars.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/Calendar/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var slot = await _context.Calendars.FindAsync(id);
            if (slot == null) return NotFound();

            return View(slot);
        }

        // POST: Admin/Calendar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Calendar model)
        {
            if (id != model.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/Calendar/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var slot = await _context.Calendars.FindAsync(id);
            if (slot == null) return NotFound();

            _context.Calendars.Remove(slot);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
