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

    [Fact]
    public async Task GetWorkRequests_ReturnsSeededResults()
    {
        var response = await _httpClient.GetAsync("/api/workrequests");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Rotate internal API keys", content);
    }
}
