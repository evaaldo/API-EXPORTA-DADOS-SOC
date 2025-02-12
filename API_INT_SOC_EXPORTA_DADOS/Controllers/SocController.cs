using API_INT_SOC_EXPORTA_DADOS.Models;
using API_INT_SOC_EXPORTA_DADOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_INT_SOC_EXPORTA_DADOS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SocController : ControllerBase
    {
        private readonly ILogger<SocController> _logger;
        private readonly SocService _socService;

        public SocController(ILogger<SocController> logger, SocService socService)
        {
            _logger = logger;
            _socService = socService;
        }

        [HttpPost("buscarAsoPorPeriodo")]
        public async Task<IActionResult> BuscarAsoPorPeriodo([FromBody] AsoPeriodo parametros)
        {
            try
            {
                var resultado = await _socService.BuscarAsoPorPeriodo(parametros);

                if (resultado == "erro")
                {
                    return BadRequest("Erro na passagem de parâmetros");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao realizar requisição SOAP. Erro: " + ex.Message);
                return BadRequest("Erro ao realizar requisição SOAP. Erro: " + ex.Message);
            }
        }
    }
}
