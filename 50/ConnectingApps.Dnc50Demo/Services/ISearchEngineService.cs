using System.Threading.Tasks;

namespace ConnectingApps.Dnc50Demo.Services
{
    public interface ISearchEngineService
    {
        Task<int> GetNumberOfCharactersFromSearchQuery(string toSearchFor);
    }
}