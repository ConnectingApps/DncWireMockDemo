using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ConnectingApps.DncWireMockDemo.IntegrationTests
{
    public class SearchEngineClientTest : TestBase
    {
        private FluentMockServer _mockServerSearchEngine;

        public SearchEngineClientTest(WebApplicationFactory<Startup> factory) : base(factory, 5347, false)
        {
        }

        [Theory]
        [InlineData("Daan","SomeResponseFromGoogle")]
        [InlineData("Sean","SomeOtherResponseFromGoogle")]
        public async Task TestWithStableServer(string searchQuery, string externalResponseContent)
        {
            SetupStableServer(externalResponseContent);
            var response = await HttpClient.GetAsync($"/api/searchengine/{searchQuery}");
            response.EnsureSuccessStatusCode();
            var actualResponseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal($"{externalResponseContent.Length}", actualResponseContent);
            var requests = _mockServerSearchEngine.LogEntries.Select(l => l.RequestMessage).ToList();
            Assert.Single(requests);
            Assert.Contains($"/search?q={searchQuery}", requests.Single().AbsoluteUrl);
        }

        [Theory]
        [InlineData("Daan", "SomeResponseFromGoogle")]
        [InlineData("Sean", "SomeOtherResponseFromGoogle")]
        public async Task TestWithUnstableServer(string searchQuery, string externalResponseContent)
        {
            SetupUnStableServer(externalResponseContent);
            var response = await HttpClient.GetAsync($"/api/searchengine/{searchQuery}");
            response.EnsureSuccessStatusCode();
            var actualResponseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal($"{externalResponseContent.Length}", actualResponseContent);
            var requests = _mockServerSearchEngine.LogEntries.Select(l => l.RequestMessage).ToList();
            Assert.Equal(2,requests.Count);
            Assert.Contains($"/search?q={searchQuery}", requests.Last().AbsoluteUrl);
            Assert.Contains($"/search?q={searchQuery}", requests.First().AbsoluteUrl);
        }

        protected override Dictionary<string, string> GetConfiguration()
        {
            _mockServerSearchEngine = FluentMockServer.Start();
            var googleUrl = _mockServerSearchEngine.Urls.Single();
            var configuration = base.GetConfiguration();
            configuration.Add("Google", googleUrl);
            return configuration;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _mockServerSearchEngine.Stop();
                _mockServerSearchEngine.Dispose();
            }
        }

        private void SetupStableServer(string response)
        {
            _mockServerSearchEngine.Given(Request.Create().UsingGet())
                .RespondWith(Response.Create().WithBody(response, encoding:Encoding.UTF8)
                    .WithStatusCode(HttpStatusCode.OK));
        }

        private void SetupUnStableServer(string response)
        {
            _mockServerSearchEngine.Given(Request.Create().UsingGet())
                .InScenario("UnstableServer")
                .WillSetStateTo("FIRSTCALLDONE")
                .RespondWith(Response.Create().WithBody(response, encoding: Encoding.UTF8)
                .WithStatusCode(HttpStatusCode.InternalServerError));

            _mockServerSearchEngine.Given(Request.Create().UsingGet())
                .InScenario("UnstableServer")
                .WhenStateIs("FIRSTCALLDONE")
                .RespondWith(Response.Create().WithBody(response, encoding: Encoding.UTF8)
                .WithStatusCode(HttpStatusCode.OK));
        }
    }
}
