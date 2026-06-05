using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TaskFlowLite.IntegrationTests;

internal static class SecurityTestAuthHelpers
{
    internal const string SeedPassword = "TaskFlow!234";
    internal const string AlexEmail = "alex.rivera@taskflow.local";
    internal const string JamieEmail = "jamie.chen@taskflow.local";
    internal const string SamirEmail = "samir.patel@taskflow.local";

    internal static async Task<string> LoginAndGetTokenAsync(HttpClient httpClient, string email)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = new StringContent(
                $$"""
                {
                  "email": "{{email}}",
                  "password": "{{SeedPassword}}"
                }
                """,
                Encoding.UTF8,
                "application/json")
        };

        var response = await httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Login response did not contain an access token.");
    }

    internal static HttpRequestMessage BuildAuthenticatedRequest(
        HttpMethod method,
        string uri,
        string accessToken,
        HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = content;
        return request;
    }
}