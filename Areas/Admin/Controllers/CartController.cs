using BlueDream.Data;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueDream.Areas.Admin.Controllers
{


    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
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
                .Include(c => c.CartItems)            // ← CartItems به جای Items
                .ThenInclude(ci => ci.Item)      // ← آیتم واقعی داخل CartItem
                .ThenInclude(i => i.ItemGroup)
                .ThenInclude(g => g.Category)
                .OrderByDescending(c => c.TimeStart)
                .ToListAsync();

            return View(carts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return NotFound();

            if (!Enum.TryParse<StatusEnum>(newStatus, out var statusEnum))
                return BadRequest("Invalid status value.");

            // اعتبارسنجی تغییر وضعیت
            switch (cart.Status)
            {
                case StatusEnum.Created:
                case StatusEnum.Requested:
                    if (statusEnum != StatusEnum.Confirmed && statusEnum != StatusEnum.Rejected)
                        return BadRequest("Invalid status transition.");
                    break;
                case StatusEnum.Confirmed:
                    if (statusEnum != StatusEnum.Done)
                        return BadRequest("Invalid status transition.");
                    break;
                default:
                    return BadRequest("Cannot change status from this state.");
            }

            cart.Status = statusEnum;
            
            await _context.SaveChangesAsync();
        
            TempData["ToastrMessage"] = $"Cart for {cart.User?.Name ?? cart.User?.UserName} on {cart.TimeStart:yyyy/MM/dd HH:mm} has been {statusEnum}.";

            return RedirectToAction(nameof(Index));
        }
    }
}

