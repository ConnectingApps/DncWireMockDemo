using System;
using System.Collections.Generic;
using System.Linq;
using ConnectingApps.IntegrationFixture.Shared.Customizers;
using WireMock.Server;

namespace ConnectingApps.IntegrationFixture
{
    public partial class FixtureBase<TStartup> : IDisposable where TStartup : class
    {
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
    }
}
