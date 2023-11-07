using Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Authentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) 
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            return RedirectToAction("AccessDenied", "Account");
        }

        public IActionResult Privacy()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            return RedirectToAction("AccessDenied", "Account");

        }


    }
}