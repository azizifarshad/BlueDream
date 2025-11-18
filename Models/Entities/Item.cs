namespace BlueDream.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int TimeSpend { get; set; }
        public bool IsActive { get; set; } = true;

        public int ItemGroupId { get; set; }
        public ItemGroup? ItemGroup { get; set; }

        public List<Cart> Carts { get; set; } = new();
    }


}