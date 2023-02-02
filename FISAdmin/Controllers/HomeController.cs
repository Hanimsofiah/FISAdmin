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
            string sql = "";

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

            switch (shortform)
            {
                case "BCB":
                    sql = "billing_category_bill";
                    break;
                case "BCC":
                    sql = "billing_customer_category";
                    break;
                case "BDT":
                    sql = "billing_doc_type";
                    break;
                case "BFD":
                    sql = "billing_fis_destination";
                    break;
                case "BP":
                    sql = "billing_priority";
                    break;
                case "BSCB":
                    sql = "billing_source_bill";
                    break;
                case "BSTB":
                    sql = "billing_status_bill";
                    break;
                default:
                    break;
            }

            switch (shortform)
            {
                case "PDT":
                    sql = "payment_doc_type";
                    break;
                case "PSC":
                    sql = "payment_source";
                    break;
                case "PST":
                    sql = "payment_status";
                    break;
                case "PT":
                    sql = "payment_type";
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