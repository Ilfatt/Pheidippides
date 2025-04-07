using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Auth;
using Pheidippides.DomainServices.Services.Auth;
using Pheidippides.DomainServices.Services.User;

namespace Pheidippides.Api.Controllers;

[Route("api/auth")]
public class AuthController(AuthService service) : Controller
{
    [HttpPost("send_activation_code")]
    public async Task<ActionResult> SendCode(SendActivationCodeDto request, CancellationToken cancellationToken)
    {
        await service.SendCode(request.PhoneNumber, cancellationToken);

        return Ok();
    }

    [HttpGet("login")]
    public async Task<ActionResult<JwtTokenDto>> Login(LoginDto request, CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        return Ok(new JwtTokenDto { AccessToken = "111" });
    }
}