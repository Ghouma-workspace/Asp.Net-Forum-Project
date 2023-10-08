using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyForum.BL.Entities;
using MyForum.WEB.Models;
using System.Diagnostics;

namespace MyForum.WEB.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private SignInManager<User> _signInManager;

        public HomeController(SignInManager<User> signInMgr)
        {
            _signInManager = signInMgr;
        }

        /*
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        */

        public IActionResult Index()
        {
            if (!_signInManager.IsSignedIn(User))
                return Redirect("/Identity/Account/Login");
            return Redirect("/Blogs");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}