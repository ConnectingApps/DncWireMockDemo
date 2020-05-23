using ConnectingApps.DncWireMockDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.DncWireMockDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogicController : ControllerBase
    {
        private readonly ILogger<LogicController> _logger;

        public LogicController(ILogger<LogicController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] Name value)
        {
            return Ok($"{value.FirstName} {value.MiddleName} {value.LastName}");
        }

        [HttpPut]
        public ActionResult<string> Put([FromBody] Name value)
        {
            _logger.LogWarning("Warning Logged");

            _logger.LogInformation("This is the input {name}", value);

            return Ok($"{value.FirstName} {value.MiddleName} {value.LastName}");
        }
    }
}