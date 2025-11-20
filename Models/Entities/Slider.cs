using System.ComponentModel.DataAnnotations;

namespace BlueDream.Models.Entities
{
    public class Slider
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }   // مسیر عکس در wwwroot

        public string Title { get; set; }

        public string Description { get; set; }
    }
}