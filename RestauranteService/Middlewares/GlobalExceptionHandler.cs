using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestauranteService.Exceptions;

namespace RestauranteService.Middlewares;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exceção capturada no pipeline HTTP: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            NotFoundException nf => (StatusCodes.Status404NotFound, "Recurso Não Encontrado", nf.Message),
            ConflictException cf => (StatusCodes.Status409Conflict, "Conflito de Estado", cf.Message),
            ValidationException val => (StatusCodes.Status400BadRequest, "Erro de Validação", val.Message),
            ArgumentException arg => (StatusCodes.Status400BadRequest, "Argumento Inválido", arg.Message),
            _ => (StatusCodes.Status500InternalServerError, "Erro Interno no Servidor",
                  environment.IsDevelopment() ? exception.ToString() : "Ocorreu um erro interno inesperado.")
        };

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            }
        });
    }
}
