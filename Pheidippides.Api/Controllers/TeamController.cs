using Microsoft.AspNetCore.Mvc;
using Pheidippides.Api.Contracts.Common;
using Pheidippides.Api.Contracts.Team;

namespace Pheidippides.Api.Controllers;

[Route("api/team")]
public class TeamController : Controller
{
    [HttpGet("get")]
    public async Task<ActionResult<TeamDto>> GetTeam(CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        return Ok(new TeamDto
        {
            Id = 1,
            Name = "teamName",
            InviteToken = "InviteToken",
            Lead = new UserDto
            {
                Id = 1,
                PhoneNumber = "PhoneNumber",
                FirstName = "FirstName",
                SecondName = "SecondName",
                Surname = "Surname",
                IsDuty = true,
                TeamId = 1
            },
            Members =
            [
                new UserDto
                {
                    Id = 2,
                    PhoneNumber = "PhoneNumber",
                    FirstName = "FirstName",
                    SecondName = "SecondName",
                    Surname = "Surname",
                    IsDuty = true,
                    TeamId = 1
                },

                new UserDto
                {
                    Id = 2,
                    PhoneNumber = "PhoneNumber",
                    FirstName = "FirstName",
                    SecondName = "SecondName",
                    Surname = "Surname",
                    IsDuty = false,
                    TeamId = 1
                }
            ],
            LeadRotationRule = LeadRotationRule.LeadIsNotDuty
        });
    }
}