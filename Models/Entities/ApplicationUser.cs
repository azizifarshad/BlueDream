using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BlueDream.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}