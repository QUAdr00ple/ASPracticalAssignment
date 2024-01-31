using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.ViewModels;
using WebApplication3.Model;
using Microsoft.AspNetCore.DataProtection;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {

        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
				var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
				var protector = dataProtectionProvider.CreateProtector("PracticalAssignment2");

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
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }


    }
}
