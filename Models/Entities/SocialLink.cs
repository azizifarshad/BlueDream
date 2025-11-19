using System.ComponentModel.DataAnnotations;

namespace BlueDream.Models.Entities
{
    public class SocialLink
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }   // Instagram, YouTube...

        [Required]
        public string Url { get; set; }     // https://...

        public string IconUrl { get; set; } // مسیر عکس آیکون (اختیاری)
    }
}