using System;
using System.Collections.Generic;

namespace BlueDream.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeFinish { get; set; }
        public decimal PriceWithoutCount { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountPrice { get; set; }

        public string StatusTypeId { get; set; }
        public StatusType StatusType { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}