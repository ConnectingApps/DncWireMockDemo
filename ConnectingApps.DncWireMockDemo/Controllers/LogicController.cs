using ConnectingApps.DncWireMockDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectingApps.DncWireMockDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogicController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> Post([FromBody] Name value)
        {
            return Ok($"{value.FirstName} {value.MiddleName} {value.LastName}");
        }
    }
}