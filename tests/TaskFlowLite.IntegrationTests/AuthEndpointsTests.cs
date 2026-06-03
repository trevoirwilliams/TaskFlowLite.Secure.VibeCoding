using System.Net;
using System.Text;
using System.Text.Json;

namespace TaskFlowLite.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidPayload_ReturnsToken()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register")
        {
            Content = new StringContent(
                """
                {
                  "displayName": "Taylor Reese",
                  "email": "taylor.reese@taskflow.local",
                  "password": "StrongPass123"
                }
                """,
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(string.IsNullOrWhiteSpace(json.RootElement.GetProperty("accessToken").GetString()));
        Assert.Equal("Bearer", json.RootElement.GetProperty("tokenType").GetString());
    }

    [Fact]
    public async Task Login_WithSeededUser_ReturnsToken()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = new StringContent(
                """
                {
                  "email": "alex.rivera@taskflow.local",
                  "password": "TaskFlow!234"
                }
                """,
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(string.IsNullOrWhiteSpace(json.RootElement.GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = new StringContent(
                """
                {
                  "email": "alex.rivera@taskflow.local",
                  "password": "WrongPassword1"
                }
                """,
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
