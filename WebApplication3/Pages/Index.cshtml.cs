using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApplication3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
	[Authorize]
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
        private readonly IDataProtector protector;
        private readonly IHttpContextAccessor contxt;

        public IndexModel(ILogger<IndexModel> logger, IDataProtector protector, IHttpContextAccessor contxt)
		{
			_logger = logger;
			this.protector = protector;
            this.contxt = contxt;
        }

		public void OnGet()
		{
		}
        public string GetDecryptedCreditCardNo()
        {
            // Retrieve and decrypt the credit card number from session
            string encryptedCreditCardNo = contxt.HttpContext.Session.GetString("UserCreditCardNo");
            try
            {
                // Decrypt the credit card number
                return protector.Unprotect(encryptedCreditCardNo);
            }
            catch (Exception ex)
            {
                // Handle decryption error (e.g., log or display an error message)
                Console.WriteLine($"Error decrypting credit card number: {ex.Message}");
                return "Error decrypting credit card number";
            }
        }
    }
}
