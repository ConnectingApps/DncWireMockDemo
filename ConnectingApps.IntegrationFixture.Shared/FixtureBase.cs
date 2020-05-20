using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectingApps.IntegrationFixture.Shared.Customizers;
using WireMock.Server;

namespace ConnectingApps.IntegrationFixture
{
    public class FixtureBase<TStartup> : IDisposable where TStartup : class
    {
        protected readonly IntegrationWebApplicationFactory<TStartup> _factory = new IntegrationWebApplicationFactory<TStartup>();
        protected readonly List<IConfigbuilderCustomizer> _configbuilderCustomizers = new List<IConfigbuilderCustomizer>();

        public FluentMockServer FreezeServer(string configurationParameter)
        {
            var server = FluentMockServer.Start();
            var url = server.Urls.Single();
            Customize(new DictionaryCustomizer(new Dictionary<string, string>()
            {
                {configurationParameter,url}
            }));
            return server;
        }

        public void Customize(IConfigbuilderCustomizer configbuilderCustomizer)
        {
            _configbuilderCustomizers.Add(configbuilderCustomizer);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _factory.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
