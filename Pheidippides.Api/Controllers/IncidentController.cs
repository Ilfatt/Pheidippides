using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Incidents;
using Pheidippides.Api.Extensions;
using Pheidippides.Domain;
using Pheidippides.DomainServices.Services.Incidents;
using Swashbuckle.AspNetCore.Annotations;

namespace Pheidippides.Api.Controllers;

[Route("api/incident")]
public class IncidentController(IncidentService service, IHttpContextAccessor contextAccessor) : Controller
{
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Level cannot be zero or empty")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Team with this IncidentCreateToken not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateIncident(
        [FromBody] CreateNewIncidentDto dto,
        CancellationToken cancellationToken)
    {
        if(dto.Level == 0) 
            return BadRequest("Level cannot be zero or empty");
        
        await service.CreateIncident(new CreateNewIncidentCommand
            {
                Title = dto.Title,
                Description = dto.Description,
                Level = dto.Level,
                IncidentCreateToken = dto.IncidentCreateToken,
            },
            cancellationToken);

        return NoContent();
    }

    [HttpPut("acknowledge")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The user was not called for this incident")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Acknowledge([FromQuery] long incidentId, CancellationToken cancellationToken)
    {
        await service.Acknowledge(contextAccessor.HttpContext.GetUserId(), incidentId, cancellationToken);

        return NoContent();
    }

    [HttpGet("get_history")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(IncidentDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetHistory(CancellationToken cancellationToken)
    {
        var userId = contextAccessor.HttpContext.GetUserId();
        
        var incidents = await service.GetHistory(userId, cancellationToken);

        return Ok(incidents.Select(x => new IncidentDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Level = x.Level,
                CreatedAt = x.CreatedAt,
                ClosedAt = x.ClosedAt,
                NeedAcknowledgeCurrentUser =
                    (x.AdditionallyNeedAcknowledgedUsers.Contains(userId)
                    || x.Team.DutyId == userId
                    || x.Team.LeadRotationRule == LeadRotationRule.LeadIsDuty && x.Team.LeadId == userId)
                && !x.AcknowledgedUsers.Select(user => user.Id).Contains(userId)
            })
            .ToArray());
    }
}