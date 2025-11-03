namespace BlueDream.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int TimeSpend { get; set; }
        public decimal Discount { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}