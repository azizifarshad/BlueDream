using BlueDream.Models.Entities;
using System.Collections.Generic;

namespace BlueDream.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }

        public List<Cart> OrderHistory { get; set; } = new();
    }
}