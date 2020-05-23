using System;
using System.Collections.Generic;
using System.Linq;
using ConnectingApps.IntegrationFixture.Logging;
using ConnectingApps.IntegrationFixture.Shared.Customizers;
using ConnectingApps.IntegrationFixture.Shared.Logging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WireMock.Server;

namespace ConnectingApps.IntegrationFixture
{
    /// <summary>
    /// A fixture to create your dependencies.
    /// Before creating, you may need to call the "FreezeServer" method to setup the behaviour of your external server.
    /// </summary>
    /// <typeparam name="TStartup">The Startup class of your program</typeparam>
    public partial class Fixture<TStartup> : FixtureBase<TStartup> where TStartup : class
    {
        private readonly Lazy<IServiceScope> _serviceScope;
        private ServiceProvider _serviceProvider;

        public ILogSource LogSource { get; } = new LogSource();

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
                    foreach (var configbuilderCustomizer in _configbuilderCustomizers)
                    {
                        configbuilderCustomizer.Customize(configBuilder);
                    }
                });
                whb.ConfigureTestServices(sc =>
                {
                    _serviceProvider = sc.BuildServiceProvider();
                    serviceScope = _serviceProvider.CreateScope();
                });
            }).CreateClient().Dispose();

            var loggerFactory = serviceScope.ServiceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new TestLoggerProvider(LogSource));
            return serviceScope;
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_serviceScope.IsValueCreated)
                {
                    _serviceProvider.Dispose();
                    _serviceScope.Value.Dispose();
                }
            }
        }
    }
}