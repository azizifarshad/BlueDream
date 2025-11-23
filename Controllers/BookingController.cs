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

        // ------------------------- Index (Select Items) -----------------------
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

        // ------------------------- ToggleCartItem (AJAX) ----------------------
        [HttpPost]
        public IActionResult ToggleCartItem(int itemId)
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            List<int> selectedIds = new();

            if (!string.IsNullOrEmpty(sessionItems))
                selectedIds = sessionItems.Split(',').Select(int.Parse).ToList();

            var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound();

            int groupId = item.ItemGroupId;
            var groupItems = _context.Items
                .Where(i => i.ItemGroupId == groupId)
                .Select(i => i.Id)
                .ToList();

            if (selectedIds.Contains(itemId))
            {
                selectedIds = selectedIds.Where(id => !groupItems.Contains(id)).ToList();
                HttpContext.Session.SetString("SelectedItems", string.Join(",", selectedIds));
                return Ok(new { removed = true });
            }

            selectedIds = selectedIds.Where(id => !groupItems.Contains(id)).ToList();
            selectedIds.Add(itemId);
            HttpContext.Session.SetString("SelectedItems", string.Join(",", selectedIds));

            return Ok(new { removed = false });
        }

        // ------------------------- MiniCart JSON ------------------------------
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

        // --------------------------- Calendar View ---------------------------
        [Authorize]
        [HttpGet, HttpPost]
        public IActionResult Calendar()
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(sessionItems))
                return RedirectToAction("Index");

            var selectedIds = sessionItems.Split(',').Select(int.Parse).ToList();
            var selectedItems = _context.Items
                .Where(i => selectedIds.Contains(i.Id))
                .ToList();

            var vm = new SubmitBookingViewModel
            {
                SelectedItems = selectedItems
            };

            return View(vm);
        }

        // ---------------------- Get Available Time Slots -----------------------
        [HttpGet]
        public IActionResult GetAvailableTimeSlots(string date)
        {
            if (!DateTime.TryParse(date, out var selectedDate))
                return BadRequest("Invalid date");

            // تایم اسلات‌های فعال
            var slots = _context.Calendars
                .Where(c => c.IsActive && c.Date.Date == selectedDate.Date && c.Type == CalendarSlotType.WorkingHour)
                .Select(c => new
                {
                    start = c.StartTime.ToString(@"hh\:mm"),
                    end = c.EndTime.ToString(@"hh\:mm")
                })
                .ToList();

            // تایم‌های رزرو شده
            var bookedCarts = _context.Carts
                .Where(c => (c.Status == StatusEnum.Created || c.Status == StatusEnum.Confirmed)
                            && c.TimeStart.Date == selectedDate.Date)
                .ToList(); // <-- تبدیل به لیست اول، حل ambiguity

            var booked = bookedCarts
                .Select(c => new
                {
                    start = c.TimeStart.ToString("HH:mm"),
                    end = c.TimeStart.AddMinutes((double)c.TotalTime).ToString("HH:mm") // <-- cast decimal -> double
                })
                .ToList();

            return Json(new { slots, booked });
        }

        // ----------------------------- Submit --------------------------------
        [Authorize]
        [HttpPost]
        public IActionResult Submit(string selectedTime)
        {
            var sessionItems = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(sessionItems))
                return RedirectToAction("Index");

            var selectedIds = sessionItems.Split(',').Select(int.Parse).ToList();
            var selectedItems = _context.Items.Where(i => selectedIds.Contains(i.Id)).ToList();

            if (string.IsNullOrEmpty(selectedTime) || !DateTime.TryParse(selectedTime, out var parsedDateTime))
            {
                ModelState.AddModelError("SelectedDateTime", "Invalid time or no time selected.");
                return View("Calendar", new SubmitBookingViewModel { SelectedItems = selectedItems });
            }

            var vm = new SubmitBookingViewModel
            {
                SelectedItems = selectedItems,
                SelectedDateTime = parsedDateTime
            };

            return View(vm);
        }

        // --------------------------- ConfirmBooking ---------------------------
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
                CartItems = selectedItems.Select(i => new CartItem
                {
                    ItemId = i.Id,
                    Quantity = 1
                }).ToList()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("SelectedItems");

            return RedirectToAction("Profile", "Account", new { activeTab = "orders" });
        }
    }
}
