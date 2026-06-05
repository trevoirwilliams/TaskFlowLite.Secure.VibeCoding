using System.Net;
using System.Text;
using System.Text.Json;

namespace TaskFlowLite.IntegrationTests;

public class ValidationAndExposureSecurityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public ValidationAndExposureSecurityTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWorkRequest_WithMissingTitle_ReturnsBadRequest()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Post,
            "/api/workrequests",
            token,
            new StringContent(
                """
                {
                  "title": "",
                  "description": "desc",
                                    "priority": 2,
                  "assignedToUserId": 2
                }
                """,
                Encoding.UTF8,
                "application/json"));

        var response = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateWorkRequest_WithTitleOver120Chars_ReturnsBadRequest()
    {
        var longTitle = new string('x', 121);
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Put,
            "/api/workrequests/1",
            token,
            new StringContent(
                $$"""
                {
                  "title": "{{longTitle}}",
                  "description": "Valid description",
                                    "priority": 1
                }
                """,
                Encoding.UTF8,
                "application/json"));

        var response = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddNote_WithBodyOver2000Chars_ReturnsBadRequest()
    {
        var longBody = new string('n', 2001);
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Post,
            "/api/workrequests/1/notes",
            token,
            new StringContent($$"""{"body":"{{longBody}}"}""", Encoding.UTF8, "application/json"));

        var response = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateWorkRequest_WithInvalidPriorityEnumValue_ReturnsBadRequest()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(
            HttpMethod.Post,
            "/api/workrequests",
            token,
            new StringContent(
                """
                {
                  "title": "Invalid enum check",
                  "description": "Reject invalid enum value",
                                    "priority": "NotARealPriority",
                  "assignedToUserId": 2
                }
                """,
                Encoding.UTF8,
                "application/json"));

        var response = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_DoesNotExposeSensitiveIdentityFields()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(HttpMethod.Get, "/api/users", token);
        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var first = json.RootElement.EnumerateArray().First();

        Assert.True(first.TryGetProperty("id", out _));
        Assert.True(first.TryGetProperty("displayName", out _));
        Assert.True(first.TryGetProperty("email", out _));

        Assert.False(first.TryGetProperty("passwordHash", out _));
        Assert.False(first.TryGetProperty("securityStamp", out _));
        Assert.False(first.TryGetProperty("concurrencyStamp", out _));
        Assert.False(first.TryGetProperty("roles", out _));
    }

    [Fact]
    public async Task GetWorkRequests_DoesNotExposeUnexpectedSensitiveFields()
    {
        var token = await SecurityTestAuthHelpers.LoginAndGetTokenAsync(_httpClient, SecurityTestAuthHelpers.AlexEmail);

        using var request = SecurityTestAuthHelpers.BuildAuthenticatedRequest(HttpMethod.Get, "/api/workrequests", token);
        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var first = json.RootElement.EnumerateArray().First();

        Assert.True(first.TryGetProperty("id", out _));
        Assert.True(first.TryGetProperty("title", out _));
        Assert.True(first.TryGetProperty("description", out _));
        Assert.True(first.TryGetProperty("priority", out _));
        Assert.True(first.TryGetProperty("status", out _));
        Assert.True(first.TryGetProperty("requestedByUserId", out _));
        Assert.True(first.TryGetProperty("assignedToUserId", out _));
        Assert.True(first.TryGetProperty("createdAtUtc", out _));
        Assert.True(first.TryGetProperty("updatedAtUtc", out _));

        Assert.False(first.TryGetProperty("passwordHash", out _));
        Assert.False(first.TryGetProperty("securityStamp", out _));
        Assert.False(first.TryGetProperty("requesterEmail", out _));
    }
}