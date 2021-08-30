using System;
using System.Threading.Tasks;

namespace ConnectingApps.IntegrationFixture
{
    public partial class Fixture<TStartup> : IDisposable, IAsyncDisposable where TStartup : class
    {
        public async ValueTask DisposeAsync()
        {
            _factory.Dispose();
            if (_serviceScope.IsValueCreated)
            {
                await _serviceProvider.DisposeAsync();
                _serviceScope.Value.Dispose();
            }
        }
    }
}
