using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.RequestNotes;

namespace TaskFlowLite.Api.Controllers;

[ApiController]
[Route("api/workrequests/{workRequestId:int}/notes")]
[Authorize]
public class RequestNotesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        int workRequestId,
        [FromServices] IWorkRequestService workRequestService,
        [FromServices] IRequestNoteService service,
        CancellationToken cancellationToken)
    {
        var workRequest = await workRequestService.GetByIdAsync(workRequestId, cancellationToken);
        if (workRequest is null)
        {
            return NotFound();
        }

        var notes = await service.GetForWorkRequestAsync(workRequestId, cancellationToken);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        int workRequestId,
        [FromServices] IWorkRequestService workRequestService,
        [FromServices] IRequestNoteService service,
        [FromBody] AddRequestNoteRequest request,
        CancellationToken cancellationToken)
    {
        var workRequest = await workRequestService.GetByIdAsync(workRequestId, cancellationToken);
        if (workRequest is null)
        {
            return NotFound();
        }

        var created = await service.AddAsync(workRequestId, request, cancellationToken);
        return created is null ? NotFound() : Ok(created);
    }
}
