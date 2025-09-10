using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Versionamento.Api.Controllers
{

    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/carros")]
    [Authorize(Policy = "AcessoV2")] // Apenas quem tem claim AcessoApi=v2
    public class CarrosController : ControllerBase
    {
        [HttpGet("lista")]
        public IActionResult GetCarros() =>
            Ok(new[]
            {
                new { Id = 1, Modelo = "Fusca" },
                new { Id = 2, Modelo = "Opala" }
            });
    }
}
