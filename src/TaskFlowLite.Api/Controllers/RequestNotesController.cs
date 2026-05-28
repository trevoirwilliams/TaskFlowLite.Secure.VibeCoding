using Microsoft.AspNetCore.Mvc;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.RequestNotes;

namespace TaskFlowLite.Api.Controllers;

[ApiController]
[Route("api/workrequests/{workRequestId:int}/notes")]
public class RequestNotesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        int workRequestId,
        [FromServices] IRequestNoteService service,
        CancellationToken cancellationToken)
    {
        var notes = await service.GetForWorkRequestAsync(workRequestId, cancellationToken);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        int workRequestId,
        [FromServices] IRequestNoteService service,
        [FromBody] AddRequestNoteRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.AddAsync(workRequestId, request, cancellationToken);
        return created is null ? NotFound() : Ok(created);
    }
}
