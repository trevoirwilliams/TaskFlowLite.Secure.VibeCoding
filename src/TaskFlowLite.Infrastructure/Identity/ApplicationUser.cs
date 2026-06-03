using Microsoft.AspNetCore.Identity;

namespace TaskFlowLite.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public int DomainUserId { get; set; }
}
