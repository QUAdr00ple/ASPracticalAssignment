using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.ViewModels;
using WebApplication3.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AuditLogService auditLogService;
        private readonly IHttpContextAccessor contxt;
        private readonly IDataProtector protector;  

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AuditLogService auditLogService,
            IHttpContextAccessor contxt,
            IDataProtector protector)  
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.auditLogService = auditLogService;
            this.contxt = contxt;
            this.protector = protector;
        }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

				var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
					FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					CreditCardNo = protector.Protect(RModel.CreditCardNo),
					MobileNo = RModel.MobileNo,
					BillingAddress = RModel.BillingAddress,
					ShippingAddress = RModel.ShippingAddress,
				};
				if (RModel.Photo != null)
				{
					try
					{
						using (var memoryStream = new MemoryStream())
						{
							await RModel.Photo.CopyToAsync(memoryStream);
							user.Photo = memoryStream.ToArray();
						}
					}
					catch (Exception ex)
					{
						// Log the exception details
						Console.WriteLine($"Error copying photo to user object: {ex.Message}");
						ModelState.AddModelError("", "Error copying photo. Please try again.");
						return Page();
					}
				}
				var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
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
                    auditLogService.LogRegister(user.Id);
					await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
            }
            return Page();
        }


    }
}
