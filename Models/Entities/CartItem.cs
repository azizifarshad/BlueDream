using System.ComponentModel.DataAnnotations.Schema;

namespace BlueDream.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        // تعداد (اگر میخوای فقط انتخاب باشه، پیش‌فرض ۱)
        public int Quantity { get; set; } = 1;

        // قیمت لحظه انتخاب
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPrice { get; set; }
    }
}