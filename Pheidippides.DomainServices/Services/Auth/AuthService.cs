using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pheidippides.Domain;
using Pheidippides.Domain.Exceptions;
using Pheidippides.Domain.Utils;
using Pheidippides.DomainServices.Extensions;
using Pheidippides.ExternalServices;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.Auth;

public class AuthService(
    IConfiguration configuration,
    AppDbContext appDbContext,
    ZvonokClient zvonokClient)
{
    public async Task SendCode(string phoneNumber, CancellationToken cancellationToken)
    {
        phoneNumber = PhoneNumberUnifier.Standardize(phoneNumber);
        
        var phoneIsAlreadyInUse = await appDbContext.Users
            .AnyAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);

        if (phoneIsAlreadyInUse)
            throw new ConflictException("Phone number is already in use");
        
        var code = await zvonokClient.FlashCall(phoneNumber, cancellationToken);

        appDbContext.FlashCallCodes.Add(new FlashCallCodes { Code = code, PhoneNumber = phoneNumber });

        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> Login(string phoneNumber, string password, CancellationToken cancellationToken)
    {
        phoneNumber = PhoneNumberUnifier.Standardize(phoneNumber);
        var user = await appDbContext.Users.FirstOrDefaultAsync(
            x => x.PhoneNumber == phoneNumber,
            cancellationToken);

        if (user == null)
            throw new NotFoundException("User with this phone number not found");

        if (user.PasswordHash == password.HashSha256())
            return GenerateJwtToken(user.Id, user.Role);

        throw new UnauthorizedException("Invalid password");
    }

    public string GenerateJwtToken(long userId, UserRole role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityKey = Encoding.ASCII.GetBytes(
            configuration["AuthOptions:JwtSecretKey"] ?? throw new ArgumentException("Missing JWT Secret Key"));

        var claims = new List<Claim>
        {
            new(nameof(ClaimTypes.NameIdentifier), userId.ToString(CultureInfo.InvariantCulture)),
            new(nameof(ClaimTypes.Role), ((int)role).ToString(CultureInfo.InvariantCulture))
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = configuration["AuthOptions:Audience"] ?? throw new ArgumentException("Missing JWT Audience"),
            Issuer = configuration["AuthOptions:Issuer"] ?? throw new ArgumentException("Missing JWT Issuer"),
            Subject = new ClaimsIdentity(claims.ToArray()),
            Expires = DateTime.MaxValue,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtSecurityKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}