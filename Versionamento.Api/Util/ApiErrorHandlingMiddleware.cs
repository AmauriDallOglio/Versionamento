using System.Diagnostics;
using System.Text.Json;

namespace Versionamento.Api.Util
{
    public class ApiErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiErrorHandlingMiddleware> _logger;

        public ApiErrorHandlingMiddleware(RequestDelegate next, ILogger<ApiErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Erro inesperado na requisição.");

                await HandleExceptionAsync(context, ex, stopwatch.ElapsedMilliseconds);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, long tempoMs)
        {
            var versao = DetectarVersao(context.Request.Path);
            var response = CriarRespostaErro(versao, context.Response.StatusCode == 0 ? 500 : context.Response.StatusCode, ex.Message, tempoMs);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }


        private static string DetectarVersao(PathString path)
        {
            if (path.HasValue)
            {
                if (path.Value.Contains("/v1/", StringComparison.OrdinalIgnoreCase)) return "v1";
                if (path.Value.Contains("/v2/", StringComparison.OrdinalIgnoreCase)) return "v2";
                if (path.Value.Contains("/v3/", StringComparison.OrdinalIgnoreCase)) return "v3";
            }
            return "v1"; // padrão
        }

        private static object CriarRespostaErro(string versao, int statusCode, string mensagem, long tempoMs)
        {
            return versao switch
            {
                "v2" => new
                {
                    sucesso = false,
                    mensagem,
                    codigo = statusCode,
                    tempo = $"{tempoMs}ms"
                },
                "v3" => new
                {
                    sucesso = false,
                    mensagem,
                    codigo = statusCode,
                    tempo = $"{tempoMs}ms",
                    detalhes = "Verifique os parâmetros enviados ou contate o suporte técnico."
                },
                _ => new
                {
                    sucesso = false,
                    mensagem
                }
            };
        }
    }
}