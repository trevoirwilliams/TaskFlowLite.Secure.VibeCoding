using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Infrastructure.Identity;

namespace TaskFlowLite.Infrastructure.Persistence;

public class TaskFlowLiteDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public TaskFlowLiteDbContext(DbContextOptions<TaskFlowLiteDbContext> options)
        : base(options)
    {
    }

    public new DbSet<User> Users => Set<User>();
    public DbSet<WorkRequest> WorkRequests => Set<WorkRequest>();
    public DbSet<RequestNote> RequestNotes => Set<RequestNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AppUsers");
            entity.Property(x => x.DomainUserId).IsRequired();
            entity.HasIndex(x => x.DomainUserId).IsUnique();

            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<ApplicationUser>(x => x.DomainUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdentityRole<int>>().ToTable("AppRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("AppUserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("AppUserLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("AppUserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("AppRoleClaims");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("AppUserRoles");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<WorkRequest>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.Priority);
            entity.HasIndex(x => x.AssignedToUserId);
            entity.HasIndex(x => x.CreatedAtUtc);

            entity.HasOne(x => x.RequestedByUser)
                .WithMany(u => u.RequestedWorkRequests)
                .HasForeignKey(x => x.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AssignedToUser)
                .WithMany(u => u.AssignedWorkRequests)
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RequestNote>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Body).HasMaxLength(2000).IsRequired();
            entity.HasIndex(x => x.CreatedAtUtc);

            entity.HasOne(x => x.WorkRequest)
                .WithMany(r => r.Notes)
                .HasForeignKey(x => x.WorkRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.AuthorUser)
                .WithMany(u => u.RequestNotes)
                .HasForeignKey(x => x.AuthorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
