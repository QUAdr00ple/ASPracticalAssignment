using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.ViewModels;
using WebApplication3.Model;
using IdentityUser = Microsoft.AspNetCore.Identity.IdentityUser;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Pages
{
	public class LoginModel : PageModel
	{
		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly IHttpContextAccessor contxt;
		private readonly ILogger<LoginModel> logger;

		public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, ILogger<LoginModel> logger)
		{
			this.signInManager = signInManager;
			this.contxt = httpContextAccessor;
			this.logger = logger;
		}

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				if (ModelState.IsValid)
				{

					var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
					   LModel.RememberMe, false);
					if (identityResult.Succeeded)
					{
						var user = await signInManager.UserManager.FindByEmailAsync(LModel.Email);
						if (user != null)
						{
							contxt.HttpContext.Session.SetString("UserId", user.Email);
							contxt.HttpContext.Session.SetString("UserEmail", user.Email);
							contxt.HttpContext.Session.SetString("UserFirstName", user.FirstName);
							contxt.HttpContext.Session.SetString("UserLastName", user.LastName);
							contxt.HttpContext.Session.SetString("UserCreditCardNo", user.CreditCardNo);
							contxt.HttpContext.Session.SetString("UserMobileNo", user.MobileNo);
							contxt.HttpContext.Session.SetString("UserBillingAddress", user.BillingAddress);
							contxt.HttpContext.Session.SetString("UserShippingAddress", user.ShippingAddress);
						}
						return RedirectToPage("Index");
					}
					else
					{
						var errorDescription = identityResult.ToString();
						logger.LogError($"Identity error: {errorDescription}");
						ModelState.AddModelError("", errorDescription);
						ModelState.AddModelError("", "Username or Password incorrect");
						return Page();
					}
				}
				return Page();
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred during login: {ex.Message}");
				// Handle or rethrow the exception based on your application's requirements
				return Page();  // Add a return statement to handle the exception case
			}
		}

		private async Task<bool> ValidateRecaptchaAsync(string recaptchaToken)
		{
			var secretKey = "6Ldr7WEpAAAAANrEb_hVqkFt9JU5q3OhXYjBEHMz";

			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.GetStringAsync(
					$"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaToken}");

				var jsonDocument = JsonDocument.Parse(response);
				var successProperty = jsonDocument.RootElement.GetProperty("success");

				return successProperty.GetBoolean();
			}
		}
	}
}

