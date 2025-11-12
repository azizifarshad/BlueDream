using BlueDream.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BlueDream.Models.ViewModels
{
    public class SubmitBookingViewModel
    {
        [Required(ErrorMessage = "زمان انتخاب نشده است.")]
        public DateTime SelectedDateTime { get; set; }

        [Required(ErrorMessage = "هیچ آیتمی انتخاب نشده است.")]
        public List<Item> SelectedItems { get; set; } = new();

        public decimal TotalPrice => SelectedItems?.Sum(i => i.Price) ?? 0;
        public decimal TotalDiscount => SelectedItems?.Sum(i => i.Discount) ?? 0;
        public decimal FinalPrice => SelectedItems?.Sum(i => i.Price - i.Discount) ?? 0;
    }
}