using Microsoft.AspNetCore.Mvc;
using RateMeApiServer.Common;
using RateMeApiServer.Services;
using RateMeShared.Dto;

namespace RateMeApiServer.Controllers;

[ApiController]
[Route("api/users/{userId:int}/subjects")]
public class SubjectsController : ControllerBase
{
    private ISubjectService _service;

    public SubjectsController(ISubjectService service)
    {
        _service = service;
    }
    
    
    /// <summary>
    /// Adds user's subjects.
    /// </summary>
    /// <response code="200">Returns obj with created subjects' and elements' ids.</response>
    /// <response code="404">If user with such id doesn't exist.</response>
    [HttpPost]
    public async Task<IActionResult> AddSubjects(int userId, SubjectDto[] subjects)
    {
        DbInteractionResult<SubjectsIds> interaction = await _service.AddSubjectsAsync(userId, subjects);
        
        switch (interaction.Status)
        {
            case DbInteractionStatus.Success: return Ok(interaction.Value);
            case DbInteractionStatus.NotFound: return NotFound("User with such id was not found");
            default: return StatusCode(500);
        }
    }

    
    /// <summary>
    /// Updates given subjects. Ignores non-existing subjects.
    /// </summary>
    /// <response code="204">If everything is ok.</response>
    /// <response code="404">If user with such id doesn't exist.</response>
    [HttpPut]
    public async Task<IActionResult> UpdateSubjects(int userId, PlainSubject[] subjects)
    {
        DbInteractionStatus status = await _service.UpdateSubjectsAsync(userId, subjects);
        
        switch (status)
        {
            case DbInteractionStatus.Success: return NoContent();
            case DbInteractionStatus.NotFound: return NotFound("User with such id was not found");
            default: return StatusCode(500);
        }
    }  
    
    
    /// <summary>
    /// Removes subjects of the given keys.
    /// </summary>
    /// <response code="204">If removed.</response>
    /// <response code="404">If user with such id doesn't exist.</response>
    [HttpPost("delete")]
    public async Task<IActionResult> RemoveSubjects(int userId, HashSet<int> keys)
    {
        DbInteractionStatus status = await _service.RemoveSubjectsAsync(userId, keys);
        
        switch (status)
        {
            case DbInteractionStatus.Success: return NoContent();
            case DbInteractionStatus.NotFound: return NotFound("User with such id was not found");
            default: return StatusCode(500);
        }
    }
}