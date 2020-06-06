using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace ConnectingApps.IntegrationFixture.Shared.GraphQl
{
    public class GraphQlFixture<TStartup> : FixtureBase<TStartup> where TStartup : class
    {
        private readonly Lazy<GraphQlClient> _graphQlClient;
        private readonly Lazy<HttpClient> _httpClient;

        /// <summary>
        /// Create a GraphQlFixture
        /// </summary>
        /// <param name="uriPath">Not the full uri, typically '/playground/..' or '/graphql' </param>
        /// <param name="queryObjectNameInRequest">Typically '/query'</param>
        /// <param name="portNumber"></param>
        /// <param name="useHttps"></param>
        public GraphQlFixture(string uriPath, string queryObjectNameInRequest = "query", int portNumber = 5392, bool useHttps = false)
        {
            _httpClient = new Lazy<HttpClient>(() => CreateHttpClient(useHttps, portNumber)); 
            _graphQlClient = new Lazy<GraphQlClient>(() => new GraphQlClient(_httpClient.Value, uriPath, queryObjectNameInRequest));
        }

        private HttpClient CreateHttpClient(bool useHttps, int portNumber)
        {
            string afterHttp = useHttps ? "s" : "";
            return _factory.WithWebHostBuilder(whb =>
            {
                whb.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    foreach (var configbuilderCustomizer in _configbuilderCustomizers)
                    {
                        configbuilderCustomizer.Customize(configBuilder);
                    }
                });
                whb.ConfigureTestServices(sc =>
                {
                    foreach (var mockedObject in _mockedObjects)
                    {
                        ReplaceDependency(sc, mockedObject.Value.MockType, mockedObject.Value.MockObject);
                    }
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri($"http{afterHttp}://localhost:{portNumber}")
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _factory.Dispose();
                if (_httpClient.IsValueCreated)
                {
                    _httpClient.Value.Dispose();
                }
            }
        }

        public GraphQlClient GetClient()
        {
            return _graphQlClient.Value;
        }
    }
}
