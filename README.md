# DncWireMockDemo

## Introduction
A demonstration of how [WireMock.NET](https://github.com/WireMock-Net/WireMock.Net) can help you with [integration testing of your .NET Core project](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1).
Here is [our article on codeproject](https://www.codeproject.com/Articles/5267354/How-WireMock-NET-can-help-doing-integration-testin) about this.


## IntegrationFixture

IntegrationFixture is a package [available on NuGet](https://www.nuget.org/packages/ConnectingApps.IntegrationFixture/) to do integration testing with Fixture, just like many developers use [AutoFixture](https://github.com/AutoFixture/AutoFixture) for unit tests. Read more about this in [our article on CodeProject](https://www.codeproject.com/Articles/5267948/Integration-Testing-More-Fixtures-than-AutoFixture). The main difference is that you need to setup (and Freeze) and verify external dependencies instead of external dependencies. Setting up an external dependency (typically a web service) is done with [WireMock.NET](https://github.com/WireMock-Net/WireMock.Net/wiki/Stubbing). Here is a coding example with .NET Core 3.1 (this is the recommended version, we have .NET Core 2.1 support too with an example [here](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/21/ConnectingApps.IntegrationFixtureTests21.NuGet)):

````csharp
[Fact]
public async Task GetTest()
{
    // arrange
    using (var fixture = new Fixture<Startup>())
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

private void SetupStableServer(FluentMockServer fluentMockServer, string response)
{
    fluentMockServer.Given(Request.Create().UsingGet())
        .RespondWith(Response.Create().WithBody(response, encoding: Encoding.UTF8)
            .WithStatusCode(HttpStatusCode.OK));
}
````

To work with it, you need to setup the `ItemGroup` dependency in your test project correctly, like done here:

````xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="3.1.3" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.6.1" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="ConnectingApps.IntegrationFixture" Version="3.1.4" />
  </ItemGroup>
````

For .NET Core 2.1, it is typically:

````xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="2.1.3" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.6.1" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="ConnectingApps.IntegrationFixture" Version="2.1.4" />
  </ItemGroup>
````



xUnit is recommended to used a test framework but it is not required. Here is a [full example](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/ConnectingApps.IntegrationFixtureTests.Nuget) of such a test project.

There is also an [example](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/21/ConnectingApps.IntegrationFixtureTests21.NuGet) for .NET Core 2.1 .

### Customization

Like with AutoFixture, it is possible to Customize your fixture and create your own Customization.

Here is an example of Customization:

New Configuration parameters (from a json file) are added in 2.1.3 and 3.1.3.
````csharp
fixture.Customize(new JsonCustomizer(jsonPath));
````

### Refit support
Call your controller from a refit client and test both your client package and controller package at once, added in 2.1.4 and 3.1.4 .

````csharp
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
````

