using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using new_websub.Models;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
namespace new_websub.Controllers
{
    public class HomeController : Controller
    {


        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "",
            BasePath = ""
        };
        IFirebaseClient client;

        public IActionResult Index()// henry (observing) comes here first because of routing on startup.cs
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("talentsID");

            HttpContext.Session.SetString("TalentsID", response.Body.ToString());
            return View();
        }

        public IActionResult About()// comes here because of the action on the button in layout page navbar
        {
            ViewData["Message"] = "Your application description page.";
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
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
