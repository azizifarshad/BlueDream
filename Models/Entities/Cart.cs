using System;
using System.Collections.Generic;

namespace BlueDream.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime TimeStart { get; set; }
        public decimal TotalTime { get; set; }
        public decimal PriceWithoutCount { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public StatusEnum Status { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Item> Items { get; set; } = [];
    }
}

