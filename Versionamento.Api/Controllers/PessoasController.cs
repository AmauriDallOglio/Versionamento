using Microsoft.AspNetCore.Mvc;

namespace Versionamento.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/pessoas")]
    public class PessoasController : ControllerBase
    {
        [HttpGet("uma")]
        public IActionResult GetUmaPessoa()
        {
            return Ok(new { Id = 1, Nome = "Pessoa 1" });
        }


        [HttpGet("cinco")]
        public IActionResult GetCincoPessoas()
        {
            return Ok(Enumerable.Range(1, 5).Select(i => new { Id = i, Nome = $"Pessoa {i}" }));

        }
       

        [HttpGet("dez")]
        public IActionResult GetDezPessoas()
        {
            return Ok(Enumerable.Range(1, 10).Select(i => new { Id = i, Nome = $"Pessoa {i}" }));
        }
          
    }
}
