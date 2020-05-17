using Microsoft.Extensions.Configuration;

namespace ConnectingApps.IntegrationFixture.Shared.Customizers
{
    public interface IConfigbuilderCustomizer
    {
        void Customize(IConfigurationBuilder configurationBuilder);
    }
}