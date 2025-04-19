using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Schedule;
using Pheidippides.Api.Extensions;
using Pheidippides.DomainServices.Services.Schedules;
using Swashbuckle.AspNetCore.Annotations;

namespace Pheidippides.Api.Controllers;

[Route("api/schedule")]
public class ScheduleController(
    ScheduleService scheduleService,
    IHttpContextAccessor httpContextAccessor) : Controller
{
    [HttpGet("get")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "OK", typeof(ScheduleDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ScheduleDto>> GetSchedule(CancellationToken cancellationToken)
    {
        var schedule = await scheduleService.GetSchedule(
            httpContextAccessor.HttpContext.GetUserId(),
            cancellationToken);


        return Ok(new ScheduleDto
        {
            ScheduleItems = schedule.ScheduleItems
                .Select(x => new ScheduleItemDto
                {
                    From = x.From,
                    To = x.To,
                    UserId = x.UserId,
                })
                .ToList()
        });
    }
}