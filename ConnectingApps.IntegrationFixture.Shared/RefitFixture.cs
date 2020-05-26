using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectingApps.IntegrationFixture
{
    public class RefitFixture<TStartup, TRefitClient> : FixtureBase<TStartup> where TRefitClient : class where TStartup : class
    {
        private readonly int _portNumber;
        private readonly bool _useHttps;
        private readonly Lazy<(HttpClient HttpClient, TRefitClient RefitClient)> _refit;

        public RefitFixture(Func<HttpClient, TRefitClient> refitClientCreator, int portNumber = 5392, bool useHttps = false)
        {
            _portNumber = portNumber;
            _useHttps = useHttps;
            _refit = new Lazy<(HttpClient httpClient, TRefitClient refitClient)>(() =>
            {
                var httpClient = CreateHttpClient();
                var refitClient = refitClientCreator(httpClient);
                return (httpClient, refitClient);
            });
        }

        public HttpClient CreateHttpClient()
        {
            string afterHttp = _useHttps ? "s" : "";
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
                BaseAddress = new Uri($"http{afterHttp}://localhost:{_portNumber}")
            });
        }

        public TRefitClient GetRefitClient()
        {
            return _refit.Value.RefitClient;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _factory.Dispose();
                if (_refit.IsValueCreated)
                {
                    _refit.Value.HttpClient.Dispose();
                }
            }
        }


    }
}