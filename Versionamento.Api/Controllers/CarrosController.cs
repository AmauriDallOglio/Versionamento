using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Versionamento.Api.Controllers
{

    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "AcessoV2")] // Apenas quem tem claim AcessoApi=v2
    public class CarrosController : ControllerBase
    {
        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            return Ok(new[]
            {
                new { Id = 1, Modelo = "Fusca" },
                new { Id = 2, Modelo = "Opala" }
            });
        }

        [HttpGet("PingRespostaV2")]
        public IActionResult PingRespostaV2()
        {

            return Ok(new { Sucesso = true, Mensagem = "Resposta v2", Codigo = 200, Tempo = DateTime.UtcNow });

        }
    }
}
