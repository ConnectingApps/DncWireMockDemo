using System.Threading.Tasks;
using ConnectingApps.DncWireMockDemo.Models;
using Refit;

namespace ConnectingApps.IntegrationFixtureTests
{
    public interface ILogicClient
    {
        [Post("/api/logic")]
        Task<ApiResponse<string>> Post([Body] Name name);

        [Put("/api/logic")]
        Task<ApiResponse<string>> Put([Body] Name name);
    }
}
