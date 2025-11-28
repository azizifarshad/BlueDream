using System.Collections.Generic;
using BlueDream.Models.Entities;

namespace BlueDream.Models.ViewModels
{
    public class AboutViewModel
    {
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string IntroHtml { get; set; } // HTML-safe string (Html.Raw in view)
        public string Amenities { get; set; }
        public List<string> PaymentMethods { get; set; }
        public List<string> Languages { get; set; }
        public string ProductsUsed { get; set; }
        public IEnumerable<SocialLink> SocialLinks { get; set; }
    }
}