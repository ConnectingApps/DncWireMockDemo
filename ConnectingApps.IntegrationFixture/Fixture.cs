using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace ConnectingApps.IntegrationFixture
{
    public class Fixture<TStartup> where TStartup : class
    {
        private readonly Dictionary<string, string> _configurationDictionary = new Dictionary<string, string>();
        private readonly IntegrationWebApplicationFactory<TStartup> _factory = new IntegrationWebApplicationFactory<TStartup>();
        private readonly Lazy<IServiceScope> _serviceScope;

        public Fixture()
        {
            _serviceScope = new Lazy<IServiceScope>(CreateServiceScope);
        }

        private IServiceScope CreateServiceScope()
        {
            IServiceScope serviceScope = null;
            _factory.WithWebHostBuilder(whb =>
            {
                whb.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(_configurationDictionary);
                });
                whb.ConfigureTestServices(sc =>
                {
                    serviceScope = sc.BuildServiceProvider().CreateScope();
                });
            }).CreateClient().Dispose();
            return serviceScope;
        }

        public FluentMockServer FreezeServer(string configurationParameter)
        {
            var server = FluentMockServer.Start();
            var url = server.Urls.Single();
            _configurationDictionary.Add(configurationParameter, url);
            return server;
        }

        public TTestType Create<TTestType>() where TTestType : class
        {
            return _serviceScope.Value.ServiceProvider.GetService<TTestType>();
        }
    }
}