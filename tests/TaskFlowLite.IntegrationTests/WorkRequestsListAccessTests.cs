using System.Net;
using System.Text.Json;

namespace TaskFlowLite.IntegrationTests;

public class WorkRequestsListAccessTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string UserHeaderName = "X-TaskFlow-UserId";
    private readonly HttpClient _httpClient;

    public WorkRequestsListAccessTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetWorkRequests_WithoutCurrentUserHeader_ReturnsEmptyList()
    {
        var response = await SendListRequestAsync(userIdHeader: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Empty(ReadIds(body));
    }

    [Fact]
    public async Task GetWorkRequests_WithInvalidCurrentUserHeader_ReturnsEmptyList()
    {
        var response = await SendListRequestAsync(userIdHeader: "invalid-user");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Empty(ReadIds(body));
    }

    [Fact]
    public async Task GetWorkRequests_AsRequester_ReturnsOnlyVisibleRequests()
    {
        var response = await SendListRequestAsync(userIdHeader: "1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 1 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_AsAssignee_ReturnsOnlyVisibleRequests()
    {
        var response = await SendListRequestAsync(userIdHeader: "3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 2 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_WithStatusFilter_NarrowsVisibleResults()
    {
        var response = await SendListRequestAsync(userIdHeader: "1", queryString: "?status=InProgress");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Empty(ReadIds(body));
    }

    [Fact]
    public async Task GetWorkRequests_WithAssignedToFilter_DoesNotExpandVisibility()
    {
        var response = await SendListRequestAsync(userIdHeader: "1", queryString: "?assignedToUserId=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Empty(ReadIds(body));
    }

    [Fact]
    public async Task GetWorkRequests_WithSearchByTitle_ReturnsMatchingVisibleRequests()
    {
        var response = await SendListRequestAsync(userIdHeader: "1", queryString: "?search=Rotate");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 1 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_WithSearchByDescription_ReturnsMatchingVisibleRequests()
    {
        var response = await SendListRequestAsync(userIdHeader: "3", queryString: "?search=security");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var ids = ReadIds(body);

        Assert.Equal(new[] { 2 }, ids);
    }

    [Fact]
    public async Task GetWorkRequests_WithWhitespaceSearch_TreatsAsNoSearch()
    {
        var baselineResponse = await SendListRequestAsync(userIdHeader: "1");
        var whitespaceResponse = await SendListRequestAsync(userIdHeader: "1", queryString: "?search=%20%20%20");

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
        var response = await SendListRequestAsync(userIdHeader: "1", queryString: $"?search={oversizedSearch}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> SendListRequestAsync(string? userIdHeader, string queryString = "")
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/workrequests{queryString}");

        if (!string.IsNullOrWhiteSpace(userIdHeader))
        {
            request.Headers.Add(UserHeaderName, userIdHeader);
        }

        return await _httpClient.SendAsync(request);
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
