namespace BlueDream.Models.ViewModels
{
    public class BookingViewModel
    {
        public List<CategoryVM> Categories { get; set; } = new();
    }

    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public List<ItemGroupVM> ItemGroups { get; set; } = new();
    }

    public class ItemGroupVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public List<ItemVM> Items { get; set; } = new();
    }

    public class ItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal TimeSpend { get; set; }
        public int Discount { get; set; }
    }
}