using Microsoft.AspNetCore.Mvc;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.WorkRequests;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Api.Controllers;

[ApiController]
[Route("api/workrequests")]
public class WorkRequestsController : ControllerBase
{
    private const int MaxSearchLength = 100;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] IWorkRequestService service,
        [FromQuery] WorkRequestStatus? status,
        [FromQuery] Priority? priority,
        [FromQuery] int? assignedToUserId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var normalizedSearch = search?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedSearch))
        {
            normalizedSearch = null;
        }

        if (normalizedSearch is not null && normalizedSearch.Length > MaxSearchLength)
        {
            ModelState.AddModelError(nameof(search), $"The search query cannot exceed {MaxSearchLength} characters.");
            return ValidationProblem(ModelState);
        }

        var items = await service.GetAsync(status, priority, assignedToUserId, normalizedSearch, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        [FromServices] IWorkRequestService service,
        CancellationToken cancellationToken)
    {
        var item = await service.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] IWorkRequestService service,
        [FromBody] CreateWorkRequestRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromServices] IWorkRequestService service,
        [FromBody] UpdateWorkRequestRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await service.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> Assign(
        int id,
        [FromServices] IWorkRequestService service,
        [FromBody] AssignWorkRequestRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await service.AssignAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromServices] IWorkRequestService service,
        [FromBody] UpdateWorkRequestStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await service.UpdateStatusAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Title = "Invalid status transition.",
                Detail = ex.Message
            });
        }
    }
}
