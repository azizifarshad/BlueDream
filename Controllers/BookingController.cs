using BlueDream.Data;
using BlueDream.Models.Entities;
using BlueDream.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlueDream.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------------------------------------------------------------------
        // ------------------------- Index (Select Items) -----------------------
        // ---------------------------------------------------------------------
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ItemGroups)
                .ThenInclude(g => g.Items)
                .ToListAsync();

            var vm = new BookingViewModel
            {
                Categories = categories.Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Name + " services",
                    ItemGroups = c.ItemGroups.Select(g => new ItemGroupVM
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        Items = g.Items.Select(i => new ItemVM
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            Price = i.Price,
                            Discount = i.Discount,
                            TimeSpend = i.TimeSpend
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return View(vm);
        }

        // ---------------------------------------------------------------------
        // ------------------------- ToggleCartItem (AJAX) ----------------------
        // ---------------------------------------------------------------------
        // انتخاب یک آیتم از هر گروه + toggle اگر دوباره روی همان گزینه کلیک شود
        [HttpPost]
        public IActionResult ToggleCartItem(int itemId)
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            List<int> selectedIds = new();

            if (!string.IsNullOrEmpty(sessionItems))
                selectedIds = sessionItems.Split(',').Select(int.Parse).ToList();

            var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return NotFound();

            int groupId = item.ItemGroupId;

            // آیتم‌های گروه
            var groupItems = _context.Items
                .Where(i => i.ItemGroupId == groupId)
                .Select(i => i.Id)
                .ToList();

            // اگر همین آیتم از قبل انتخاب شده بود → حذف گروه
            if (selectedIds.Contains(itemId))
            {
                selectedIds = selectedIds
                    .Where(id => !groupItems.Contains(id))
                    .ToList();

                HttpContext.Session.SetString("SelectedItems", string.Join(",", selectedIds));
                return Ok(new { removed = true });
            }

            // در حالت انتخاب جدید
            selectedIds = selectedIds
                .Where(id => !groupItems.Contains(id))
                .ToList();

            selectedIds.Add(itemId);

            HttpContext.Session.SetString("SelectedItems", string.Join(",", selectedIds));

            return Ok(new { removed = false });
        }

        // ---------------------------------------------------------------------
        // ------------------------- MiniCart JSON ------------------------------
        // ---------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            List<int> selectedIds = new();

            if (!string.IsNullOrEmpty(sessionItems))
                selectedIds = sessionItems.Split(',').Select(int.Parse).ToList();

            var items = _context.Items
                .Where(i => selectedIds.Contains(i.Id))
                .ToList()
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    Price = i.Discount > 0
                        ? (i.Price - (i.Price * i.Discount / 100))
                        : i.Price
                }).ToList();

            return Json(new
            {
                itemCount = items.Count,
                items = items,
                total = items.Sum(i => i.Price)
            });
        }

        // ---------------------------------------------------------------------
        // --------------------------- Calendar --------------------------------
        // ---------------------------------------------------------------------
        [Authorize]
        [HttpPost]
        public IActionResult Calendar()
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(sessionItems))
                return RedirectToAction("Index");

            return View();
        }

        // ---------------------------------------------------------------------
        // ----------------------------- Submit --------------------------------
        // ---------------------------------------------------------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Submit(string selectedTime)
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(sessionItems))
                return RedirectToAction("Index");

            var selectedIds = sessionItems.Split(',').Select(int.Parse).ToList();
            var selectedItems = await _context.Items.Where(i => selectedIds.Contains(i.Id)).ToListAsync();

            if (string.IsNullOrEmpty(selectedTime) || !DateTime.TryParse(selectedTime, out var parsedDateTime))
            {
                ModelState.AddModelError("selectedTime", "Invalid time or no time selected.");
                return View("Calendar");
            }

            var vm = new SubmitBookingViewModel
            {
                SelectedItems = selectedItems,
                SelectedDateTime = parsedDateTime
            };

            return View(vm);
        }

        // ---------------------------------------------------------------------
        // --------------------------- ConfirmBooking ---------------------------
        // ---------------------------------------------------------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(SubmitBookingViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(sessionItems))
                return RedirectToAction("Index");

            var selectedIds = sessionItems.Split(',').Select(int.Parse).ToList();
            var selectedItems = await _context.Items.Where(i => selectedIds.Contains(i.Id)).ToListAsync();

            var cart = new Cart
            {
                UserId = user.Id,
                TimeStart = model.SelectedDateTime,
                TotalTime = selectedItems.Sum(i => i.TimeSpend),
                PriceWithoutCount = selectedItems.Sum(i => i.Price),
                DiscountPrice = selectedItems.Sum(i => (i.Price * i.Discount / 100)),
                FinalPrice = selectedItems.Sum(i =>
                    i.Discount > 0 ? (i.Price - (i.Price * i.Discount / 100)) : i.Price),
                Status = StatusEnum.Created,
                Items = selectedItems
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("SelectedItems");

            return RedirectToAction("Profile", "Account", new { activeTab = "orders" });
        }
    }
}
