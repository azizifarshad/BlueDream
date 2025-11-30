using System;
using System.ComponentModel.DataAnnotations;

namespace BlueDream.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int UserId { get; set; }  // int چون ApplicationUser.Id هم int هست

        public ApplicationUser User { get; set; } // navigation property

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}