using BlueDream.Data;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlueDream.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ItemGroups)
                .ThenInclude(g => g.Items)
                .ToListAsync();

            ViewData["Title"] = "Service Management";
            return View(categories);
        }

        // ---------- Create ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _context.Categories.Add(new Category { Name = name });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(int categoryId, string name, string description)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var group = new ItemGroup { CategoryId = categoryId, Name = name, Description = description };
                _context.ItemGroups.Add(group);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int groupId, string name, decimal price, decimal timeSpend, int discount)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var item = new Item
                {
                    ItemGroupId = groupId,
                    Name = name,
                    Price = price,
                    TimeSpend = timeSpend,
                    Discount = discount,
                    IsActive = true
                };
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ---------- Edit (POST) ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string name)
        {
            if (id <= 0) return RedirectToAction(nameof(Index));
            var cat = await _context.Categories.FindAsync(id);
            if (cat != null && !string.IsNullOrWhiteSpace(name))
            {
                cat.Name = name;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(int id, string name, string description)
        {
            if (id <= 0) return RedirectToAction(nameof(Index));
            var group = await _context.ItemGroups.FindAsync(id);
            if (group != null && !string.IsNullOrWhiteSpace(name))
            {
                group.Name = name;
                group.Description = description;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(int id, string name, decimal price, decimal timeSpend, int discount, bool isActive)
        {
            if (id <= 0) return RedirectToAction(nameof(Index));
            var item = await _context.Items.FindAsync(id);
            if (item != null && !string.IsNullOrWhiteSpace(name))
            {
                item.Name = name;
                item.Price = price;
                item.TimeSpend = timeSpend;
                item.Discount = discount;
                item.IsActive = isActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ---------- Delete ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var cat = await _context.Categories
                .Include(c => c.ItemGroups)
                .ThenInclude(g => g.Items)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.ItemGroups
                .Include(g => g.Items)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (group != null)
            {
                _context.ItemGroups.Remove(group);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
