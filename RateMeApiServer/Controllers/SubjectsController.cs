using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Services;
using RateMeShared.Dto;

namespace RateMeApiServer.Controllers;

[ApiController]
[Route("api/users/{id}/subjects")]
public class SubjectsController : ControllerBase
{
    private ISubjectService _service;

    public SubjectsController(ISubjectService service)
    {
        _service = service;
    }
    
    
    /// <summary>
    /// Adds all user's subjects.
    /// </summary>
    /// <response code="200">Returns obj with created subjects' and elements' ids.</response>
    /// <response code="404">If user with such id doesn't exist.</response>
    [HttpPost]
    public async Task<IActionResult> AddAllSubjects(SubjectsByUserId subjectsObj)
    {
        try
        {
           SubjectsIds subjIds = await _service.AddSubjectsAsync(subjectsObj);
           return Ok(subjIds);
        }
        catch (InvalidDataException e)
        {
            return NotFound(e.Message);
        }
    }
    
    
    /// <summary>
    /// Removes subjects of the given keys.
    /// </summary>
    /// <response code="204">If removed.</response>
    /// <response code="404">If some keys weren't found.</response>
    [HttpPost("delete")]
    public async Task<IActionResult> RemoveSubjects(PlainKeys subjKeys)
    {
        try
        {
            await _service.RemoveSubjectsAsync(subjKeys);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return NotFound($"Some of the keys dont exist in bd: {e.Message}");
        }
    }
}