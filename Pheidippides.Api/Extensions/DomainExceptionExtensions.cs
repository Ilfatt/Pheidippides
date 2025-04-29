using Microsoft.AspNetCore.Mvc;
using Pheidippides.Domain.Exceptions;

namespace Pheidippides.Api.Extensions;

public static class DomainExceptionExtensions
{
    public static ProblemDetails ToProblemDetails(this DomainException ex)
    {
        var problems = new ProblemDetails
        {
            Title = ex.Message,
            Status = ex.StatusCode,
            Extensions =
            {
                ["placeholderData"] = ex.PlaceholderData
            }
        };
        return problems;
    }
}