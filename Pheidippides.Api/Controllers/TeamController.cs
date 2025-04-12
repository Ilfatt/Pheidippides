using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Common;
using Pheidippides.Api.Contracts.Team;
using Pheidippides.Api.Extensions;
using Pheidippides.Domain;
using Pheidippides.DomainServices.Services.Teams;
using Swashbuckle.AspNetCore.Annotations;

namespace Pheidippides.Api.Controllers;

[Route("api/team")]
public class TeamController(
    TeamService teamService,
    IHttpContextAccessor httpContextAccessor) : Controller
{
    [HttpGet("get")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(TeamDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamDto>> GetTeam(CancellationToken cancellationToken)
    {
        var team = await teamService.GetUserTeamWithMember(
            httpContextAccessor.HttpContext.GetUserId(),
            cancellationToken);
        
        return Ok(new TeamDto
        {
            Name = team.Name,
            InviteToken = team.InviteToken,
            LeadRotationRule = team.LeadRotationRule,
            Lead = ToDto(team.Lead),
            Workers = team.Workers.Select(ToDto).ToList(),
        });
    }

    private static UserDto ToDto(User user)
        => new() 
        {
            Id = user.Id,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            Surname = user.Surname,
            IsDuty = user.IsDuty,
            TeamId = user.TeamId ?? user.LeadTeamId,
            Role = user.Role,
            YandexScenarioName = user.YandexScenarioName,
            YandexOAuthToken = user.YandexOAuthToken,
            CreatedAt = user.CreatedAt,
            Email = user.Email
        };
}