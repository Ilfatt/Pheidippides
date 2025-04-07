using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Common;
using Pheidippides.Api.Contracts.Schedule;

namespace Pheidippides.Api.Controllers;

[Route("api/schedule")]
public class ScheduleController : Controller
{
    [HttpGet("get")]
    public async Task<ActionResult<ScheduleDto>> GetSchedule(CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        return Ok(new ScheduleDto
        {
            LeadRotationRule = LeadRotationRule.LeadIsNotDuty,
            ScheduleItems = new List<ScheduleItemDto>()
            {
                new ScheduleItemDto
                {
                    From = DateTimeOffset.UtcNow,
                    To = DateTimeOffset.UtcNow.AddDays(3),
                    UserId = 1
                },
                new ScheduleItemDto
                {
                    From = DateTimeOffset.UtcNow.AddDays(3),
                    To = DateTimeOffset.UtcNow.AddDays(6),
                    UserId = 2
                }
            }
        });
    }
}