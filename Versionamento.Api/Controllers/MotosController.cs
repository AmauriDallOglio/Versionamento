using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Versionamento.Api.Controllers
{

    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/motos")]
    [Authorize(Policy = "AcessoV3")] // Apenas quem tem claim AcessoApi=v3
    public class MotosController : ControllerBase
    {
        [HttpGet("lista")]
        public IActionResult GetMotos() =>
            Ok(new[] { new { Id = 1, Modelo = "Hornet 600" } });
    }

}
