using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConnectingApps.IntegrationFixture.Shared.GraphQl
{
    public class GraphQlClient
    {
        private readonly HttpClient _client;
        private readonly string _uriPath;
        private readonly string _queryObjectName;

        internal GraphQlClient(HttpClient client, string uriPath, string queryObjectName = "query")
        {
            _client = client;
            _uriPath = uriPath;
            _queryObjectName = queryObjectName;
        }

        public async Task<GraphQlResponse> ExecuteQuery(string graphQlQuery)
        {
            var dataObject = new Dictionary<string, string>()
            {
                {_queryObjectName,graphQlQuery}
            };

            var theString = JsonConvert.SerializeObject(dataObject);
            var stringContent = new StringContent(theString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_uriPath, stringContent);
            var content = await response.Content.ReadAsStringAsync();
            return new GraphQlResponse
            {
                ResponseContent = content,
                StatusCode = response.StatusCode
            };
        }


    }
}
