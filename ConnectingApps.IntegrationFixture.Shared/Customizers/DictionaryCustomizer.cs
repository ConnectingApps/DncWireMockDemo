using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ConnectingApps.IntegrationFixture.Shared.Customizers
{
    public class DictionaryCustomizer : IConfigbuilderCustomizer
    {
        private readonly IDictionary<string, string> _customizationDictionary;

        public DictionaryCustomizer(IDictionary<string,string> customizationDictionary)
        {
            _customizationDictionary = customizationDictionary ??
                                       throw new ArgumentNullException(nameof(customizationDictionary));
        }

        public void Customize(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddInMemoryCollection(_customizationDictionary);
        }
    }
}
