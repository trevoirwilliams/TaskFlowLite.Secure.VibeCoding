using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        TaskFlowLiteDbContext dbContext,
        UserManager<ApplicationUser>? userManager = null,
        string seedPassword = "TaskFlow!234",
        CancellationToken cancellationToken = default)
    {
        if (userManager is null)
        {
            throw new InvalidOperationException("UserManager<ApplicationUser> is required for seeding.");
        }

        var now = DateTime.UtcNow;
        var sampleUsers = new[]
        {
            new ApplicationUser
            {
                Id = 1,
                UserName = "alex.rivera@taskflow.local",
                Email = "alex.rivera@taskflow.local",
                EmailConfirmed = true,
                DisplayName = "Alex Rivera",
                IsActive = true,
                CreatedAtUtc = now
            },
            new ApplicationUser
            {
                Id = 2,
                UserName = "jamie.chen@taskflow.local",
                Email = "jamie.chen@taskflow.local",
                EmailConfirmed = true,
                DisplayName = "Jamie Chen",
                IsActive = true,
                CreatedAtUtc = now
            },
            new ApplicationUser
            {
                Id = 3,
                UserName = "samir.patel@taskflow.local",
                Email = "samir.patel@taskflow.local",
                EmailConfirmed = true,
                DisplayName = "Samir Patel",
                IsActive = true,
                CreatedAtUtc = now
            }
        };

        foreach (var sampleUser in sampleUsers)
        {
            var existingIdentity = await userManager.Users
                .FirstOrDefaultAsync(x => x.Id == sampleUser.Id || x.Email == sampleUser.Email, cancellationToken);

            if (existingIdentity is not null)
            {
                var shouldUpdate = existingIdentity.DisplayName != sampleUser.DisplayName
                    || existingIdentity.IsActive != sampleUser.IsActive
                    || existingIdentity.CreatedAtUtc != sampleUser.CreatedAtUtc;

                if (shouldUpdate)
                {
                    existingIdentity.DisplayName = sampleUser.DisplayName;
                    existingIdentity.IsActive = sampleUser.IsActive;
                    existingIdentity.CreatedAtUtc = sampleUser.CreatedAtUtc;
                    await userManager.UpdateAsync(existingIdentity);
                }

                continue;
            }

            var result = await userManager.CreateAsync(sampleUser, seedPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Failed to seed identity user '{sampleUser.Email}': {errors}");
            }
        }

        if (!await dbContext.WorkRequests.AnyAsync(cancellationToken))
        {
            var alexId = await userManager.Users
                .Where(x => x.Email == "alex.rivera@taskflow.local")
                .Select(x => x.Id)
                .SingleAsync(cancellationToken);

            var jamieId = await userManager.Users
                .Where(x => x.Email == "jamie.chen@taskflow.local")
                .Select(x => x.Id)
                .SingleAsync(cancellationToken);

            var samirId = await userManager.Users
                .Where(x => x.Email == "samir.patel@taskflow.local")
                .Select(x => x.Id)
                .SingleAsync(cancellationToken);

            var requestOne = new WorkRequest
            {
                Id = 1,
                Title = "Rotate internal API keys",
                Description = "Complete scheduled key rotation for internal integrations.",
                Priority = Priority.High,
                RequestedByUserId = alexId,
                CreatedAtUtc = now.AddDays(-2)
            };

            var requestTwo = new WorkRequest
            {
                Id = 2,
                Title = "Refresh onboarding checklist",
                Description = "Update checklist based on the latest security baseline.",
                Priority = Priority.Medium,
                RequestedByUserId = jamieId,
                CreatedAtUtc = now.AddDays(-1)
            };

            requestTwo.AssignTo(samirId);
            requestTwo.ChangeStatus(WorkRequestStatus.InProgress);

            var notes = new List<RequestNote>
            {
                new()
                {
                    Id = 1,
                    WorkRequestId = 1,
                    AuthorUserId = alexId,
                    Body = "Coordinate with ops before key cutover.",
                    CreatedAtUtc = now.AddDays(-2)
                },
                new()
                {
                    Id = 2,
                    WorkRequestId = 2,
                    AuthorUserId = samirId,
                    Body = "Draft complete, pending review.",
                    CreatedAtUtc = now.AddHours(-6)
                }
            };

            await dbContext.WorkRequests.AddRangeAsync([requestOne, requestTwo], cancellationToken);
            await dbContext.RequestNotes.AddRangeAsync(notes, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
