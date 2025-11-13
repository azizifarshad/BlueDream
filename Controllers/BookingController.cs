using BlueDream.Data;
using BlueDream.Models.Entities;
using BlueDream.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // ✅ Accessible for everyone (even without login)
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

        // ✅ Only logged-in users can proceed to calendar
        [Authorize]
        [HttpPost]
        public IActionResult Calendar(List<int> selectedItems)
        {
            if (selectedItems == null || !selectedItems.Any())
                return RedirectToAction("Index");

            HttpContext.Session.SetString("SelectedItems", string.Join(",", selectedItems));

            return View("Calendar");
        }

        // ✅ Only logged-in users can submit date/time
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Submit(string selectedTime)
        {
            var selectedItemsString = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(selectedItemsString))
                return RedirectToAction("Index");

            var selectedIds = selectedItemsString.Split(',').Select(int.Parse).ToList();
            var selectedItems = await _context.Items.Where(i => selectedIds.Contains(i.Id)).ToListAsync();

            if (string.IsNullOrEmpty(selectedTime))
            {
                ModelState.AddModelError("selectedTime", "No time selected.");
                return View("Calendar");
            }

            if (!DateTime.TryParse(selectedTime, out var parsedDateTime))
            {
                ModelState.AddModelError("selectedTime", "Invalid time format.");
                return View("Calendar");
            }

            var vm = new SubmitBookingViewModel
            {
                SelectedItems = selectedItems,
                SelectedDateTime = parsedDateTime
            };

            return View(vm);
        }

        // ✅ Final booking confirmation (requires login)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(SubmitBookingViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var selectedItemsString = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(selectedItemsString))
                return RedirectToAction("Index");

            var selectedIds = selectedItemsString.Split(',').Select(int.Parse).ToList();
            var selectedItems = await _context.Items.Where(i => selectedIds.Contains(i.Id)).ToListAsync();

            var cart = new Cart
            {
                UserId = user.Id,
                TimeStart = model.SelectedDateTime,
                TotalTime = selectedItems.Sum(i => i.TimeSpend),
                PriceWithoutCount = selectedItems.Sum(i => i.Price),
                DiscountPrice = selectedItems.Sum(i => i.Discount),
                FinalPrice = selectedItems.Sum(i => i.Price - i.Discount),
                Status = StatusEnum.Created,
                Items = selectedItems
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("SelectedItems");

            // 🔹 Redirect to user profile (orders tab)
            return RedirectToAction("Profile", "Account", new { activeTab = "orders" });
        }

        // ✅ Cancel booking (requires login)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var order = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);

            if (order == null)
                return NotFound();

            if (order.Status != StatusEnum.Done && order.Status != StatusEnum.Canceled)
            {
                order.Status = StatusEnum.Canceled;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Profile", "Account", new { activeTab = "orders" });
        }
    }
}
