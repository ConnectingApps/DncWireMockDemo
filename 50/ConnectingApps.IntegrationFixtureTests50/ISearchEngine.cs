using System.Threading.Tasks;
using Refit;

namespace ConnectingApps.IntegrationFixtureTests50
{
    public interface ISearchEngine
    {
        [Get("/api/searchengine/{queryEntry}")]
        Task<ApiResponse<int>> GetNumberOfCharacters(string queryEntry);
    }
}
