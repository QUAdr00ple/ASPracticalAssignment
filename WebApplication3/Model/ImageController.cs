using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;

namespace WebApplication3.Model
{
    public class ImageController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        public ImageController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetImage(string userId)
        {
            var user = userManager.FindByIdAsync(userId).Result;
            if (user != null && user.Photo != null)
            {
                return File(user.Photo, "image/jpeg"); // Adjust the content type based on your image type
            }
            else
            {
                // Return a default image or a placeholder if the user doesn't have a photo
                return File("~/path/to/placeholder.jpg", "image/jpeg");
            }
        }

    }
}
