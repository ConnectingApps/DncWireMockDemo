using System.Threading.Tasks;

namespace ConnectingApps.DncWireMockDemo.Services
{
    public interface ISearchEngineService
    {
        Task<int> GetNumberOfCharactersFromSearchQuery(string toSearchFor);
    }
}