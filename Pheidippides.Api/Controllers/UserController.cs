using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.User;

namespace Pheidippides.Api.Controllers;

[Route("api/[controller]")]
public class UserController : Controller
{
    [HttpPost("register")]
    public async Task<ActionResult<JwtTokenDto>> Register(NewUserDto newUserDto, CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        return Ok(new JwtTokenDto { AccessToken = "111" });
    }

    [HttpGet("login")]
    public async Task<ActionResult<JwtTokenDto>> Register(LoginDto loginDto, CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        return Ok(new JwtTokenDto { AccessToken = "111" });
    }
}