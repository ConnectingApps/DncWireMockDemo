using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using ConnectingApps.DncWireMockDemo.Controllers;
using ConnectingApps.DncWireMockDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ConnectingApps.AutoFixtureTests
{
    public class SearchEngineControllerTests
    {

        [Fact]
        public async Task GetTest()
        {
            // arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var service = fixture.Freeze<Mock<ISearchEngineService>>();
            service.Setup(a => a.GetNumberOfCharactersFromSearchQuery(It.IsNotNull<string>()))
                .ReturnsAsync(8);

            var controller = fixture.Build<SearchEngineController>().OmitAutoProperties().Create();

            // act
            var response = await controller.GetNumberOfCharacters("Hoi");

            // assert
            Assert.Equal(8, ((OkObjectResult)response.Result).Value);
            service.Verify(s => s.GetNumberOfCharactersFromSearchQuery("Hoi"), Times.Once);
        }
    }
}
