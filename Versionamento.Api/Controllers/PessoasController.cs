using Microsoft.AspNetCore.Mvc;
using Versionamento.Servico.DTO;

namespace Versionamento.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/pessoas")]
    public class PessoasController : ControllerBase
    {
        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var usuarios = Usuario.Lista();
            return Ok(usuarios);
        }

 
          
    }
}
