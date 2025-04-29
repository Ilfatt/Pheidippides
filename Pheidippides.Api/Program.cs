using Pheidippides.Api.Extensions;
using Pheidippides.Api.Jobs;
using Pheidippides.Api.Middlewares;
using Pheidippides.DomainServices.Notifiers;
using Pheidippides.DomainServices.Services.Auth;
using Pheidippides.DomainServices.Services.Incidents;
using Pheidippides.DomainServices.Services.Schedules;
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
builder.Services.AddSingleton<SmtpClient>();

builder.Services.AddHostedService<UpdateSchedulesJob>();
builder.Services.AddHostedService<NotifyJob>();

builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<IncidentService>();

builder.Services.AddSingleton<INotifier, PhoneCallNotifier>();
builder.Services.AddSingleton<INotifier, YandexHomeStationNotifier>();
builder.Services.AddSingleton<INotifier, EmailNotifier>();

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