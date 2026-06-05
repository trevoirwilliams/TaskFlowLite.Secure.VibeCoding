using System.Net;
using System.Text;
using System.Text.Json;

namespace TaskFlowLite.IntegrationTests;

public class WorkRequestsSecurityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public WorkRequestsSecurityTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetWorkRequestById_WithoutBearerToken_ReturnsUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/workrequests/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetWorkRequestById_AsUnrelatedUser_ReturnsNotFound()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.SamirEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(HttpMethod.Get, "/api/workrequests/1", token);
        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateWorkRequest_AsUnrelatedUser_DoesNotMutateTarget()
    {
        var ownerToken = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);
        var outsiderToken = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.SamirEmail);

        var baseline = await GetWorkRequestByIdAsync(ownerToken, 1);

        using var updateRequest = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Put,
            "/api/workrequests/1",
            outsiderToken,
            new StringContent(
                """
                {
                  "title": "UNAUTHORIZED TITLE CHANGE",
                  "description": "UNAUTHORIZED DESCRIPTION CHANGE",
                  "priority": "Low"
                }
                """,
                Encoding.UTF8,
                "application/json"));

        var updateResponse = await _httpClient.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);

        var afterUpdate = await GetWorkRequestByIdAsync(ownerToken, 1);
        Assert.Equal(baseline.GetProperty("title").GetString(), afterUpdate.GetProperty("title").GetString());
        Assert.Equal(baseline.GetProperty("description").GetString(), afterUpdate.GetProperty("description").GetString());
        Assert.Equal(baseline.GetProperty("priority").GetString(), afterUpdate.GetProperty("priority").GetString());
    }

    [Fact]
    public async Task CreateWorkRequest_UsesAuthenticatedUserAsRequester()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.JamieEmail);

        using var createRequest = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Post,
            "/api/workrequests",
            token,
            new StringContent(
                """
                {
                  "title": "Security test request",
                  "description": "Validate requester identity source.",
                  "priority": "High",
                  "assignedToUserId": 1
                }
                """,
                Encoding.UTF8,
                "application/json"));

        var response = await _httpClient.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var requesterUserId = json.RootElement.GetProperty("requestedByUserId").GetInt32();
        Assert.Equal(2, requesterUserId);
    }

    [Fact]
    public async Task AssignWorkRequest_WithoutManagerRole_ReturnsForbidden()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var assignRequest = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Patch,
            "/api/workrequests/2/assign",
            token,
            new StringContent("""{"assignedToUserId":1}""", Encoding.UTF8, "application/json"));

        var response = await _httpClient.SendAsync(assignRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_WithoutManagerOrWorkerRole_ReturnsForbidden()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var statusRequest = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Patch,
            "/api/workrequests/2/status",
            token,
            new StringContent("""{"status":"Blocked"}""", Encoding.UTF8, "application/json"));

        var response = await _httpClient.SendAsync(statusRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<JsonElement> GetWorkRequestByIdAsync(string accessToken, int id)
    {
        using var getRequest = SecurityTestAuthHelpers.BuildAuthenticatedRequest(HttpMethod.Get, $"/api/workrequests/{id}", accessToken);
        var getResponse = await _httpClient.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        using var json = JsonDocument.Parse(await getResponse.Content.ReadAsStringAsync());
        return json.RootElement.Clone();
    }
}