using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

public class ResetPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IHttpContextAccessor contxt;
    private readonly PasswordHistoryService passwordHistoryService;

    [BindProperty]
    public ResetPassword RPModel { get; set; }

    public ResetPasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contxt, PasswordHistoryService passwordHistoryService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.contxt = contxt;
        this.passwordHistoryService = passwordHistoryService;
    }


    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string userId, string code)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

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

        var result = await userManager.ResetPasswordAsync(user, code, RPModel.Password);

        if (result.Succeeded)
        {
            var recentPasswords = await passwordHistoryService.GetRecentPasswordsAsync(user);

            if (recentPasswords.Contains(RPModel.Password))
            {
                ModelState.AddModelError(string.Empty, "Cannot use a recent password. Please choose a different one.");
                return Page();
            }
            return RedirectToPage("Login");
        }

        return Page();
    }
}

