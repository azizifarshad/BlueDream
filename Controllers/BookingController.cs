using BlueDream.Data;
using BlueDream.Models.Entities;
using BlueDream.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlueDream.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 مرحله ۱: نمایش دسته‌بندی‌ها و آیتم‌ها
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

        // 🟡 مرحله ۲: دریافت آیتم‌های انتخابی و نمایش تقویم
        [HttpPost]
        public IActionResult Calendar(List<int> selectedItems)
        {
            if (selectedItems == null || !selectedItems.Any())
                return RedirectToAction("Index");

            // آیتم‌های انتخابی در سشن نگه‌داری می‌شن
            HttpContext.Session.SetString("SelectedItems", string.Join(",", selectedItems));

            return View("Calendar"); // نمایش تقویم برای انتخاب زمان
        }

        // 🔵 مرحله ۳: بعد از انتخاب زمان و کلیک "مرحله بعد"
        [HttpPost]
        public async Task<IActionResult> Submit(string selectedTime)
        {
            var selectedItemsString = HttpContext.Session.GetString("SelectedItems");
            if (string.IsNullOrEmpty(selectedItemsString))
                return RedirectToAction("Index");

            var selectedIds = selectedItemsString
                .Split(',')
                .Select(int.Parse)
                .ToList();

            // گرفتن آیتم‌های واقعی از دیتابیس
            var selectedItems = await _context.Items
                .Where(i => selectedIds.Contains(i.Id))
                .ToListAsync();

            if (string.IsNullOrEmpty(selectedTime))
            {
                ModelState.AddModelError("selectedTime", "زمان انتخاب نشده است.");
                return View("Calendar");
            }

            if (!DateTime.TryParse(selectedTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
            {
                ModelState.AddModelError("selectedTime", "فرمت زمان اشتباه است.");
                return View("Calendar");
            }

            var vm = new SubmitBookingViewModel
            {
                SelectedItems = selectedItems, // Item list
                SelectedDateTime = parsedDateTime
            };

            return View(vm); // نمایش صفحه Submit با اطلاعات آیتم‌ها و زمان
        }

        // 🔴 مرحله ۴: ثبت نهایی رزرو در دیتابیس
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(SubmitBookingViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Submit", model);

            var userId = 1; // موقتاً، بعداً از User.Identity گرفته میشه

            var cart = new Cart
            {
                UserId = userId,
                TimeStart = model.SelectedDateTime,
                TotalTime = model.SelectedItems.Sum(i => i.TimeSpend),
                PriceWithoutCount = model.SelectedItems.Sum(i => i.Price),
                DiscountPrice = model.SelectedItems.Sum(i => i.Discount),
                FinalPrice = model.SelectedItems.Sum(i => i.Price - i.Discount),
                Status = StatusEnum.Created,
                Items = model.SelectedItems
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // پاک کردن سشن بعد از ثبت نهایی
            HttpContext.Session.Remove("SelectedItems");

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
