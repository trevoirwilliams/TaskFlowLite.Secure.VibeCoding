using System.Net;

namespace TaskFlowLite.IntegrationTests;

public class MetadataEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public MetadataEndpointsTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetPriorities_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/api/metadata/priorities");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
