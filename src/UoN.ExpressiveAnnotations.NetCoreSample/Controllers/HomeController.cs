using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using UoN.ExpressiveAnnotations.NetCoreSample.Models;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Controllers
{
    public class HomeController : BaseController
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

            HttpContext.Session.SetInt32("Postbacks", (int?)TempData["Postbacks"] ?? 0);
            ViewBag.Success = TempData["Success"];

            try
            {
                return View("Home", JsonConvert.DeserializeObject<Query>((string) TempData["Query"]));
            }
            catch
            {
                return View("Home", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Index(Query model)
        {
            HttpContext.Session.SetInt32("Postbacks", (HttpContext.Session.GetInt32("Postbacks") ?? 0) + 1);

            // Suppress validation of this circular reference; for unknown reason, model binding is reporting
            // 'Unvalidated' for this even though it previously worked just fine. We don't need to validate
            // this field anyway because it's already going to be validated directly in the model.
            ModelState["ContactDetails.Parent.GoAbroad"].ValidationState = ModelValidationState.Skipped;

            if (ModelState.IsValid)
            {
                var result = await Save(model);
                if (!result.IsSuccessStatusCode)
                    throw new ApplicationException("Unexpected failure in WebAPI pipeline.");

                TempData["Postbacks"] = HttpContext.Session.GetInt32("Postbacks");
                TempData["Success"] = "[query successfully submitted]";
                TempData["Query"] = JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

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
                return await client.PostAsync("api/Default", model, formatter);
            }
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
