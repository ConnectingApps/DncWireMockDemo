using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConnectingApps.DncWireMockDemo;
using ConnectingApps.DncWireMockDemo.Controllers;
using ConnectingApps.IntegrationFixture;
using ConnectingApps.IntegrationFixture.Shared.Customizers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Refit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ConnectingApps.IntegrationFixtureTests
{
    public class SearchEngineControllerTests
    {

        [Fact]
        public async Task GetTest()
        {
            await using (var fixture = new Fixture<Startup>())
            {
                using (var mockServer = fixture.FreezeServer("Google"))
                {
                    SetupStableServer(mockServer, "Response");
                    var controller = fixture.Create<SearchEngineController>();
                    var response = await controller.GetNumberOfCharacters("Hoi");

                    var request = mockServer.LogEntries.Select(a => a.RequestMessage).Single();
                    Assert.Contains("Hoi", request.RawQuery);
                    Assert.Equal(8, ((OkObjectResult)response.Result).Value);
                    Assert.Equal(2, fixture.LogSource.GetCriticals().Count());
                }
            }
        }


        [Fact]
        public async Task JsonTest()
        {
            await using (var fixture = new Fixture<Startup>())
            {
                string assemblyDirectoryName = Path.GetDirectoryName(typeof(SearchEngineController).Assembly.Location);
                // ReSharper disable once AssignNullToNotNullAttribute
                var jsonPath = Path.Combine(assemblyDirectoryName, "data.json");
                fixture.Customize(new JsonCustomizer(jsonPath));
                var controller = fixture.Create<SearchEngineController>();
                var configuration = fixture.Create<IConfiguration>();
                Assert.NotNull(controller);
                Assert.Equal("JsonDummyValue", configuration["JsonDummy"]);
            }
        }

        [Fact]
        public async Task GetTestWithJson()
        {
            await using (var fixture = new Fixture<Startup>())
            {
                using (var mockServer = fixture.FreezeServer("Google"))
                {
                    string assemblyDirectoryName = Path.GetDirectoryName(typeof(SearchEngineController).Assembly.Location);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var jsonPath = Path.Combine(assemblyDirectoryName, "data.json");
                    fixture.Customize(new JsonCustomizer(jsonPath));

                    SetupStableServer(mockServer, "Response");
                    var controller = fixture.Create<SearchEngineController>();

                    var response = await controller.GetNumberOfCharacters("Hoi");

                    var request = mockServer.LogEntries.Select(a => a.RequestMessage).Single();
                    Assert.Contains("Hoi", request.RawQuery);
                    Assert.Equal(8, ((OkObjectResult)response.Result).Value);
                    var configuration = fixture.Create<IConfiguration>();
                    Assert.Equal("JsonDummyValue", configuration["JsonDummy"]);
                }
            }
        }

        [Fact]
        public async Task GetViaRefitClient()
        {
            using (var fixture = new RefitFixture<Startup, ISearchEngine>(RestService.For<ISearchEngine>))
            {
                using (var mockServer = fixture.FreezeServer("Google"))
                {
                    SetupStableServer(mockServer, "Response");
                    var refitClient = fixture.GetRefitClient();
                    var response = await refitClient.GetNumberOfCharacters("Hoi");
                    await response.EnsureSuccessStatusCodeAsync();
                    var request = mockServer.LogEntries.Select(a => a.RequestMessage).Single();
                    Assert.Contains("Hoi", request.RawQuery);
                }
            }
        }

        private void SetupStableServer(FluentMockServer fluentMockServer, string response)
        {
            fluentMockServer.Given(Request.Create().UsingGet())
                .RespondWith(Response.Create().WithBody(response, encoding: Encoding.UTF8)
                    .WithStatusCode(HttpStatusCode.OK));
        }
    }
}
