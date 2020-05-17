using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConnectingApps.DncWireMockDemo;
using ConnectingApps.DncWireMockDemo.Controllers;
using ConnectingApps.IntegrationFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
