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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Forbid();

            var review = new Review
            {
                ServiceId = model.ServiceId,
                UserId = user.Id,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ServiceId == model.ServiceId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var pageModel = new ReviewPageViewModel
            {
                ServiceId = model.ServiceId,
                Reviews = reviews,
                NewReview = new ReviewViewModel { ServiceId = model.ServiceId }
            };

            return PartialView("_ReviewsList", pageModel);
        }
    }
}
