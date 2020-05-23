using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ConnectingApps.DncWireMockDemo;
using ConnectingApps.DncWireMockDemo.Controllers;
using ConnectingApps.DncWireMockDemo.Models;
using ConnectingApps.IntegrationFixture;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Xunit;

namespace ConnectingApps.IntegrationFixtureTests
{
    public class LogicControllerTests
    {
        [Fact]
        public async Task NameTest()
        {
            using (var fixture = new RefitFixture<Startup, ILogicClient>(RestService.For<ILogicClient>))
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
            }
        }

        [Fact]
        public async Task NoMiddleNameTest()
        {
            using (var fixture = new RefitFixture<Startup, ILogicClient>(RestService.For<ILogicClient>))
            {
                var refitClient = fixture.GetRefitClient();
                var response = await refitClient.Post(new Name
                {
                    FirstName = "Alexander",
                    LastName = "Johnson",
                });
                await response.EnsureSuccessStatusCodeAsync();
                Assert.Contains("Alexander", response.Content);
                Assert.Contains("Johnson", response.Content);
            }
        }

        [Fact]
        public async Task NoMiddleNamePutRequestTest()
        {
            using (var fixture = new RefitFixture<Startup, ILogicClient>(RestService.For<ILogicClient>))
            {
                var refitClient = fixture.GetRefitClient();
                var response = await refitClient.Put(new Name
                {
                    FirstName = "Alexander",
                    LastName = "Johnson",
                });
                await response.EnsureSuccessStatusCodeAsync();
                Assert.Contains("Alexander", response.Content);
                Assert.Contains("Johnson", response.Content);
            }
        }

        [Fact]
        public async Task NoMiddleNamePutTest()
        {
            using (var fixture = new Fixture<Startup>())
            {
                var controller = fixture.Create<LogicController>();
                var response = controller.Put(new Name()
                {
                    FirstName = "F",
                    LastName = "L"
                });
                Assert.Equal(200, ((ObjectResult)response.Result).StatusCode);
                Assert.Single(fixture.LogSource.GetWarnings());
                var dataLogged = fixture.LogSource.GetLoggedObjects<Name>().ToList();
                Assert.Single(dataLogged);
                Assert.Equal("F", dataLogged.Single().Value.FirstName);
                Assert.Contains(fixture.LogSource.GetLogLines(), a => a == "Warning Logged");
                Assert.Contains(fixture.LogSource.GetLogLines(), a => a.Contains("This is the input"));
                Assert.Single(fixture.LogSource.GetExceptions().OfType<InvalidDataException>());
            } 
        }

        [Theory]
        [InlineData("Boris","Alexander", "Johnson", HttpStatusCode.OK, "Boris", "Alexander", "Johnson")]
        [InlineData("Boris", "Alexander", null, HttpStatusCode.BadRequest)]
        [InlineData(null, "Alexander", "Johnson", HttpStatusCode.BadRequest)]
        [InlineData("Boris", "Alexander", "", HttpStatusCode.BadRequest)]
        [InlineData("", "Alexander", "Johnson", HttpStatusCode.BadRequest)]
        public async Task NamesTest(string firstName, string middleName, string lastName, HttpStatusCode expectedStatusCode, params string[] responseContains)
        {
            using (var fixture = new RefitFixture<Startup, ILogicClient>(RestService.For<ILogicClient>))
            {
                var refitClient = fixture.GetRefitClient();
                var response = await refitClient.Post(new Name
                {
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                });

                var statusCode = response.StatusCode;

                Assert.Equal(expectedStatusCode, statusCode);
                var content = response.Content;

                foreach (var expectedResponse in responseContains)
                {
                    Assert.Contains(expectedResponse, content);
                }
            }
        }
    }
}
