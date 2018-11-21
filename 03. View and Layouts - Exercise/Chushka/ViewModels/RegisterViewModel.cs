using System.ComponentModel.DataAnnotations;

namespace Chushka.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(maximumLength: 32, ErrorMessage = "Username length must be between {2} and {1}", MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(maximumLength: 32, ErrorMessage = "Password length must be between {2} and {1}", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(maximumLength: 32, ErrorMessage = "Confirm Password length must be between {2} and {1}", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        //[StringLength(maximumLength: 64, ErrorMessage = "Full Name length must be between {2} and {1}", MinimumLength = 2)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
