using System.Net;
using System.Text;
using System.Text.Json;

namespace TaskFlowLite.IntegrationTests;

public class RequestNotesSecurityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public RequestNotesSecurityTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetNotes_WithoutBearerToken_ReturnsUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/workrequests/1/notes");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetNotes_AsUnrelatedUser_ReturnsNotFound()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.SamirEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(HttpMethod.Get, "/api/workrequests/1/notes", token);
        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddNote_AsUnrelatedUser_ReturnsNotFound_AndDoesNotPersist()
    {
        var ownerToken = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);
        var outsiderToken = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.SamirEmail);

        var beforeCount = await GetNotesCountAsync(ownerToken, 1);

        using var addRequest = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Post,
            "/api/workrequests/1/notes",
            outsiderToken,
            new StringContent("""{"body":"Unauthorized note write attempt."}""", Encoding.UTF8, "application/json"));

        var addResponse = await _httpClient.SendAsync(addRequest);
        Assert.Equal(HttpStatusCode.NotFound, addResponse.StatusCode);

        var afterCount = await GetNotesCountAsync(ownerToken, 1);
        Assert.Equal(beforeCount, afterCount);
    }

    [Fact]
    public async Task AddNote_ForMissingWorkRequest_ReturnsNotFound()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Post,
            "/api/workrequests/999/notes",
            token,
            new StringContent("""{"body":"missing work request"}""", Encoding.UTF8, "application/json"));

        var response = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> GetNotesCountAsync(string accessToken, int workRequestId)
    {
        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(HttpMethod.Get, $"/api/workrequests/{workRequestId}/notes", accessToken);
        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetArrayLength();
    }
}