using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CreditCardNo { get; set; }

        public string MobileNo { get; set; }

        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }

        public byte[] Photo { get; set; } 
    }
}
