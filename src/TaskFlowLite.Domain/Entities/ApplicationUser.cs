using Microsoft.AspNetCore.Identity;

namespace TaskFlowLite.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
