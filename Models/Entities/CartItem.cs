namespace BlueDream.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Count { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}