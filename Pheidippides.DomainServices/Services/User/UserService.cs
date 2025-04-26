using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;
using Pheidippides.Domain.Exceptions;
using Pheidippides.Domain.Utils;
using Pheidippides.DomainServices.Extensions;
using Pheidippides.DomainServices.Services.Auth;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.User;

public class UserService(AppDbContext appDbContext, AuthService authService)
{
    public async Task<string> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (command is { TeamInviteCode: not null, TeamName: not null })
            throw new BadRequestException("TeamInviteCode or TeamName must be null");
        
        // var codeIsValid = await appDbContext.FlashCallCodes.AnyAsync(x =>
        //         x.PhoneNumber == PhoneNumberUnifier.Standardize(command.PhoneNumber)
        //         && x.Code == command.PhoneActivationCode,
        //     cancellationToken);
        //
        // if (!codeIsValid)
        //     throw new ForbiddenException("Invalid phone activation code");

        var userExist = await appDbContext.Users.AnyAsync(
            x => x.PhoneNumber == PhoneNumberUnifier.Standardize(command.PhoneNumber),
            cancellationToken);

        if (userExist)
            throw new ConflictException("User with this phone number already exists");

        Domain.User user;

        if (command.TeamName is not null)
        {
            user = CreateUser(command, UserRole.Lead);
            var team = CreateTeamAndAddLead(command, user);

            appDbContext.Teams.Add(team);
            appDbContext.Users.Add(user);
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

            team.Workers.Add(user);
        }
        else throw new BadRequestException("The team invitation code or team name is not specified.");

        await appDbContext.SaveChangesAsync(cancellationToken);

        var id = await appDbContext.Users
            .Where(x => x.PhoneNumber == PhoneNumberUnifier.Standardize(command.PhoneNumber))
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        return authService.GenerateJwtToken(id, user.Role);
    }

    public async Task UpdateEmail(long userId, string email, CancellationToken cancellationToken)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        ArgumentNullException.ThrowIfNull(user);
        
        user.Email = email;
        
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateYandexIntegration(
        long userId, 
        string yandexScenarioId, 
        string yandexOAuthToken, 
        CancellationToken cancellationToken)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        ArgumentNullException.ThrowIfNull(user);
        
        user.YandexScenarioName = yandexScenarioId;
        user.YandexOAuthToken = yandexOAuthToken;
        
        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ThrowIfUserNotExist(
        long userId, 
        bool isDomainError = false,
        CancellationToken cancellationToken = default)
    {
        var userExist = await appDbContext.Users.AnyAsync(x => x.Id == userId, cancellationToken);

        switch (userExist)
        {
            case false when isDomainError:
                throw new NotFoundException("User with this id does not exist");
            case false:
                throw new ArgumentException("User with this id does not exist");
        }
    }

    private static Team CreateTeamAndAddLead(RegisterCommand command, Domain.User user)
        => new()
        {
            Name = command.TeamName ?? throw new ArgumentNullException(nameof(command)),
            InviteToken = GenerateSecureToken(),
            Lead = user,
            LeadRotationRule = LeadRotationRule.LeadInRotation,
            IncidentCreateToken = GenerateSecureToken()
        };

    private static Domain.User CreateUser(RegisterCommand command, UserRole userRole)
        => new()
        {
            PhoneNumber = PhoneNumberUnifier.Standardize(command.PhoneNumber),
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