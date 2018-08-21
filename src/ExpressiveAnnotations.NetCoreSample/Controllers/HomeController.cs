using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpressiveAnnotations.NetCoreSample.Models;

namespace ExpressiveAnnotations.NetCoreSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
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
