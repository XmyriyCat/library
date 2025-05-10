using System.Net;
using System.Text.Json;
using FluentValidation;
using Library.Application.Exceptions;
using Library.Data.Exceptions;

namespace Library.Api.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case AuthorIdNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case DbPageException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new
                {
                    source = error.Source,
                    message = error.Message
                });
            await response.WriteAsync(result);
        }
    }
}