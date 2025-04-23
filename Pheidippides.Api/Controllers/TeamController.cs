using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Common;
using Pheidippides.Api.Contracts.Team;
using Pheidippides.Api.Extensions;
using Pheidippides.Domain;
using Pheidippides.Domain.Exceptions;
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
            Lead = ToDto(team.Lead, team),
            Workers = team.Workers.Select(x => ToDto(x, team)).ToList(),
            IncidentCreateToken = team.IncidentCreateToken,
        });
    }

    [HttpPut("set_lead_rotation_rule")]
    [Authorize(Roles = nameof(UserRole.Lead))]
    [SwaggerOperation(Summary = "0 = LeadIsNotDuty, 1 = LeadIsDuty, 2 = LeadInRotation")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "LeadRotationRule cannot chage worker.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetLeadRotationRule(
        [FromQuery] LeadRotationRule leadRotationRule,
        CancellationToken cancellationToken)
    {
        if(httpContextAccessor.HttpContext.GetUserRole() != UserRole.Lead)
            throw new ForbiddenException("You are not an Lead.");
        
        await teamService.SetLeadRotationRule(
            httpContextAccessor.HttpContext.GetUserId(),
            leadRotationRule,
            cancellationToken);

        return NoContent();
    }

    private static UserDto ToDto(User user, Team team)
        => new()
        {
            Id = user.Id,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            Surname = user.Surname,
            IsDuty = team.DutyId == user.Id,
            TeamId = user.TeamId ?? user.LeadTeamId,
            Role = user.Role,
            YandexScenarioName = user.YandexScenarioName,
            YandexOAuthToken = user.YandexOAuthToken,
            CreatedAt = user.CreatedAt,
            Email = user.Email
        };
}