# DncWireMockDemo

## Introduction
A demonstration of how [WireMock.NET](https://github.com/WireMock-Net/WireMock.Net) can help you with [integration testing of your .NET Core project](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1).
Here is [our article on codeproject](https://www.codeproject.com/Articles/5267354/How-WireMock-NET-can-help-doing-integration-testin) about this.


## IntegrationFixture

IntegrationFixture is a package [available on NuGet](https://www.nuget.org/packages/ConnectingApps.IntegrationFixture/) to do integration testing with a Fixture, just like many developers use [AutoFixture](https://github.com/AutoFixture/AutoFixture) for unit tests. Read more about this in [our article on CodeProject](https://www.codeproject.com/Articles/5267948/Integration-Testing-More-Fixtures-than-AutoFixture). The main difference is that you need to setup (and Freeze) and verify external dependencies instead of external dependencies. Setting up an external dependency (typically a web service) is done with [WireMock.NET](https://github.com/WireMock-Net/WireMock.Net/wiki/Stubbing). Here is a coding example with .NET Core 3.1 (this is the recommended version, we have .NET Core 2.1 support too with an example [here](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/21/ConnectingApps.IntegrationFixtureTests21.NuGet)):

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

> :warning: ⚠ **The type specified via Create<> is typically a controller class or an interface. This is because it has to be resolved via the DI of .NET Core. So use Create < SomeController> or Create< ISomeInterface>.**

> :warning: ⚠ **Call Freeze methods before the Create methods. This ensures the Create "knows" about the frozen objects or mock servers**

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
    <PackageReference Include="ConnectingApps.IntegrationFixture" Version="3.1.7" />
  </ItemGroup>
````

For .NET Core 2.1, it is typically:

````xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="2.1.3" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.6.1" />
    <PackageReference Include="ConnectingApps.IntegrationFixture" Version="2.1.7" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
````

For .NET 5, it is typically:

````xml
<ItemGroup>
    <PackageReference Include="ConnectingApps.IntegrationFixture" Version="5.0.7" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="5.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.8.0" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.3">
        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        <PrivateAssets>all</PrivateAssets>
    </PackageReference>
</ItemGroup>
````

> :warning: ⚠ **ReSharper and Test Runner may not directly detect the dependencies but this can be solved by reloading your project **

For [Refit support](https://github.com/ConnectingApps/DncWireMockDemo#refit-support), you just need to a install a recent version of Refit. The specific version does not really matter.

xUnit is recommended to be used as a test framework but it is not required. Here is a [full example](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/ConnectingApps.IntegrationFixtureTests.Nuget) of such a test project.

There is also an [example](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/21/ConnectingApps.IntegrationFixtureTests21.NuGet) for .NET Core 2.1 .

### Customization

Like with AutoFixture, it is possible to Customize your fixture and create your own Customization.

Here is an example of Customization (added in 2.1.3 and 3.1.3).
New Configuration parameters (from a json file) are added like this:
````csharp
fixture.Customize(new JsonCustomizer(jsonPath));
````

### Refit support
Call your controller from a [refit client](https://github.com/reactiveui/refit#refit-the-automatic-type-safe-rest-library-for-net-core-xamarin-and-net) and test both your client package and controller package at once, added in 2.1.4 and 3.1.4 . We wrote [an article about this on CodeProject](https://www.codeproject.com/Tips/5268823/Testing-Validation-Attributes-with-xUnit).

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

### Assert on the logged data
A new feature added in 3.1.5 and 2.1.5 . You can assert on the logged data and the test lines that are logged
````csharp
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
````
### Freeze Moq Mocks
[Just like with AutoFixture](https://thomasardal.com/using-automoqcustomization-with-nunit-moq-and-autofixture/), IntegrationFixture supports the Freeze method since 3.1.6 and 2.1.6 method to Freeze mocks. For this, [Moq](https://www.nuget.org/packages/Moq/) needs to be installed in the test project. Once that is done, the mock can be used inside the test project to be verified at the end of the test. Here are some examples:

````csharp
// arrange
await using (var fixture = new Fixture<Startup>())
{
    var service = fixture.Freeze<Mock<ISearchEngineService>>();
    service.Setup(a => a.GetNumberOfCharactersFromSearchQuery(It.IsNotNull<string>()))
        .ReturnsAsync(8);

    var controller = fixture.Create<SearchEngineController>();

    // act
    var response = await controller.GetNumberOfCharacters("Hoi");

    // assert
    Assert.Equal(8, ((OkObjectResult) response.Result).Value);
    service.Verify(s => s.GetNumberOfCharactersFromSearchQuery("Hoi"), Times.Once);
}
````

````csharp
// arrange
using (var fixture = new RefitFixture<Startup, ISearchEngine>(RestService.For<ISearchEngine>))
{
    var service = fixture.Freeze<Mock<ISearchEngineService>>();
    service.Setup(a => a.GetNumberOfCharactersFromSearchQuery(It.IsNotNull<string>()))
        .ReturnsAsync(8);

    var refitClient = fixture.GetRefitClient();

    // act
    var response = await refitClient.GetNumberOfCharacters("Hoi");
    
    // assert
    await response.EnsureSuccessStatusCodeAsync();
    service.Verify(s => s.GetNumberOfCharactersFromSearchQuery("Hoi"), Times.Once);
}
````

### GraphQL Support
In version 2.1.7 and 3.1.7, [GraphQL](https://graphql.org/) support was added. The following code uses IntegrationFixture to do the test.  It works by creating a GraphQl client. After creation, a GraphQL query can be executed asynchronously. The output is a string (typically Json formatted). The output is verified with [FluentAssertions.Json](https://github.com/fluentassertions/fluentassertions.json#usage) but this is a personal preference and not a requirement for testing.
A full example of such a test can be found [here](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/ConnectingApps.GraphQlDemo.IntegrationTests.NuGet).

````csharp
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
````
### .NET 5 Support

.NET 5 Support has been added. An example of .NET 5 usage can be found [here](https://github.com/ConnectingApps/DncWireMockDemo/tree/master/50/ConnectingApps.IntegrationFixture50Tests.NuGet). This is how your `ItemGroup` element in your csproj should look like.

````xml
<ItemGroup>
    <PackageReference Include="ConnectingApps.IntegrationFixture" Version="5.0.7" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="5.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.8.0" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.3">
        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        <PrivateAssets>all</PrivateAssets>
    </PackageReference>
</ItemGroup>
````



## TestTemplates

You can use our [test templates](https://www.nuget.org/packages/ConnectingApps.TestTemplates) after installation and package restore. It uses IntegrationFixture as described [here](https://github.com/ConnectingApps/DncWireMockDemo#integrationfixture).
```bash
dotnet new --install ConnectingApps.TestTemplates 
dotnet new integrationtest-31
dotnet restore
```

If you use .NET Core 2.1 instead of 3.1, use this:

```bash
dotnet new --install ConnectingApps.TestTemplates 
dotnet new integrationtest-21
dotnet restore
```

We now have .NET 5 support too.

If you use .NET Core 5.0

```bash
dotnet new --install ConnectingApps.TestTemplates 
dotnet new integrationtest-50
dotnet restore
```


