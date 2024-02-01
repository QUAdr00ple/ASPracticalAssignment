using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class ResetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Enter at least a 8 characters password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$",
        ErrorMessage = "Passwords must be at least 8 characters long and contain at least an uppercase letter, lower case letter, digit and a symbol")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
