using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TaskFlowLite.IntegrationTests;

public class WorkRequestsListAccessTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string SeedPassword = "TaskFlow!234";
    private readonly HttpClient _httpClient;

    public WorkRequestsListAccessTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetWorkRequests_WithoutBearerToken_ReturnsUnauthorized()
    {
        var response = await SendListRequestAsync(accessToken: null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetWorkRequests_WithInvalidBearerToken_ReturnsUnauthorized()
    {
        var response = await SendListRequestAsync(accessToken: "invalid-token-value");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetWorkRequests_AsRequester_ReturnsOnlyVisibleRequests()
    {
        var accessToken = await LoginAndGetTokenAsync("alex.rivera@taskflow.local");
        var response = await SendListRequestAsync(accessToken: accessToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 1 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_AsAssignee_ReturnsOnlyVisibleRequests()
    {
        var accessToken = await LoginAndGetTokenAsync("samir.patel@taskflow.local");
        var response = await SendListRequestAsync(accessToken: accessToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 2 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_WithStatusFilter_NarrowsVisibleResults()
    {
        var accessToken = await LoginAndGetTokenAsync("alex.rivera@taskflow.local");
        var response = await SendListRequestAsync(accessToken: accessToken, queryString: "?status=InProgress");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Empty(ReadIds(body));
    }

    [Fact]
    public async Task GetWorkRequests_WithAssignedToFilter_DoesNotExpandVisibility()
    {
        var accessToken = await LoginAndGetTokenAsync("alex.rivera@taskflow.local");
        var response = await SendListRequestAsync(accessToken: accessToken, queryString: "?assignedToUserId=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Empty(ReadIds(body));
    }

    [Fact]
    public async Task GetWorkRequests_WithSearchByTitle_ReturnsMatchingVisibleRequests()
    {
        var accessToken = await LoginAndGetTokenAsync("alex.rivera@taskflow.local");
        var response = await SendListRequestAsync(accessToken: accessToken, queryString: "?search=Rotate");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 1 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_WithSearchByDescription_ReturnsMatchingVisibleRequests()
    {
        var accessToken = await LoginAndGetTokenAsync("samir.patel@taskflow.local");
        var response = await SendListRequestAsync(accessToken: accessToken, queryString: "?search=security");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 2 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_WithWhitespaceSearch_TreatsAsNoSearch()
    {
        var accessToken = await LoginAndGetTokenAsync("alex.rivera@taskflow.local");
        var baselineResponse = await SendListRequestAsync(accessToken: accessToken);
        var whitespaceResponse = await SendListRequestAsync(accessToken: accessToken, queryString: "?search=%20%20%20");

        Assert.Equal(HttpStatusCode.OK, baselineResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, whitespaceResponse.StatusCode);

        var baselineBody = await baselineResponse.Content.ReadAsStringAsync();
        var whitespaceBody = await whitespaceResponse.Content.ReadAsStringAsync();

        Assert.Equal(ReadIds(baselineBody), ReadIds(whitespaceBody));
    }

    [Fact]
    public async Task GetWorkRequests_WithSearchOverMaxLength_ReturnsBadRequest()
    {
        var oversizedSearch = new string('a', 101);
        var accessToken = await LoginAndGetTokenAsync("alex.rivera@taskflow.local");
        var response = await SendListRequestAsync(accessToken: accessToken, queryString: $"?search={oversizedSearch}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> SendListRequestAsync(string? accessToken, string queryString = "")
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/workrequests{queryString}");

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await _httpClient.SendAsync(request);
    }

    private async Task<string> LoginAndGetTokenAsync(string email)
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

        var response = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Login response did not contain an access token.");
    }

    private static int[] ReadIds(string responseBody)
    {
        using var json = JsonDocument.Parse(responseBody);

        return json.RootElement
            .EnumerateArray()
            .Select(x => x.GetProperty("id").GetInt32())
            .ToArray();
    }
}
