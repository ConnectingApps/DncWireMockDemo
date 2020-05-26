using System.Linq;

namespace ConnectingApps.IntegrationFixture.Shared
{
    internal static class Validator
    {
        public static void EnsureMockIsValid<TMock>()
        {
            var assemblyOfMock = typeof(TMock).Assembly.GetName().Name;
            var objectProperty = typeof(TMock).GetProperties().FirstOrDefault(p => p.Name == "Object");
            var typeOfMock = typeof(TMock).GetGenericArguments().FirstOrDefault();

            if (assemblyOfMock == "Moq" && objectProperty != null && typeOfMock != null &&
                objectProperty.PropertyType == typeOfMock)
            {
                return;
            }

            throw new IntegrationFixtureException(
                $"Invalid type: {typeof(TMock)}. Expected a Mock<Interterface> as used with Moq");
        }
    }
}