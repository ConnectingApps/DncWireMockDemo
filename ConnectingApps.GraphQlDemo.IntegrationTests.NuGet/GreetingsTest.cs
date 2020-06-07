using System.Net;
using System.Threading.Tasks;
using ConnectingApps.IntegrationFixture.Shared.GraphQl;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ConnectingApps.GraphQlDemo.IntegrationTests.NuGet
{
    public class GreetingsTest
    {
        [Fact]
        public async Task HelloTest()
        {
            // arrange
            using (var graphQlFixture = new GraphQlFixture<Startup>("playground/.."))
            {
                var client = graphQlFixture.GetClient();

                // act
                var response = await client.ExecuteQuery("{ greetings { hello }}");

                // assert
                string expectedJson = "{\"data\":{\"greetings\":{\"hello\":\"World\"}}}";

                var actual = JToken.Parse(response.ResponseContent);
                var expected = JToken.Parse(expectedJson);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}
