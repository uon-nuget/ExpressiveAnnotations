using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UoN.ExpressiveAnnotations.NetCoreSample.Misc;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Controllers
{
    public abstract class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var culture = CultureManager.Instance.Load(HttpContext);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = culture;

            var validation = ValidationManager.Instance.Load(HttpContext);
            ViewBag.Validation = validation;

            var indication = IndicationsManager.Instance.Load(HttpContext);
            ViewBag.Indication = indication;

            var triggers = TriggersManager.Instance.Load(HttpContext);
            ViewBag.Triggers = triggers;

            var verbose = VerbosityManager.Instance.Load(HttpContext);
            ViewBag.Verbose = verbose;
        }
    }
}
