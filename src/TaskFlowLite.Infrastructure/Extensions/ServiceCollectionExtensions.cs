using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Infrastructure.Persistence;
using TaskFlowLite.Infrastructure.Services;

namespace TaskFlowLite.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskFlowInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskFlowLite") ?? "Data Source=taskflowlite.db";

        services.AddDbContext<TaskFlowLiteDbContext>(options => options.UseSqlite(connectionString));
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkRequestService, WorkRequestService>();
        services.AddScoped<IRequestNoteService, RequestNoteService>();

        return services;
    }
}
