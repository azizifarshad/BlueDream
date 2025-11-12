using BlueDream.Data;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// 🔹 Database Configuration
// =====================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =====================================
// 🔹 Identity Configuration
// =====================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// =====================================
// 🔹 Cookie Authentication Settings
// =====================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    // اگر روی HTTPS هستی، این خط رو هم فعال کن:
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// =====================================
// 🔹 Add MVC + Session
// =====================================
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // زمان نگهداری session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =====================================
// 🔹 Build App
// =====================================
var app = builder.Build();

// =====================================
// 🔹 Middleware Pipeline
// =====================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 باید قبل از Authentication بیاد
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// =====================================
// 🔹 Routing
// =====================================

// مسیر Area (برای ادمین پنل)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

// مسیر پیش‌فرض کاربر
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// =====================================
app.Run();
