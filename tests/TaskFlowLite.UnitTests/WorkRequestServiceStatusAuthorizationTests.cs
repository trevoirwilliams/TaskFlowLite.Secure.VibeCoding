using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.WorkRequests;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Domain.Enums;
using TaskFlowLite.Infrastructure.Persistence;
using TaskFlowLite.Infrastructure.Services;

namespace TaskFlowLite.UnitTests;

public class WorkRequestServiceStatusAuthorizationTests
{
    [Fact]
    public async Task UpdateStatus_Manager_CanUpdateAnyWorkRequest()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var currentUser = new StubCurrentUserContext(userId: 99, roles: ["Manager"]);
        var service = new WorkRequestService(fixture.DbContext, currentUser);

        var result = await service.UpdateStatusAsync(
            id: 1,
            new UpdateWorkRequestStatusRequest(WorkRequestStatus.Blocked),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(WorkRequestStatus.Blocked, result!.Status);

        var persisted = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 1);
        Assert.Equal(WorkRequestStatus.Blocked, persisted.Status);
    }

    [Fact]
    public async Task UpdateStatus_Worker_AssignedToRequest_CanUpdateStatus()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var currentUser = new StubCurrentUserContext(userId: 2, roles: ["Worker"]);
        var service = new WorkRequestService(fixture.DbContext, currentUser);

        var result = await service.UpdateStatusAsync(
            id: 2,
            new UpdateWorkRequestStatusRequest(WorkRequestStatus.Done),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(WorkRequestStatus.Done, result!.Status);

        var persisted = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 2);
        Assert.Equal(WorkRequestStatus.Done, persisted.Status);
    }

    [Fact]
    public async Task UpdateStatus_Worker_NotAssignedToRequest_ReturnsNull_AndDoesNotMutate()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var currentUser = new StubCurrentUserContext(userId: 3, roles: ["Worker"]);
        var service = new WorkRequestService(fixture.DbContext, currentUser);

        var before = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 2);

        var result = await service.UpdateStatusAsync(
            id: 2,
            new UpdateWorkRequestStatusRequest(WorkRequestStatus.Done),
            CancellationToken.None);

        Assert.Null(result);

        var after = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 2);
        Assert.Equal(before.Status, after.Status);
    }

    [Fact]
    public async Task UpdateStatus_RequesterWithoutPermittedRole_ReturnsNull_AndDoesNotMutate()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var currentUser = new StubCurrentUserContext(userId: 1, roles: []);
        var service = new WorkRequestService(fixture.DbContext, currentUser);

        var before = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 1);

        var result = await service.UpdateStatusAsync(
            id: 1,
            new UpdateWorkRequestStatusRequest(WorkRequestStatus.Blocked),
            CancellationToken.None);

        Assert.Null(result);

        var after = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 1);
        Assert.Equal(before.Status, after.Status);
    }

    [Fact]
    public async Task UpdateStatus_UnauthenticatedCaller_ReturnsNull_AndDoesNotMutate()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var currentUser = new StubCurrentUserContext();
        var service = new WorkRequestService(fixture.DbContext, currentUser);

        var before = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 1);

        var result = await service.UpdateStatusAsync(
            id: 1,
            new UpdateWorkRequestStatusRequest(WorkRequestStatus.Blocked),
            CancellationToken.None);

        Assert.Null(result);

        var after = await fixture.DbContext.WorkRequests.AsNoTracking().SingleAsync(x => x.Id == 1);
        Assert.Equal(before.Status, after.Status);
    }

    private sealed class StubCurrentUserContext : ICurrentUserContext
    {
        private readonly int? _userId;
        private readonly HashSet<string> _roles;

        public StubCurrentUserContext(int? userId = null, IEnumerable<string>? roles = null)
        {
            _userId = userId;
            _roles = new HashSet<string>(roles ?? [], StringComparer.OrdinalIgnoreCase);
        }

        public bool TryGetUserId(out int userId)
        {
            userId = _userId.GetValueOrDefault();
            return _userId.HasValue && _userId.Value > 0;
        }

        public bool IsInRole(string role)
        {
            return _roles.Contains(role);
        }

        public int UserId => _userId ?? throw new InvalidOperationException("Current user is not authenticated.");

        public string DisplayName => "Unit Test User";
    }

    private sealed class TestFixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        private TestFixture(TaskFlowLiteDbContext dbContext, SqliteConnection connection)
        {
            DbContext = dbContext;
            _connection = connection;
        }

        public TaskFlowLiteDbContext DbContext { get; }

        public static async Task<TestFixture> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<TaskFlowLiteDbContext>()
                .UseSqlite(connection)
                .Options;

            var dbContext = new TaskFlowLiteDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();

            var users = new[]
            {
                new ApplicationUser { Id = 1, UserName = "requester@local", Email = "requester@local", DisplayName = "Requester" },
                new ApplicationUser { Id = 2, UserName = "worker@local", Email = "worker@local", DisplayName = "Worker" },
                new ApplicationUser { Id = 3, UserName = "outsider@local", Email = "outsider@local", DisplayName = "Outsider" },
                new ApplicationUser { Id = 99, UserName = "manager@local", Email = "manager@local", DisplayName = "Manager" }
            };

            var requests = new[]
            {
                new WorkRequest
                {
                    Id = 1,
                    Title = "Req-1",
                    Description = "Requested by user 1 and unassigned.",
                    RequestedByUserId = 1,
                    Priority = Priority.Medium,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new WorkRequest
                {
                    Id = 2,
                    Title = "Req-2",
                    Description = "Assigned to worker user 2.",
                    RequestedByUserId = 1,
                    Priority = Priority.High,
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            requests[1].AssignTo(2);

            await dbContext.Users.AddRangeAsync(users);
            await dbContext.WorkRequests.AddRangeAsync(requests);
            await dbContext.SaveChangesAsync();

            return new TestFixture(dbContext, connection);
        }

        public async ValueTask DisposeAsync()
        {
            await DbContext.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
