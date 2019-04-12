using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using webmonitor.Models;

namespace webmonitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration Config;

        public HomeController(IConfiguration config)
        {
            Config = config;
        }

        public IActionResult Index()
        {
            ViewData["Assets"] = Config["Assets"];
            ViewData["WelcomeMessage"] = Config["WelcomeMessage"];
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
