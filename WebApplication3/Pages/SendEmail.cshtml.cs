using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

public class SendEmailModel : PageModel
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly EmailService emailSender;

    [BindProperty]
    public SendEmail EModel { get; set; }

    public SendEmailModel(UserManager<ApplicationUser> userManager, EmailService emailSender)
    {
        this.userManager = userManager;
       this.emailSender = emailSender;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(EModel.Email);

            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                // Update the callbackUrl to point to the ResetPassword page
                var callbackUrl = Url.Page("/ResetPassword", pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = token }, protocol: Request.Scheme);

                var emailBody = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                await emailSender.SendEmailAsync(EModel.Email, "Reset Password", emailBody);

                return RedirectToPage("Login");
            }

            // Don't reveal that the user does not exist or is not confirmed
            return RedirectToPage("Login");
        }

        // If we got this far, something failed, redisplay the form
        return Page();
    }
}
