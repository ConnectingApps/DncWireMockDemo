using System.IO;
using ConnectingApps.Dnc50Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.Dnc50Demo.Controllers
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

            _logger.LogInformation("This is the input input {name}", value);
            _logger.LogError(new InvalidDataException("Some exception message"), "Some Exception");

            return Ok($"{value.FirstName} {value.MiddleName} {value.LastName}");
        }
    }
}