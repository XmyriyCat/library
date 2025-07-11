using System.Net;
using System.Text.Json;
using FluentValidation;
using Library.Application.Exceptions;
using Library.Data.Exceptions;
using DirectoryNotFoundException = Library.Application.Exceptions.DirectoryNotFoundException;

namespace Library.Api.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
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
                case InvalidImageExtensionException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case ImageAlreadyExistsException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case DeleteImageException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case FileIdEmptyException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case WrongImageException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case DirectoryNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    break;
                case UserAlreadyExistsException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UserNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case BookNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UserCreationException:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case ExpiredRefreshTokenException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            
            _logger.LogError(error,
                "Unhandled exception for request {Method} {Path}: {Message} | StatusCode: {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                error.Message,
                response.StatusCode);

            var result = JsonSerializer.Serialize(new
            {
                source = error.Source,
                message = error.Message
            });
            await response.WriteAsync(result);
        }
    }
}