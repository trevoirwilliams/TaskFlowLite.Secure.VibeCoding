using Microsoft.AspNetCore.Mvc;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Api.Controllers;

[ApiController]
[Route("api/metadata")]
public class MetadataController : ControllerBase
{
    [HttpGet("priorities")]
    public IActionResult GetPriorities() => Ok(Enum.GetNames<Priority>());

    [HttpGet("statuses")]
    public IActionResult GetStatuses() => Ok(Enum.GetNames<WorkRequestStatus>());
}
