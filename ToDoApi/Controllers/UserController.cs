using Microsoft.AspNetCore.Mvc;

// Only for admin purposes to get a list of all users registered.
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        if (!result.Success)
            return BadRequest(result.ErrorDescription);

        return Ok(result.Data);
    }
}
