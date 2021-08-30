using System.Threading.Tasks;

namespace ConnectingApps.Dnc60Demo.Services
{
    public interface ISearchEngineService
    {
        Task<int> GetNumberOfCharactersFromSearchQuery(string toSearchFor);
    }
}