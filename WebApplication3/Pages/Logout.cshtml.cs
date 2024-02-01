using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace WebApplication3.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHttpContextAccessor contxt;
		private readonly AuditLogService _auditLogService;

		public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, AuditLogService auditLogService)
		{
			this.signInManager = signInManager;
			this.contxt = httpContextAccessor;
			_auditLogService = auditLogService;
		}
		public void OnGet() { }
		public async Task<IActionResult> OnPostLogoutAsync()
		{
			_auditLogService.LogLogout(contxt.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
			await signInManager.SignOutAsync();
            contxt.HttpContext.Session.Clear();
            return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
