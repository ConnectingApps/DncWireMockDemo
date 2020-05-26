using ConnectingApps.DncWireMockDemo;
using ConnectingApps.DncWireMockDemo.Services;
using ConnectingApps.IntegrationFixture;
using ConnectingApps.IntegrationFixture.Shared;
using Moq;
using Refit;
using Xunit;

namespace ConnectingApps.IntegrationFixtureTests
{
    public class FreezeTests
    {
        [Fact]
        public void DoubleCallRefitFixture()
        {
            using (var fixture = new RefitFixture<Startup, ISearchEngine>(RestService.For<ISearchEngine>))
            {
                var service = fixture.Freeze<Mock<ISearchEngineService>>();
                Assert.Throws<IntegrationFixtureException>(() => fixture.Freeze<Mock<ISearchEngineService>>());
            }
        }

        [Fact]
        public void DoubleCallFixture()
        {
            using (var fixture = new Fixture<Startup>())
            {
                var service = fixture.Freeze<Mock<ISearchEngineService>>();
                Assert.Throws<IntegrationFixtureException>(() => fixture.Freeze<Mock<ISearchEngineService>>());
            }
        }

        [Fact]
        public void WrongClassFixture()
        {
            using (var fixture = new Fixture<Startup>())
            {
                Assert.Throws<IntegrationFixtureException>(() => fixture.Freeze<FreezeTests>());
            }
        }

        [Fact]
        public void WrongClassRefitFixture()
        {
            using (var fixture = new RefitFixture<Startup, ISearchEngine>(RestService.For<ISearchEngine>))
            {
                Assert.Throws<IntegrationFixtureException>(() => fixture.Freeze<FreezeTests>());
            }
        }
    }
}
