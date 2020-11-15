using ConnectingApps.IntegrationFixture;
using System.Threading.Tasks;
using Xunit;

namespace ConnectingApps.Templates.IntegrationFixture50
{
    /// <summary>
    ///     Read the docs here https://github.com/ConnectingApps/DncWireMockDemo#integrationfixture
    ///     If you can't run the tests in VS, just reload the project
    /// </summary>
    public class YourTests
    {

        /// <summary>
        /// The Startup class is part of you web (api) project, please add a reference
        /// A controller test where the external services is mocked with wiremock
        /// Read about WireMock.NET here https://github.com/WireMock-Net/WireMock.Net/wiki/Using-WireMock-in-UnitTes
        /// More info: https://github.com/ConnectingApps/DncWireMockDemo#integrationfixture
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FixtureTest()
        {
            // arrange
            /*await using (var fixture = new Fixture<Startup>())
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
            }*/
        }

        /// <summary>
        ///  This is a RefitFixture test, doing real web requests to your project without starting it but with self hosting
        ///  The Startup class is part of you web (api) project,  please add a reference
        ///  The ILogicClient is your refit interface to test your own test project
        ///  More info https://github.com/ConnectingApps/DncWireMockDemo#refit-support
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RefitFixtureTest()
        {
            /*using (var fixture = new RefitFixture<Startup, ILogicClient>(RestService.For<ILogicClient>))
            {
                var refitClient = fixture.GetRefitClient();
                var response = await refitClient.Post(new Name
                {
                    FirstName = "Alexander",
                    MiddleName = "Boris",
                    LastName = "Johnson",
                });
                await response.EnsureSuccessStatusCodeAsync();
                Assert.Contains("Alexander", response.Content);
                Assert.Contains("Boris", response.Content);
                Assert.Contains("Johnson", response.Content);
            }*/
        }

        /// <summary>
        ///  This is a Graph Ql test, only use it if your web api project uses GraphQl
        ///  The Startup class is part of you web (api) project,  please add a reference
        ///  More info: https://github.com/ConnectingApps/DncWireMockDemo#graphql-support
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GraphQlTest()
        {
            // arrange
            /*using (var graphQlFixture = new GraphQlFixture<Startup>("playground/.."))
            {
                var client = graphQlFixture.GetClient();

                // act
                var response = await client.ExecuteQuery("{ greetings { hello }}");

                // assert
                string expectedJson = "{\"data\":{\"greetings\":{\"hello\":\"World\"}}}";

                var actual = JToken.Parse(response.ResponseContent);
                var expected = JToken.Parse(expectedJson);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                actual.Should().BeEquivalentTo(expected); // You need Fluent assertions.json for this
            }*/
        }
    }
}