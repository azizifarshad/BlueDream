using BlueDream.Models.Entities;
using BlueDream.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BlueDream.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ===== Register =====
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ساخت کاربر جدید
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

            // نمایش خطاها
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ===== Login =====
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ApplicationUser user = null;

            // بررسی اینکه ورودی ایمیل است یا شماره تلفن
            if (model.EmailOrPhone.Contains("@"))
                user = await _userManager.FindByEmailAsync(model.EmailOrPhone);
            else
                user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == model.EmailOrPhone);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            // احراز هویت
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Invalid email/phone or password.");
            return View(model);
        }

        // ===== Logout =====
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
