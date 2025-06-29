using Microsoft.AspNetCore.Mvc;
using RateMeApiServer.Common;
using RateMeApiServer.Services;
using RateMeShared.Dto;

namespace RateMeApiServer.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        
        /// <summary>
        /// Gets user by id.
        /// </summary>
        /// <response code="200">Returns user</response>
        /// <response code="404">Such id doesn't exist</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            DbInteractionResult<UserDto> interaction = await _userService.GetUserByIdAsync(id);

            switch (interaction.Status)
            {
                case DbInteractionStatus.Success: return Ok(interaction.Value);
                case DbInteractionStatus.NotFound: return NotFound();
                default: return StatusCode(500);
            }
        }

        
        /// <summary>
        /// Signs up user
        /// </summary>
        /// <response code="200">Returns the newly created user's id</response>
        /// <response code="409">If this email is already taken</response>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserDto userRequestDto)
        {
            DbInteractionResult<int> interaction = await _userService.AddUserAsync(userRequestDto);

            switch (interaction.Status)
            {
                case DbInteractionStatus.Success: 
                    return CreatedAtAction(nameof(GetById), new {id = interaction.Value}, new {id = interaction.Value});
                
                case DbInteractionStatus.Conflict: return Conflict("User with such email already exists");
                default: return StatusCode(500);
            }
        }

        
        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <response code="200">Returns authenticated user</response>
        /// <response code="404">If such email was not found</response>
        /// <response code="401">If password is wrong</response>
        [HttpPost("auth")]
        public async Task<IActionResult> SignIn(AuthRequest authRequest)
        {
            DbInteractionResult<int> interaction = await _userService.AuthUserAsync(authRequest);
            
            switch (interaction.Status)
            {
                case DbInteractionStatus.Success: return Ok(new {id = interaction.Value});
                case DbInteractionStatus.NotFound: return NotFound("User with such email doesn't exist");
                case DbInteractionStatus.WrongData: return Unauthorized("Wrong password");
                default: return StatusCode(500);
            }
        }
    }
}
