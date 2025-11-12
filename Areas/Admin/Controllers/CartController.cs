using BlueDream.Data;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueDream.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var carts = await _context.Carts
                .Include(c => c.User)
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemGroup)
                .ThenInclude(g => g.Category)
                .OrderByDescending(c => c.TimeStart)
                .ToListAsync();

            return View(carts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, StatusEnum newStatus)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            cart.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["Message"] = $"وضعیت رزرو شماره {cart.Id} با موفقیت تغییر کرد.";
            return RedirectToAction(nameof(Index));
        }
    }
}