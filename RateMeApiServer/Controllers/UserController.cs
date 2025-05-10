using Microsoft.AspNetCore.Mvc;
using RateMeApiServer.Models.Dto;
using RateMeApiServer.Services;

namespace RateMeApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                UserDto user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        
        /// <summary>
        /// Signs up user
        /// </summary>
        /// <response code="200">Returns the newly created user's id</response>
        /// <response code="409">If this email is already taken</response>
        [HttpPost("signup")]
        public async Task<IActionResult> Add(UserDto userRequestDto)
        {
            try
            {
                int addedId = await _userService.AddUserAsync(userRequestDto);
                return Ok(addedId);
            }
            catch (InvalidDataException e)
            {
                return Conflict(e.Message);
            }
            
        }

        
        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <response code="200">Returns authenticated user</response>
        /// <response code="404">If such email was not found</response>
        /// <response code="401">If password is wrong</response>
        [HttpPost("auth")]
        public async Task<IActionResult> LogIn(AuthRequestDto authRequest)
        {
            try
            {
                UserDto dto = await _userService.AuthUserAsync(authRequest);
                return Ok(dto);
            }
            catch (InvalidOperationException e)
            {
                return NotFound("User with such email doesn't exist");
            }
            catch (InvalidDataException)
            {
                return Unauthorized("Wrong password");
            }
        }
    }
}
