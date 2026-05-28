using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(TaskFlowLiteDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var users = new List<User>
        {
            new() { Id = 1, DisplayName = "Alex Rivera", Email = "alex.rivera@taskflow.local", IsActive = true },
            new() { Id = 2, DisplayName = "Jamie Chen", Email = "jamie.chen@taskflow.local", IsActive = true },
            new() { Id = 3, DisplayName = "Samir Patel", Email = "samir.patel@taskflow.local", IsActive = true }
        };

        var requestOne = new WorkRequest
        {
            Id = 1,
            Title = "Rotate internal API keys",
            Description = "Complete scheduled key rotation for internal integrations.",
            Priority = Priority.High,
            RequestedByUserId = 1,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
        };

        var requestTwo = new WorkRequest
        {
            Id = 2,
            Title = "Refresh onboarding checklist",
            Description = "Update checklist based on the latest security baseline.",
            Priority = Priority.Medium,
            RequestedByUserId = 2,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        requestTwo.AssignTo(3);
        requestTwo.ChangeStatus(WorkRequestStatus.InProgress);

        var workRequests = new List<WorkRequest> { requestOne, requestTwo };

        var notes = new List<RequestNote>
        {
            new()
            {
                Id = 1,
                WorkRequestId = 1,
                AuthorUserId = 1,
                Body = "Coordinate with ops before key cutover.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = 2,
                WorkRequestId = 2,
                AuthorUserId = 3,
                Body = "Draft complete, pending review.",
                CreatedAtUtc = DateTime.UtcNow.AddHours(-6)
            }
        };

        await dbContext.Users.AddRangeAsync(users, cancellationToken);
        await dbContext.WorkRequests.AddRangeAsync(workRequests, cancellationToken);
        await dbContext.RequestNotes.AddRangeAsync(notes, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
