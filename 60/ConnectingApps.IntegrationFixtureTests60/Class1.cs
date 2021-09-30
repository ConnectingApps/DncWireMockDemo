using System;
using System.Threading.Tasks;
using ConnectingApps.Dnc60Demo.Models;
using ConnectingApps.IntegrationFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
using Refit;
using Xunit;

namespace ConnectingApps.IntegrationFixtureTests60
{
    public class Class1
    {
        class TodoApplication : WebApplicationFactory<Program>
        {
            protected override IHost CreateHost(IHostBuilder builder)
            {
                // Add mock/test services to the builder here

                return base.CreateHost(builder);
            }
        }

        [Fact]
        public async Task NameTest()
        {
            using (var fixture = new RefitFixture<Program, ILogicClient>(RestService.For<ILogicClient>))
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
    }
}
