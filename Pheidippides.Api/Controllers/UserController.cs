using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Auth;
using Pheidippides.Api.Contracts.User;
using Pheidippides.Api.Extensions;
using Pheidippides.DomainServices.Services.User;
using Swashbuckle.AspNetCore.Annotations;

namespace Pheidippides.Api.Controllers;

[Route("api/user")]
public class UserController(UserService service, IHttpContextAccessor httpContextAccessor) : Controller
{
    [HttpPost("register")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(JwtTokenDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Exist user with this phone number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwtTokenDto>> Register(NewUserDto request, CancellationToken cancellationToken)
    {
        if (request.PhoneActivationCode > 9999)
            return BadRequest("The phone activation code must consist of 4 digits");

        var command = new RegisterCommand
        {
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            Surname = request.Surname,
            PhoneNumber = request.PhoneNumber,
            PhoneActivationCode = request.PhoneActivationCode,
            Password = request.Password,
            TeamName = request.TeamName,
            TeamInviteCode = request.TeamInviteCode
        };

        var jwt = await service.Register(command, cancellationToken);

        return Ok(new JwtTokenDto { AccessToken = jwt });
    }

    [HttpPut("update_email")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwtTokenDto>> UpdateEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        await service.UpdateEmail(
            httpContextAccessor.HttpContext.GetUserId(),
            email,
            cancellationToken);

        return NoContent();
    }
    
    [HttpPut("update_yandex_integration")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwtTokenDto>> UpdateYandexIntegration(
        [FromQuery] string yandexScenarioId, 
        [FromQuery] string yandexOAuthToken, 
        CancellationToken cancellationToken)
    {
        await service.UpdateYandexIntegration(
            httpContextAccessor.HttpContext.GetUserId(),
            yandexScenarioId,
            yandexOAuthToken,
            cancellationToken);

        return NoContent();
    }
}