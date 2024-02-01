using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class SendEmail
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

