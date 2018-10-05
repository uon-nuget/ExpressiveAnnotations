using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UoN.ExpressiveAnnotations.NetCoreSample.Models;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new Query
            {
                GoAbroad = true,
                Country = "Poland",
                NextCountry = "Other",
                SportType = "Extreme",
                AgreeForContact = false,
                SelectedCurrencies = new List<bool> { false, false },
                LatestSuggestedReturnDate = DateTime.Today.AddMonths(1)
            };

            //Session["Postbacks"] = (int?)TempData["Postbacks"] ?? 0;
            ViewBag.Success = TempData["Success"];
            return View("Home", TempData["Query"] as Query ?? model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(Query model)
        {
            //Session["Postbacks"] = (int)Session["Postbacks"] + 1;
            if (ModelState.IsValid)
            {
                var result = await Save(model);
                if (!result.IsSuccessStatusCode)
                    throw new ApplicationException("Unexpected failure in WebAPI pipeline.");

                //TempData["Postbacks"] = Session["Postbacks"];
                TempData["Success"] = "[query successfully submitted]";
                TempData["Query"] = model;
                return RedirectToAction("Index"); // PRG to avoid subsequent form submission attempts on page refresh (http://en.wikipedia.org/wiki/Post/Redirect/Get)
            }

            return View("Home", model);
        }

        private async Task<HttpResponseMessage> Save(Query model) // simulate saving through WebAPI call
        {
            using (var client = new HttpClient())
            {
                Debug.Assert(Request.GetDisplayUrl() != null);
                client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}/");
                var formatter = new JsonMediaTypeFormatter
                {
                    SerializerSettings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
                };
                return await client.PostAsync("api/Default/Save", model, formatter);
            }
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
