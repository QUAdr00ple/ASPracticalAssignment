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
		private readonly PasswordHistoryService passwordHistoryService;
		private readonly AuditLogService _auditLogService;
		public LoginModel(
			SignInManager<ApplicationUser> signInManager,
			IHttpContextAccessor httpContextAccessor,
			ILogger<LoginModel> logger,
			AuditLogService auditLogService,
			PasswordHistoryService passwordHistoryService) 
		{
			this.signInManager = signInManager;
			this.contxt = httpContextAccessor;
			this.logger = logger;
			_auditLogService = auditLogService;
			this.passwordHistoryService = passwordHistoryService;
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
					var lockeduser = await signInManager.UserManager.FindByEmailAsync(LModel.Email);
					if (lockeduser != null && await signInManager.UserManager.IsLockedOutAsync(lockeduser))
					{
						ModelState.AddModelError("", "Account is locked out. Please try again later.");
						return Page();
					}

					var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
					   LModel.RememberMe, false);
					if (identityResult.Succeeded)
					{
						var user = await signInManager.UserManager.FindByEmailAsync(LModel.Email);

                        // Check if the password has been the same for more than 60 minutes
                        var recentDate = await passwordHistoryService.GetLastPasswordChangeDateAsync(user);

                        if (recentDate.HasValue)
                        {
                            if (recentDate.Value != DateTime.MinValue)
                            {
                                var maxPasswordAge = TimeSpan.FromMinutes(60); // Set the minimum password age
                                var timeSinceLastChange = DateTime.UtcNow - recentDate.Value;

                                if (timeSinceLastChange > maxPasswordAge)
                                {
                                    ModelState.AddModelError(string.Empty, $"Password has expired. Please change your password. Time Since Last Change: {timeSinceLastChange}. Current UTC Time: {DateTime.UtcNow}. Recent Date: {recentDate}");
                                    await signInManager.SignOutAsync();
                                    return Page();
                                }
                            }
                        }
                        if (user != null)
						{
                            contxt.HttpContext.Session.SetString("UserEmail", user.Email);
							contxt.HttpContext.Session.SetString("UserFirstName", user.FirstName);
							contxt.HttpContext.Session.SetString("UserLastName", user.LastName);
							contxt.HttpContext.Session.SetString("UserCreditCardNo", user.CreditCardNo);
							contxt.HttpContext.Session.SetString("UserMobileNo", user.MobileNo);
							contxt.HttpContext.Session.SetString("UserBillingAddress", user.BillingAddress);
							contxt.HttpContext.Session.SetString("UserShippingAddress", user.ShippingAddress);
						}
						_auditLogService.LogLogin(user.Id);
						return RedirectToPage("Index");
					}
					else
					{
						// Increment the failed login attempts
						await signInManager.UserManager.AccessFailedAsync(lockeduser);

						if (await signInManager.UserManager.IsLockedOutAsync(lockeduser))
						{
							ModelState.AddModelError("", "Account is locked out. Please try again later.");
							return Page();
						}

						var errorDescription = identityResult.ToString();
						logger.LogError($"Identity error: {errorDescription}");
						ModelState.AddModelError("", "Username or Password incorrect");
						_auditLogService.LogLoginFailed(lockeduser.Id);
						return Page();
					}
				}
				return Page();
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred during login: {ex.Message}");
				return Page();  
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

