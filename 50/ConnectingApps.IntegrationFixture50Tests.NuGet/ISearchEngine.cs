using System.Threading.Tasks;
using Refit;

namespace ConnectingApps.IntegrationFixture50Tests.NuGet
{
    public interface ISearchEngine
    {
        [Get("/api/searchengine/{queryEntry}")]
        Task<ApiResponse<int>> GetNumberOfCharacters(string queryEntry);
    }
}
