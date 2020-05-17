using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ConnectingApps.IntegrationFixture.Shared.Customizers
{
    public class JsonCustomizer : IConfigbuilderCustomizer
    {
        private readonly string _jsonFilePath;

        public JsonCustomizer(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath ?? throw new ArgumentNullException(nameof(jsonFilePath));
            if (!File.Exists(_jsonFilePath))
            {
                throw new FileNotFoundException(nameof(jsonFilePath));
            }
        }

        public void Customize(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddJsonFile(_jsonFilePath);
        }
    }
}