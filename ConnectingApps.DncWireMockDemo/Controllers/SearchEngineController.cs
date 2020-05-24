using System.Threading.Tasks;
using ConnectingApps.DncWireMockDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.DncWireMockDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchEngineController : ControllerBase
    {
        private readonly ISearchEngineService _searchEngineService;
        private readonly ILogicHelper _logicHelper;
        private readonly ILogger<SearchEngineController> _logger;

        public SearchEngineController(ISearchEngineService searchEngineService, ILogicHelper logicHelper, ILogger<SearchEngineController> logger)
        {
            _searchEngineService = searchEngineService;
            _logicHelper = logicHelper;
            _logger = logger;
            _logger.LogCritical("Log Critical SearchEngineController");
        }

        [HttpGet("{queryEntry}", Name = "GetNumberOfCharacters")] // http://localhost:61712/api/searchengine/daan
        public async Task<ActionResult<int>> GetNumberOfCharacters(string queryEntry)
        {
            var numberOfCharacters = await _searchEngineService.GetNumberOfCharactersFromSearchQuery(queryEntry);
            return Ok(numberOfCharacters);
        }
    }
}
