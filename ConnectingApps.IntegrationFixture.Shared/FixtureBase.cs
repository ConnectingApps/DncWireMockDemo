using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectingApps.IntegrationFixture.Shared;
using ConnectingApps.IntegrationFixture.Shared.Customizers;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace ConnectingApps.IntegrationFixture
{
    public partial class FixtureBase<TStartup> : IDisposable where TStartup : class
    {
        protected readonly IntegrationWebApplicationFactory<TStartup> _factory = new IntegrationWebApplicationFactory<TStartup>();
        protected readonly List<IConfigbuilderCustomizer> _configbuilderCustomizers = new List<IConfigbuilderCustomizer>();
        protected readonly Dictionary<string, (Type MockType, object MockObject)> _mockedObjects = new Dictionary<string, (Type MockType, object MockObject)>();


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

        /// <summary>
        /// Creates a Moq instance to freeze in the DI. The TMock needs be like Mock&#60;InterfaceToMock&#62;
        /// </summary>
        /// <typeparam name="TMock">The TMock needs be like Mock&#60;InterfaceToMock&#62;</typeparam>
        /// <returns>The Mock instance</returns>
        public TMock Freeze<TMock>() where TMock : class
        {
            Validator.EnsureMockIsValid<TMock>();
            var mock = Activator.CreateInstance<TMock>();
            var instance = ((dynamic)mock).Object;
            var typeOfMock = typeof(TMock).GetGenericArguments().FirstOrDefault();
            var typeNameOfMock = $"{typeOfMock}";

            if (!_mockedObjects.TryAdd(typeNameOfMock, (typeOfMock, instance)))
            {
                throw new IntegrationFixtureException($"Type {typeOfMock} already frozen");
            }

            return mock;
        }

        protected static void ReplaceDependency(IServiceCollection services, Type typeToReplace, object replacer)
        {
            var serviceDescriptor =
                services.FirstOrDefault(descriptor => descriptor.ServiceType == typeToReplace);
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }
            services.AddSingleton(typeToReplace, replacer);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
