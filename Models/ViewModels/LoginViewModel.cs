using System.ComponentModel.DataAnnotations;

namespace BlueDream.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email or Phone")]
        public string EmailOrPhone { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}