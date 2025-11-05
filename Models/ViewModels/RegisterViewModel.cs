using System.ComponentModel.DataAnnotations;

namespace BlueDream.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone, Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}