using BlueDream.Models.Entities;
using System;
using System.Collections.Generic;

namespace BlueDream.Models.ViewModels
{
    public class SubmitBookingViewModel
    {
        public List<Item> SelectedItems { get; set; } = new();

        // نگه داشتن تعداد هر آیتم (ItemId -> Quantity)
        public Dictionary<int, int> Quantities { get; set; } = new();

        public DateTime SelectedDateTime { get; set; }

        // پراپرتی‌های محاسباتی
        public decimal TotalPrice => SelectedItems.Sum(i => i.Price * GetQuantity(i.Id));
        public decimal TotalDiscount => SelectedItems.Sum(i => (i.Price * i.Discount / 100) * GetQuantity(i.Id));
        public decimal FinalPrice => TotalPrice - TotalDiscount;

        private int GetQuantity(int itemId)
        {
            if (Quantities != null && Quantities.ContainsKey(itemId))
                return Quantities[itemId];
            return 1; // پیش‌فرض یک
        }
    }

}