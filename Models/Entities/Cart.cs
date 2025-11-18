using System;
using System.Collections.Generic;

namespace BlueDream.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string? GuestId { get; set; }      // برای مهمان
        public int? UserId { get; set; }          // بعد از لاگین پر می‌شود
        public ApplicationUser? User { get; set; }

        public DateTime TimeStart { get; set; }
        public int TotalTime { get; set; }
        public decimal PriceWithoutCount { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Created;

        public List<Item> Items { get; set; } = new(); // Many-to-Many
    }

}

