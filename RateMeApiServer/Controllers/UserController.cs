using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RateMeApiServer.Models.Dto;
using RateMeApiServer.Services;
using System.Runtime.InteropServices;

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


        [HttpPost]
        public async Task<IActionResult> Add(UserDto userRequestDto)
        {
            await _userService.AddUserAsync(userRequestDto);
            return CreatedAtAction(nameof(GetById), new { Id = userRequestDto.Id }, userRequestDto);
        }
    }
}
