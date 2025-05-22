using Cool.Application.Dtos.Users;
using Cool.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cool.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel request)
    {
        var result = await _userService.CreateUser(request);
        return Ok(result);
    }
}
