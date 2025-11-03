using System.Collections.Generic;

namespace BlueDream.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}