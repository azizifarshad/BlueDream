using System.ComponentModel.DataAnnotations;

namespace BlueDream.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Last Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
    }
}