using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlueDream.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<int>,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ItemGroup> ItemGroups { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<SocialLink> SocialLinks { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Review> Reviews { get; set; }


    }
}