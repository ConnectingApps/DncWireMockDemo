using System.Threading.Tasks;
using ConnectingApps.Dnc60Demo.Models;
using Refit;

namespace ConnectingApps.IntegrationFixtureTests60
{
    public interface ILogicClient
    {
        [Post("/api/logic")]
        Task<ApiResponse<string>> Post([Body] Name name);

        [Put("/api/logic")]
        Task<ApiResponse<string>> Put([Body] Name name);
    }
}
