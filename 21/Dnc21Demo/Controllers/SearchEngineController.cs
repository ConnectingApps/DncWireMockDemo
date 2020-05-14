using System.Threading.Tasks;
using ConnectingApps.DncWireMockDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dnc21Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchEngineController : ControllerBase
    {
        private readonly ISearchEngineService _searchEngineService;

        public SearchEngineController(ISearchEngineService searchEngineService)
        {
            _searchEngineService = searchEngineService;
        }

        [HttpGet("{queryEntry}", Name = "GetNumberOfCharacters")] // http://localhost:61712/api/searchengine/daan
        public async Task<ActionResult<int>> GetNumberOfCharacters(string queryEntry)
        {
            var numberOfCharacters = await _searchEngineService.GetNumberOfCharactersFromSearchQuery(queryEntry);
            return Ok(numberOfCharacters);
        }
    }
}
