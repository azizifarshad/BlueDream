namespace BlueDream.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public decimal TimeSpend { get; set; }
        public int Discount { get; set; }

        public int ItemGroupId { get; set; }
        public ItemGroup ItemGroup { get; set; }

        // NEW: ارتباط CartItem
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // حذف کردیم:
        // public ICollection<Cart> Carts { get; set; } = [];
    }
}