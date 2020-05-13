using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace ConnectingApps.IntegrationFixture
{
    /// <summary>
    /// A fixture to create your dependencies.
    /// Before creating, you may need to call the "FreezeServer" method to setup the behaviour of your external server.
    /// </summary>
    /// <typeparam name="TStartup">The Startup class of your program</typeparam>
    public class Fixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly Dictionary<string, string> _configurationDictionary = new Dictionary<string, string>();
        private readonly IntegrationWebApplicationFactory<TStartup> _factory = new IntegrationWebApplicationFactory<TStartup>();
        private readonly Lazy<IServiceScope> _serviceScope;
        private ServiceProvider _serviceProvider;

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
                    _serviceProvider = sc.BuildServiceProvider();
                    serviceScope = _serviceProvider.CreateScope();
                });
            }).CreateClient().Dispose();
            return serviceScope;
        }


        /// <summary>
        /// Create a running WireMock Server and ensure your code will use it by changing the configuration parameter. You can now setup the behaviour of it: https://github.com/WireMock-Net/WireMock.Net/wiki/Stubbing#stubbing
        /// </summary>
        /// <param name="configurationParameter">This is typically something like "ExternalService:Url"</param>
        /// <returns>The running and self-hosted WireMock server</returns>
        public FluentMockServer FreezeServer(string configurationParameter)
        {
            var server = FluentMockServer.Start();
            var url = server.Urls.Single();
            _configurationDictionary.Add(configurationParameter, url);
            return server;
        }


        /// <summary>
        /// Create an instance of something you want to test
        /// This has to be a Controller 
        /// </summary>
        /// <typeparam name="TTestType">A Controller class or some type (typically an interface) that can be resolved from the .NET Core DI Framework</typeparam>
        /// <returns>An instance of the class you need to test </returns>
        public TTestType Create<TTestType>() where TTestType : class
        {
            return _serviceScope.Value.ServiceProvider.GetService<TTestType>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _factory.Dispose();
                if (_serviceScope.IsValueCreated)
                {
                    _serviceProvider.Dispose();
                    _serviceScope.Value.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}