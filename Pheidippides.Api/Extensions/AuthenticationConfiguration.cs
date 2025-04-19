using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Pheidippides.Domain;

namespace Pheidippides.Api.Extensions;

public static class AuthenticationConfiguration
{
    /// <summary>
    ///     Конфигурация аутентификации
    /// </summary>
    /// <param name="builder">WebApplicationBuilder</param>
    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                if (builder.Environment.IsDevelopment()) options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetSection("AuthOptions")
                        .GetValue<string>("Issuer"),
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration.GetSection("AuthOptions")
                        .GetValue<string>("Audience"),
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            builder.Configuration.GetSection("AuthOptions")
                                .GetValue<string>("JwtSecretKey")!)),
                    ValidateIssuerSigningKey = true
                };
            });
    }
}