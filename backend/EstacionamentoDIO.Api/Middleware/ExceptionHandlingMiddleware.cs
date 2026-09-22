using System.Net;
using System.Text.Json;
using EstacionamentoDIO.Domain.Exceptions;

namespace EstacionamentoDIO.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate proximo;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate proximo, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.proximo = proximo;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await proximo(context);
        }
        catch (Exception excecao)
        {
            var statusCode = MapearStatusCode(excecao);

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                logger.LogError(excecao, "Erro não tratado ao processar {Path}", context.Request.Path);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var corpo = JsonSerializer.Serialize(new { message = MensagemParaCliente(excecao, statusCode) });
            await context.Response.WriteAsync(corpo);
        }
    }

    private static HttpStatusCode MapearStatusCode(Exception excecao) => excecao switch
    {
        VeiculoNaoEncontradoException => HttpStatusCode.NotFound,
        VeiculoJaEstacionadoException => HttpStatusCode.Conflict,
        EstacionamentoLotadoException => HttpStatusCode.UnprocessableEntity,
        PlacaInvalidaException => HttpStatusCode.UnprocessableEntity,
        _ => HttpStatusCode.InternalServerError,
    };

    private static string MensagemParaCliente(Exception excecao, HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.InternalServerError
            ? "Ocorreu um erro inesperado. Tente novamente em instantes."
            : excecao.Message;
}
