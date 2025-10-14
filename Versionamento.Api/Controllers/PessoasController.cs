using Microsoft.AspNetCore.Mvc;
using Versionamento.Servico.DTO;

namespace Versionamento.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PessoasController : ControllerBase
    {
        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var usuarios = Usuario.Lista();
            return Ok(usuarios);
        }


        [HttpGet("PingRespostaV1")]
        public IActionResult PingRespostaV1()
        {
            return Ok(new { Sucesso = true, Mensagem = "Resposta v1" });
        }
    }
}
