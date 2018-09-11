using System.Threading.Tasks;
using ExpressiveAnnotations.NetCoreSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpressiveAnnotations.NetCoreSample.Controllers
{
    [Route("api/Default")]
    public class DefaultController : ControllerBase
    {
        // POST api/Default/Save
        [HttpPost]
        public async Task<ActionResult> Save(Query model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await Task.Delay(1);
            return Ok();
        }
    }
}
