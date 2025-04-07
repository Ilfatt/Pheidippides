using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;
using Pheidippides.Domain.Exceptions;
using Pheidippides.DomainServices.Extensions;
using Pheidippides.DomainServices.Services.Auth;
using Pheidippides.ExternalServices;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.User;

public class UserService(ZvonokClient zvonokClient, AppDbContext appDbContext, AuthService authService)
{
    public async Task<string> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var codeFromZvonok = await zvonokClient.GetFlashCallCode(command.PhoneNumber, cancellationToken);

        if (codeFromZvonok != command.PhoneActivationCode)
            throw new ForbiddenException("Invalid phone activation code");

        Domain.User user;

        if (command.TeamName is not null)
        {
            user = CreateUser(command, UserRole.Lead);
            var team = CreateTeamAndAddLead(command, user);
            
            user.LeadTeam = team;
            
            await appDbContext.Users.AddAsync(user, cancellationToken);
        }
        else if (command.TeamInviteCode is not null)
        {
            user = CreateUser(command, UserRole.Worker);
            var team = await appDbContext.Teams
                           .Include(x => x.Workers)
                           .FirstOrDefaultAsync(
                               x => x.InviteToken == command.TeamInviteCode,
                               cancellationToken)
                       ?? throw new ForbiddenException("Invalid team invite code");

            await appDbContext.Users.AddAsync(user, cancellationToken);
            team.Workers.Add(user);
        }
        else throw new BadRequestException("The team invitation code or team name is not specified.");

        await appDbContext.SaveChangesAsync(cancellationToken);
        return authService.GenerateJwtToken(user.Id, user.Role);
    }

    private static Team CreateTeamAndAddLead(RegisterCommand command, Domain.User user)
        => new()
        {
            Name = command.TeamName ?? throw new ArgumentNullException(nameof(command)),
            InviteToken = GenerateSecureToken(),
            Lead = user
        };

    private static Domain.User CreateUser(RegisterCommand command, UserRole userRole)
        => new()
        {
            PhoneNumber = command.PhoneNumber,
            PasswordHash = command.Password.HashSha256(),
            FirstName = command.FirstName,
            SecondName = command.SecondName,
            Surname = command.Surname,
            Role = userRole,
        };

    private static string GenerateSecureToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber).HashSha256();
    }
}