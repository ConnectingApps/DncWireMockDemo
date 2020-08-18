using System.Threading.Tasks;
using ConnectingApps.Dnc50Demo.Models;
using Refit;

namespace ConnectingApps.IntegrationFixture50Tests.NuGet
{
    public interface ILogicClient
    {
        [Post("/api/logic")]
        Task<ApiResponse<string>> Post([Body] Name name);

        [Put("/api/logic")]
        Task<ApiResponse<string>> Put([Body] Name name);
    }
}
