using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueDream.Data;
using BlueDream.Models.Entities;
using BlueDream.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace BlueDream.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context,
                                 UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Index
        public async Task<IActionResult> Index(int serviceId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ServiceId == serviceId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = new ReviewPageViewModel
            {
                ServiceId = serviceId,
                Reviews = reviews
            };

            return View(vm);
        }

        // POST: Create (AJAX)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review model)
        {
            if (model.Rating < 1 || model.Rating > 5)
                return BadRequest("Invalid rating value");

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return Forbid();

            // convert user id to int (because Review.UserId is int)
            if (!int.TryParse(userIdString, out var userId))
                return Forbid();

            var review = new Review
            {
                Rating = model.Rating,
                Comment = model.Comment,
                ServiceId = model.ServiceId,
                UserId = userId,                     // <-- fixed: int not Guid
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return await RefreshReviews(model.ServiceId);
        }

        // GET: Edit modal (AJAX: returns partial)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            if (!IsAllowed(review))
                return Forbid();

            return PartialView("_EditReviewModal", review);
        }

        // POST: Edit (AJAX)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Review model)
        {
            var review = await _context.Reviews.FindAsync(model.Id);

            if (review == null)
                return NotFound();

            if (!IsAllowed(review))
                return Forbid();

            // update allowed fields only
            review.Rating = model.Rating;
            review.Comment = model.Comment;
            // DO NOT set UpdatedAt because the entity doesn't have that property

            await _context.SaveChangesAsync();

            return await RefreshReviews(review.ServiceId);
        }

        // POST: Delete (AJAX)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            if (!IsAllowed(review))
                return Forbid();

            var serviceId = review.ServiceId;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return await RefreshReviews(serviceId);
        }

        // helper: return partial with up-to-date review list
        private async Task<IActionResult> RefreshReviews(int serviceId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ServiceId == serviceId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = new ReviewPageViewModel
            {
                ServiceId = serviceId,
                Reviews = reviews
            };

            return PartialView("_ReviewsList", vm);
        }

        // access control helper
        private bool IsAllowed(Review review)
        {
            if (User.IsInRole("Admin"))
                return true;

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return false;

            if (!int.TryParse(userIdString, out var userId))
                return false;

            return review.UserId == userId;
        }
    }
}
