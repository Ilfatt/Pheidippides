using System.Net;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pheidippides.Api.Extensions;
using Pheidippides.Domain.Exceptions;

namespace Pheidippides.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Обработчик ошибок API
    /// </summary>
    /// <param name="next">Делегат следующего обработчика в пайплайне ASP.NET Core</param>
    /// <param name="loggerFactory">Фабрика логгеров</param>
    /// <param name="hostingEnvironment">Хост</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILoggerFactory loggerFactory,
        IWebHostEnvironment hostingEnvironment)
    {
        _next = next;
        _loggerFactory = loggerFactory;
        _hostingEnvironment = hostingEnvironment;
    }

    /// <summary>
    ///     Действия по обработке запроса ASP.NET
    /// </summary>
    /// <param name="context">Контекст запроса ASP.NET</param>
    /// <returns>Задача на обработку запроса ASP.NET</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OutOfMemoryException)
        {
        }
        catch (StackOverflowException)
        {
        }
        catch (BadHttpRequestException ex)
        {
            await HandleBadHttpRequestExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    ///     Создать логгер для контроллера, исполнявшего запрос
    /// </summary>
    /// <param name="context">Контекст запроса</param>
    /// <returns>Логгер</returns>
    private ILogger GetLogger(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<MethodInfo>();
        var controllerType = controllerActionDescriptor?.DeclaringType;
        return controllerType != null
            ? _loggerFactory.CreateLogger(controllerType)
            : _loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
    }

    private async Task LogAndReturnAsync(
        HttpContext context,
        Exception exception,
        string errorText,
        HttpStatusCode responseCode,
        LogLevel logLevel,
        Dictionary<string, object>? details = null)
    {
        details ??= new Dictionary<string, object>();
        details.Add("traceId", context.TraceIdentifier);
        GetLogger(context).Log(logLevel, exception, errorText, details);

        if (exception is DomainException ex)
        {
            await WriteDomainError(context, ex);
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)responseCode;

        var response = new ProblemDetails
        {
            Title = errorText,
            Instance = null,
            Status = (int)responseCode,
            Type = null,
            Detail = errorText
        };

        if (!_hostingEnvironment.IsProduction())
            response.Extensions.Add("trace", exception.StackTrace);

        foreach (var detail in details)
            response.Extensions.Add(detail.Key, detail.Value);

        var jsonOptions = context.RequestServices.GetRequiredService<IOptions<JsonOptions>>();
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions.Value.JsonSerializerOptions));
    }


    private Task WriteDomainError(HttpContext context, DomainException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception.StatusCode;

        var problem = exception.ToProblemDetails();

        if (!_hostingEnvironment.IsProduction())
            problem.Extensions.Add("trace", exception.StackTrace);
        
        problem.Extensions.Add("traceId", context.TraceIdentifier);

        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorText = exception.Message;
        const LogLevel logLevel = LogLevel.Error;
        const HttpStatusCode responseCode = HttpStatusCode.InternalServerError;
        await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
    }

    private async Task HandleBadHttpRequestExceptionAsync(HttpContext context, BadHttpRequestException exception)
    {
        var errorText = exception.Message;
        const LogLevel logLevel = LogLevel.Information;
        const HttpStatusCode responseCode = HttpStatusCode.BadRequest;
        await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
    }
}