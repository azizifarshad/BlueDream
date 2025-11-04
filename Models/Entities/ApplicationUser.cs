using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BlueDream.Models.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}