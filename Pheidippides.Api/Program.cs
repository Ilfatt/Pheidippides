using Pheidippides.Api.Extensions;
using Pheidippides.Api.Middlewares;
using Pheidippides.Domain;
using Pheidippides.DomainServices.Notifiers;
using Pheidippides.DomainServices.Services.Auth;
using Pheidippides.DomainServices.Services.Teams;
using Pheidippides.DomainServices.Services.User;
using Pheidippides.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigurePostgresqlConnection();
builder.ConfigureAuthentication();
builder.AddCustomSwagger();

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddSingleton<ZvonokClient>();
builder.Services.AddSingleton<YandexApiClient>();

builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddSingleton<INotifier, PhoneCallNotifier>();
builder.Services.AddSingleton<INotifier, YandexHomeStationNotifier>();

var app = builder.Build();

await app.MigrateDbAsync();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x =>
{
    x.AllowAnyMethod();
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
});

app.Run();