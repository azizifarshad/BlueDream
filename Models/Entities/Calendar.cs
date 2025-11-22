using System;
using System.ComponentModel.DataAnnotations;

namespace BlueDream.Models.Entities
{
    public enum CalendarSlotType
    {
        WorkingHour = 1,
        Holiday = 2
    }

    public class Calendar
    {
        public int Id { get; set; }

        // نوع Slot: ساعت باز یا تعطیلی
        [Required]
        public CalendarSlotType Type { get; set; }

        // تاریخ این slot
        [Required]
        public DateTime Date { get; set; }

        // زمان شروع و پایان در همان روز
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // توضیح اختیاری
        public string? Description { get; set; }

        // وضعیت فعال یا غیرفعال
        public bool IsActive { get; set; } = true;
    }
}