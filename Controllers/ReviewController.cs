using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BlueDream.Data;
using BlueDream.Models.Entities;
using BlueDream.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlueDream.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Reviews/Index?serviceId=1
        [HttpGet]
        public async Task<IActionResult> Index(int serviceId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ServiceId == serviceId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var model = new ReviewPageViewModel
            {
                ServiceId = serviceId,
                Reviews = reviews,
                NewReview = new ReviewViewModel { ServiceId = serviceId }
            };

            return View(model);
        }

        // POST: /Reviews/Index (Ajax یا فرم معمولی)
        [HttpPost]
        [Authorize] // فقط کاربر لاگین شده می‌تواند ریویو ثبت کند
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ReviewPageViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Forbid();

            if (!ModelState.IsValid)
            {
                // بارگذاری مجدد ریویوها برای PartialView
                model.Reviews = await _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.ServiceId == model.ServiceId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_ReviewsList", model);

                return View(model);
            }

            // ایجاد ریویو جدید
            var review = new Review
            {
                ServiceId = model.NewReview.ServiceId,
                UserId = user.Id,
                Rating = model.NewReview.Rating,
                Comment = model.NewReview.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // بارگذاری مجدد ریویوها برای PartialView
            model.Reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ServiceId == model.ServiceId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // اگر Ajax است فقط PartialView برگردان
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_ReviewsList", model);

            // فرم معمولی: ریدایرکت به همان صفحه
            return RedirectToAction(nameof(Index), new { serviceId = model.ServiceId });
        }
    }
}
