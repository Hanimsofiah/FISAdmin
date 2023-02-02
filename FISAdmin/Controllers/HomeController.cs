using FISAdmin.Areas.Identity.Data;
using FISAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace FISAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            if(userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
                /*return View();*/
            }
            else
            {
                /*ApplicationUser user = _userManager.FindByIdAsync(userId).Result;
                return View(user);*/
                return View();
            }
        }

        public IActionResult Privacy(string type, string shortform)
        {

            switch (shortform)
            {
                case "SA":
                   
                    break;
                case "SKP":

                    break;
                case "SP":
                    
                    break;
                case "SS":

                    break;
                default:
                    break;
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}