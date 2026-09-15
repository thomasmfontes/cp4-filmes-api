using CP4.Application.DTOs.Common;
using CP4.Application.DTOs.Filmes;
using CP4.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace CP4.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableRateLimiting("fixed")] // Aplica a política de Rate Limiting
    public class FilmesController : ControllerBase
    {
        private readonly IFilmeService _filmeService;
        private readonly ILogger<FilmesController> _logger;

        public FilmesController(IFilmeService filmeService, ILogger<FilmesController> logger)
        {
            _filmeService = filmeService;
            _logger = logger;
        }

        /// <summary>
        /// Lista filmes cadastrados com suporte a paginação e filtro por gênero.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista filmes paginados",
            Description = "Retorna uma lista paginada de filmes ordenada por título, com filtro opcional por gênero.")]
        [ProducesResponseType(typeof(PagedResult<FilmeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] PagedRequest request, 
            [FromQuery] string? genero = null)
        {
            _logger.LogInformation("Recebida solicitacao para listar filmes paginados");
            var resultado = await _filmeService.ObterPaginadoAsync(request, genero);
            return Ok(resultado);
        }

        /// <summary>
        /// Busca os detalhes de um filme e suas avaliações pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Obtém filme por ID",
            Description = "Retorna os detalhes do filme informado, incluindo a lista completa de avaliações.")]
        [ProducesResponseType(typeof(FilmeDetalhesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            _logger.LogInformation("Recebida solicitacao para buscar filme ID: {Id}", id);
            var filme = await _filmeService.ObterPorIdAsync(id);

            if (filme == null)
                return NotFound(new { Message = $"Filme com ID {id} não encontrado." });

            return Ok(filme);
        }

        /// <summary>
        /// Cadastra um novo filme no catálogo.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra novo filme",
            Description = "Insere um novo filme no catálogo validando os campos obrigatórios e intervalos.")]
        [ProducesResponseType(typeof(FilmeResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Criar([FromBody] FilmeCreateDto dto)
        {
            _logger.LogInformation("Recebida solicitacao de cadastro de filme: {Titulo}", dto.Titulo);
            var criado = await _filmeService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
        }

        /// <summary>
        /// Atualiza os dados de um filme existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Atualiza filme existente",
            Description = "Atualiza título, gênero, ano de lançamento e nota IMDb do filme informado.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] FilmeUpdateDto dto)
        {
            _logger.LogInformation("Recebida solicitacao para atualizar filme ID: {Id}", id);
            var atualizado = await _filmeService.AtualizarAsync(id, dto);

            if (!atualizado)
                return NotFound(new { Message = $"Filme com ID {id} não encontrado para atualização." });

            return NoContent();
        }

        /// <summary>
        /// Remove um filme do catálogo pelo ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Exclui um filme",
            Description = "Remove permanentemente um filme e todas as avaliações vinculadas.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Remover(int id)
        {
            _logger.LogInformation("Recebida solicitacao para remover filme ID: {Id}", id);
            var removido = await _filmeService.RemoverAsync(id);

            if (!removido)
                return NotFound(new { Message = $"Filme com ID {id} não encontrado para remoção." });

            return NoContent();
        }
    }
}
