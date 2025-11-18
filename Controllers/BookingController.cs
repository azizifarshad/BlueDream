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
using System.Text.Json;

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
        // Index: نمایش دسته‌بندی‌ها و آیتم‌ها
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
        // Session Helpers
        // ---------------------------------------------------------------------
        private List<CartItemSession> GetCartSession()
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(sessionItems))
                return new List<CartItemSession>();
            return JsonSerializer.Deserialize<List<CartItemSession>>(sessionItems)!;
        }

        private void SaveCartSession(List<CartItemSession> cartItems)
        {
            HttpContext.Session.SetString("SelectedItems", JsonSerializer.Serialize(cartItems));
        }

        // ---------------------------------------------------------------------
        // Toggle item in cart (AJAX)
        // ---------------------------------------------------------------------
        [HttpPost]
        public IActionResult ToggleCartItem(int itemId)
        {
            var cartItems = GetCartSession();

            var item = _context.Items.AsNoTracking().FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound();

            int groupId = item.ItemGroupId;

            // حذف سایر آیتم‌های همان گروه
            cartItems = cartItems.Where(c =>
            {
                var it = _context.Items.AsNoTracking().FirstOrDefault(x => x.Id == c.ItemId);
                return it == null || it.ItemGroupId != groupId;
            }).ToList();

            // اضافه کردن آیتم با تعداد پیش‌فرض 1
            cartItems.Add(new CartItemSession { ItemId = itemId, Quantity = 1 });
            SaveCartSession(cartItems);

            return Ok(new { success = true });
        }

        // ---------------------------------------------------------------------
        // Update quantity (AJAX)
        // ---------------------------------------------------------------------
        [HttpPost]
        public IActionResult UpdateCartItem(int itemId, int quantity)
        {
            if (quantity < 0) quantity = 0;

            var cartItems = GetCartSession();
            var existing = cartItems.FirstOrDefault(x => x.ItemId == itemId);

            if (existing != null)
            {
                if (quantity == 0) cartItems.Remove(existing);
                else existing.Quantity = quantity;
            }
            else
            {
                if (quantity > 0)
                    cartItems.Add(new CartItemSession { ItemId = itemId, Quantity = quantity });
            }

            SaveCartSession(cartItems);
            return Ok(new { success = true });
        }

        // ---------------------------------------------------------------------
        // Get cart items (JSON)
        // ---------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var cartItems = GetCartSession();

            var items = _context.Items
                .AsNoTracking()
                .Where(i => cartItems.Select(c => c.ItemId).Contains(i.Id))
                .ToList()
                .Select(i =>
                {
                    var qty = cartItems.First(c => c.ItemId == i.Id).Quantity;
                    var price = i.Discount > 0 ? (i.Price - (i.Price * i.Discount / 100)) : i.Price;
                    return new
                    {
                        i.Id,
                        i.Name,
                        Quantity = qty,
                        Price = price,
                        Total = price * qty
                    };
                }).ToList();

            return Json(new
            {
                itemCount = items.Sum(x => x.Quantity),
                items = items,
                total = items.Sum(x => x.Total)
            });
        }

        // ---------------------------------------------------------------------
        // Calendar page
        // ---------------------------------------------------------------------
        [Authorize]
        [HttpPost]
        public IActionResult Calendar()
        {
            var cartItems = GetCartSession();
            if (!cartItems.Any())
                return RedirectToAction("Index");

            return View("Calendar");
        }

        // ---------------------------------------------------------------------
        // Submit booking (preview)
        // ---------------------------------------------------------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Submit(string selectedTime)
        {
            var cartItems = GetCartSession();
            if (!cartItems.Any())
                return RedirectToAction("Index");

            var selectedIds = cartItems.Select(c => c.ItemId).ToList();
            var selectedItems = await _context.Items
                .AsNoTracking()
                .Where(i => selectedIds.Contains(i.Id))
                .ToListAsync();

            if (string.IsNullOrEmpty(selectedTime) || !DateTime.TryParse(selectedTime, out var parsedDateTime))
            {
                ModelState.AddModelError("selectedTime", "Invalid time or no time selected.");
                return View("Calendar");
            }

            var vm = new SubmitBookingViewModel
            {
                SelectedItems = selectedItems,
                SelectedDateTime = parsedDateTime,
                Quantities = cartItems.ToDictionary(c => c.ItemId, c => c.Quantity)
            };

            return View("Submit", vm);
        }

        // ---------------------------------------------------------------------
        // Confirm booking (save to DB)
        // ---------------------------------------------------------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(SubmitBookingViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = GetCartSession();
            if (!cartItems.Any()) return RedirectToAction("Index");

            var selectedIds = cartItems.Select(c => c.ItemId).ToList();
            var selectedItems = await _context.Items
                .Where(i => selectedIds.Contains(i.Id))
                .ToListAsync();

            var cart = new Cart
            {
                UserId = user.Id,
                TimeStart = model.SelectedDateTime,
                TotalTime = selectedItems.Sum(i => i.TimeSpend * cartItems.First(c => c.ItemId == i.Id).Quantity),
                PriceWithoutCount = selectedItems.Sum(i => i.Price * cartItems.First(c => c.ItemId == i.Id).Quantity),
                DiscountPrice = selectedItems.Sum(i => (i.Price * i.Discount / 100) * cartItems.First(c => c.ItemId == i.Id).Quantity),
                FinalPrice = selectedItems.Sum(i =>
                {
                    var qty = cartItems.First(c => c.ItemId == i.Id).Quantity;
                    var price = i.Discount > 0 ? (i.Price - (i.Price * i.Discount / 100)) : i.Price;
                    return price * qty;
                }),
                Status = StatusEnum.Created,
                Items = selectedItems
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("SelectedItems");

            return RedirectToAction("Profile", "Account", new { activeTab = "orders" });
        }
    }

    // -------------------------------------------------------------------------
    // Cart item session helper
    // -------------------------------------------------------------------------
    public class CartItemSession
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
