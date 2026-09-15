using CP4.Application.DTOs.Common;
using CP4.Application.DTOs.Filmes;
using CP4.Application.Interfaces;
using CP4.Application.Mappers;
using CP4.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CP4.Application.Services
{
    public class FilmeService : IFilmeService
    {
        private readonly IFilmeRepository _filmeRepository;
        private readonly ILogger<FilmeService> _logger;

        public FilmeService(IFilmeRepository filmeRepository, ILogger<FilmeService> logger)
        {
            _filmeRepository = filmeRepository;
            _logger = logger;
        }

        public async Task<PagedResult<FilmeResponseDto>> ObterPaginadoAsync(PagedRequest request, string? genero = null)
        {
            _logger.LogInformation("Buscando filmes paginados. Pagina: {PageNumber}, Tamanho: {PageSize}, Genero: {Genero}",
                request.PageNumber, request.PageSize, genero ?? "Todos");

            var (items, totalCount) = await _filmeRepository.ObterPaginadoAsync(request.PageNumber, request.PageSize, genero);

            var dtos = items.Select(f => f.ToDto());

            return new PagedResult<FilmeResponseDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<FilmeDetalhesResponseDto?> ObterPorIdAsync(int id)
        {
            _logger.LogInformation("Buscando filme com avaliacoes por ID: {FilmeId}", id);

            var filme = await _filmeRepository.ObterPorIdComAvaliacoesAsync(id);
            return filme?.ToDetalhesDto();
        }

        public async Task<FilmeResponseDto> CriarAsync(FilmeCreateDto dto)
        {
            _logger.LogInformation("Cadastrando novo filme: {Titulo}", dto.Titulo);

            var filme = dto.ToEntity();
            var criado = await _filmeRepository.AdicionarAsync(filme);

            return criado.ToDto();
        }

        public async Task<bool> AtualizarAsync(int id, FilmeUpdateDto dto)
        {
            _logger.LogInformation("Atualizando filme ID: {FilmeId}", id);

            var filme = await _filmeRepository.ObterPorIdAsync(id);
            if (filme == null)
            {
                _logger.LogWarning("Filme ID {FilmeId} nao encontrado para atualizacao", id);
                return false;
            }

            filme.Atualizar(dto.Titulo, dto.Genero, dto.AnoLancamento, dto.NotaImdb);
            await _filmeRepository.AtualizarAsync(filme);

            return true;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            _logger.LogInformation("Removendo filme ID: {FilmeId}", id);

            var filme = await _filmeRepository.ObterPorIdAsync(id);
            if (filme == null)
            {
                _logger.LogWarning("Filme ID {FilmeId} nao encontrado para remocao", id);
                return false;
            }

            await _filmeRepository.RemoverAsync(filme);
            return true;
        }
    }
}
