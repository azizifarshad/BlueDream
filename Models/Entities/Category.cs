using System.Collections.Generic;

namespace BlueDream.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ItemGroup> ItemGroups { get; set; } = new List<ItemGroup>();
    }
}