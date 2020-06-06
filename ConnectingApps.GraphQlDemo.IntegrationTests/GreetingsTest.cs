using System.Net;
using System.Threading.Tasks;
using ConnectingApps.IntegrationFixture.Shared.GraphQl;
using Newtonsoft.Json.Linq;
using FluentAssertions.Json;
using Xunit;

namespace ConnectingApps.GraphQlDemo.IntegrationTests
{
    public class GreetingsTest
    {
        [Fact]
        public async Task HelloTest()
        {
            using (var graphQlFixture = new GraphQlFixture<Startup>("playground/.."))
            {
                var client = graphQlFixture.GetClient();
                var response = await client.ExecuteQuery("{ greetings { hello }}");
                string expectedJson = "{\"data\":{\"greetings\":{\"hello\":\"World\"}}}";

                var actual = JToken.Parse(response.ResponseContent);
                var expected = JToken.Parse(expectedJson);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}
