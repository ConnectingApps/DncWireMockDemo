using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConnectingApps.Dnc50Demo;
using ConnectingApps.Dnc50Demo.Controllers;
using ConnectingApps.Dnc50Demo.Services;
using ConnectingApps.IntegrationFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Refit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ConnectingApps.IntegrationFixtureTests50
{
    public class SearchEngineControllerTests
    {

        [Fact]
        public async Task GetTest()
        {
            // arrange
            await using (var fixture = new Fixture<Startup>())
            {
                using (var mockServer = fixture.FreezeServer("Google"))
                {
                    SetupStableServer(mockServer, "Response");
                    var controller = fixture.Create<SearchEngineController>();

                    // act
                    var response = await controller.GetNumberOfCharacters("Hoi");

                    // assert
                    var request = mockServer.LogEntries.Select(a => a.RequestMessage).Single();
                    Assert.Contains("Hoi", request.RawQuery);
                    Assert.Equal(8, ((OkObjectResult)response.Result).Value);
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

        [Fact]
        public async Task GetViaRefitClientFreeze()
        {
            using (var fixture = new RefitFixture<Startup, ISearchEngine>(RestService.For<ISearchEngine>))
            {
                var service = fixture.Freeze<Mock<ISearchEngineService>>();
                service.Setup(a => a.GetNumberOfCharactersFromSearchQuery(It.IsNotNull<string>()))
                    .ReturnsAsync(8);

                var refitClient = fixture.GetRefitClient();
                var response = await refitClient.GetNumberOfCharacters("Hoi");
                await response.EnsureSuccessStatusCodeAsync();
                service.Verify(s => s.GetNumberOfCharactersFromSearchQuery("Hoi"), Times.Once);
            }
        }

        [Fact]
        public async Task GetTestFreeze()
        {
            // arrange
            await using (var fixture = new Fixture<Startup>())
            {
                var service = fixture.Freeze<Mock<ISearchEngineService>>();
                service.Setup(a => a.GetNumberOfCharactersFromSearchQuery(It.IsNotNull<string>()))
                    .ReturnsAsync(8);

                var controller = fixture.Create<SearchEngineController>();

                // act
                var response = await controller.GetNumberOfCharacters("Hoi");

                // assert
                Assert.Equal(8, ((OkObjectResult)response.Result).Value);
                service.Verify(s => s.GetNumberOfCharactersFromSearchQuery("Hoi"), Times.Once);
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
