using Microsoft.AspNetCore.Mvc;
using RateMeShared.Dto;

namespace RateMeApiServer.Controllers;

[ApiController]
[Route("api/users/{id}/subjects")]
public class SubjectsController : ControllerBase
{
    /// <summary>
    /// Adds all user's subjects.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddAllSubjects(SubjectsByUserId subjectsObj)
    {
        return Ok(); // Obj with subjects' gained ids
    }
}