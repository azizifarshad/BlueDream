using BlueDream.Data;
using BlueDream.Models.Entities;
using BlueDream.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlueDream.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ---------------- Register ----------------
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = $"{model.FirstName} {model.LastName}",
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ---------------- Login ----------------
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ApplicationUser user = null;

            if (model.EmailOrPhone.Contains("@"))
                user = await _userManager.FindByEmailAsync(model.EmailOrPhone);
            else
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.EmailOrPhone);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Invalid email/phone or password.");
            return View(model);
        }

        // ---------------- Logout ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ---------------- Profile ----------------
        [HttpGet]
        public async Task<IActionResult> Profile(string activeTab = "info")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var orderHistory = await _context.Carts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Item)
                        .ThenInclude(i => i.ItemGroup)
                            .ThenInclude(g => g.Category)
                .OrderByDescending(c => c.TimeStart)
                .ToListAsync();

            var model = new ProfileViewModel
            {
                FullName = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                OrderHistory = orderHistory
            };
            
            ViewData["ActiveTab"] = activeTab;
            
            return View(model);
        }

        // ---------------- Update Profile ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            user.Name = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;

            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        // ---------------- Change Password ----------------
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ---------------- Cancel Order ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int cartId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == user.Id);

            if (cart == null)
                return NotFound();

            // فقط می‌توان سفارش‌هایی که هنوز Done یا Canceled نشده را لغو کرد
            if (cart.Status == StatusEnum.Done || cart.Status == StatusEnum.Canceled)
            {
                TempData["ErrorMessage"] = "This order cannot be canceled.";
                return RedirectToAction("Profile", new { activeTab = "orders" });
            }

            cart.Status = StatusEnum.Canceled;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order canceled successfully!";
            return RedirectToAction("Profile", new { activeTab = "orders" });
        }
    }
}
