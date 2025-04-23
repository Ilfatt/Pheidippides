using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Auth;
using Pheidippides.Api.Contracts.Common;
using Pheidippides.DomainServices.Services.Auth;
using Swashbuckle.AspNetCore.Annotations;

namespace Pheidippides.Api.Controllers;

[Route("api/auth")]
public class AuthController(AuthService service) : Controller
{
    [HttpPost("send_activation_code")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Exist user with this phone number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendCode(
        [FromBody] SendActivationCodeDto request,
        CancellationToken cancellationToken)
    {
        await service.SendCode(request.PhoneNumber, cancellationToken);

        return NoContent();
    }

    [HttpGet("login")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid password")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User does not exist")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwtTokenDto>> Login(LoginDto request, CancellationToken cancellationToken)
    {
        var token = await service.Login(request.PhoneNumber, request.Password, cancellationToken);

        return Ok(new JwtTokenDto { AccessToken = token });
    }
}