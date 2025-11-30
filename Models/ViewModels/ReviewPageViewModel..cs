using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BlueDream.Models.Entities;

namespace BlueDream.Models.ViewModels
{
    public class ReviewViewModel
    {
        public int ServiceId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }
    }

    public class ReviewPageViewModel
    {
        public int ServiceId { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
        public ReviewViewModel NewReview { get; set; } = new ReviewViewModel();
    }
}