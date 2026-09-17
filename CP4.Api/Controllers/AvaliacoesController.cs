using CP4.Application.DTOs.Avaliacoes;
using CP4.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace CP4.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableRateLimiting("fixed")]
    public class AvaliacoesController : ControllerBase
    {
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly ILogger<AvaliacoesController> _logger;

        public AvaliacoesController(IAvaliacaoService avaliacaoService, ILogger<AvaliacoesController> logger)
        {
            _avaliacaoService = avaliacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todas as avaliações cadastradas para determinado filme.
        /// </summary>
        [HttpGet("filme/{filmeId:int}")]
        [SwaggerOperation(
            Summary = "Lista avaliações por filme",
            Description = "Retorna todas as notas e comentários dos usuários para um filme específico.")]
        [ProducesResponseType(typeof(IEnumerable<AvaliacaoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ObterPorFilmeId(int filmeId)
        {
            _logger.LogInformation("Listando avaliacoes para filme ID: {FilmeId}", filmeId);
            var resultado = await _avaliacaoService.ObterPorFilmeIdAsync(filmeId);

            if (resultado == null)
                return NotFound(new { Message = $"Filme com ID {filmeId} não encontrado." });

            if (!resultado.Any())
                return NotFound(new { Message = $"Nenhuma avaliação encontrada para o filme com ID {filmeId}." });

            return Ok(resultado);
        }

        /// <summary>
        /// Registra uma nova avaliação para um filme.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra nova avaliação",
            Description = "Insere uma avaliação com nota de 1 a 5 e comentário para o filme informado.")]
        [ProducesResponseType(typeof(AvaliacaoResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Criar([FromBody] AvaliacaoCreateDto dto)
        {
            _logger.LogInformation("Cadastrando avaliacao para filme ID: {FilmeId}", dto.FilmeId);
            var criada = await _avaliacaoService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorFilmeId), new { filmeId = criada.FilmeId }, criada);
        }

        /// <summary>
        /// Remove uma avaliação pelo ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Exclui avaliação",
            Description = "Remove uma avaliação existente.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Remover(int id)
        {
            _logger.LogInformation("Removendo avaliacao ID: {Id}", id);
            var removido = await _avaliacaoService.RemoverAsync(id);

            if (!removido)
                return NotFound(new { Message = $"Avaliação com ID {id} não encontrada." });

            return NoContent();
        }
    }
}
