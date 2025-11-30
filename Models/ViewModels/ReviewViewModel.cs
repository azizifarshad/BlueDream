using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BlueDream.Models.Entities;

namespace BlueDream.Models.ViewModels
{
    // مدل برای فرم ارسال ریویو جدید
    public class ReviewViewModel
    {
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Please select a rating")]
        [Range(1,5)]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [MaxLength(500)]
        public string Comment { get; set; }
    }

    // مدل برای صفحه ریویو (لیست + فرم)
    public class ReviewPageViewModel
    {
        public int ServiceId { get; set; }

        // ریویوهای موجود برای سرویس
        public List<Review> Reviews { get; set; } = new List<Review>();

        // ریویو جدید
        public ReviewViewModel NewReview { get; set; }

        // اضافه: ID کاربر فعلی (int) برای بررسی مالکیت ریویو
        public int? CurrentUserId { get; set; }
    }
}