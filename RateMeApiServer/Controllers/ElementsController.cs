using Microsoft.AspNetCore.Mvc;
using RateMeApiServer.Common;
using RateMeApiServer.Services;
using RateMeShared.Dto;

namespace RateMeApiServer.Controllers;

[ApiController]
[Route("api/users/{userId:int}/subjects/elements")]
public class ElementsController : ControllerBase
{
    private IElementService _service;

    public ElementsController(IElementService service)
    {
        _service = service;
    }
    
    
    /// <summary>
    /// Adds elems to subjects of the given ids. Non-existing subjects ids are skipped.
    /// </summary>
    /// <response code="200">Returns obj with created subjects' and elements' ids.</response>
    /// <response code="404">If none of the subjects ids exist.</response>
    [HttpPost]
    public async Task<IActionResult> AddElems(int userId, Dictionary<int, IEnumerable<ElementDto>> elemsBySubjKeys)
    {
        DbInteractionResult<Dictionary<int, int>> interaction = await _service.AddElemsAsync(elemsBySubjKeys);
        
        switch (interaction.Status)
        {
            case DbInteractionStatus.Success: return Ok(interaction.Value);
            case DbInteractionStatus.NotFound: return NotFound("None of the subjects ids exist");
            default: return StatusCode(500);
        }
    }
    
    
    /// <summary>
    /// Updates given elems. Ignores non-existing elems.
    /// </summary>
    /// <response code="204">If everything is okay.</response>
    /// <response code="404">If none of the given elems' ids exist.</response>
    [HttpPut]
    public async Task<IActionResult> UpdateElems(int userId, IEnumerable<PlainElem> elems)
    {
        DbInteractionStatus status = await _service.UpdateElemsAsync(elems);
        
        switch (status)
        {
            case DbInteractionStatus.Success: return NoContent();
            case DbInteractionStatus.NotFound: return NotFound("None of the given elems' ids exist");
            default: return StatusCode(500);
        }
    }  
    
    
    /// <summary>
    /// Removes elems of the given keys.
    /// </summary>
    /// <response code="204">If removed.</response>
    /// <response code="404">If none of the given elems' ids exist.</response>
    [HttpPost("delete")]
    public async Task<IActionResult> RemoveSubjects(int userId, HashSet<int> keys)
    {
        DbInteractionStatus status = await _service.RemoveElemsByKeys(keys);
        
        switch (status)
        {
            case DbInteractionStatus.Success: return NoContent();
            case DbInteractionStatus.NotFound: return NotFound("None of the given elems' ids exist");
            default: return StatusCode(500);
        }
    }
}