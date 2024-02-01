using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;
using WebApplication3.ViewModels;

public class ChangePasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IHttpContextAccessor contxt;
    private readonly PasswordHistoryService passwordHistoryService;

    [BindProperty]
    public ChangePassword CPModel { get; set; }

    public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contxt, PasswordHistoryService passwordHistoryService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.contxt = contxt;
        this.passwordHistoryService = passwordHistoryService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string currentPassword, string newPassword, string confirmPassword)
    {
        var user = await userManager.FindByEmailAsync(contxt.HttpContext.Session.GetString("UserEmail"));

        if (user == null)
        {
            return RedirectToPage("/Index");
        }

        // Check minimum password age
        var recentDate = await passwordHistoryService.GetLastPasswordChangeDateAsync(user);

        if (recentDate.HasValue)
        {
            if (recentDate.Value != DateTime.MinValue)
            {
                var minPasswordAge = TimeSpan.FromMinutes(30); // Set the minimum password age
                var timeSinceLastChange = DateTime.UtcNow - recentDate.Value;

                if (timeSinceLastChange < minPasswordAge)
                {
                    ModelState.AddModelError(string.Empty, $"Cannot change password within {minPasswordAge.TotalMinutes} minutes from the last change.");
                    return Page();
                }
            }
        }


        var signInResult = await signInManager.PasswordSignInAsync(user, CPModel.CurrentPassword, false, false);

        if (signInResult.Succeeded)
        {
            // The current password is correct, proceed with checking if it is the same as recent passwords
            var recentPasswords = await passwordHistoryService.GetRecentPasswordsAsync(user);

            if (recentPasswords.Contains(CPModel.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "Cannot use a recent password. Please choose a different one.");
                return Page();
            }
            var changePasswordResult = await userManager.ChangePasswordAsync(user, CPModel.CurrentPassword, CPModel.NewPassword);

            if (changePasswordResult.Succeeded)
            {
                // Password changed successfully
                return RedirectToPage("/Login");
            }
            else
            {
            }
        }
        else
        {
            // The current password is incorrect
            ModelState.AddModelError(string.Empty, "Current password is incorrect");
            return Page();
        }
    }
}
