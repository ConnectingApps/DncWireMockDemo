using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConnectingApps.DncWireMockDemo.IntegrationTests
{
    public abstract class TestBase : IDisposable, IClassFixture<WebApplicationFactory<Startup>>
    {
        protected readonly HttpClient HttpClient;

        public TestBase(WebApplicationFactory<Startup> factory, int portNumber, bool useHttps)
        {
            var extraConfiguration = GetConfiguration();
            string afterHttp = useHttps ? "s" : "";
            HttpClient = factory.WithWebHostBuilder(whb =>
            {
                whb.ConfigureAppConfiguration((context, configbuilder) =>
                {
                    configbuilder.AddInMemoryCollection(extraConfiguration);
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri($"http{afterHttp}://localhost:{portNumber}")
            });
        }

        protected virtual Dictionary<string, string> GetConfiguration()
        {
            return new Dictionary<string, string>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpClient.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
