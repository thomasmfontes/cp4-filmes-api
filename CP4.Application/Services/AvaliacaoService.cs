using CP4.Application.DTOs.Avaliacoes;
using CP4.Application.Interfaces;
using CP4.Application.Mappers;
using CP4.Domain.Exceptions;
using CP4.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CP4.Application.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly IFilmeRepository _filmeRepository;
        private readonly ILogger<AvaliacaoService> _logger;

        public AvaliacaoService(
            IAvaliacaoRepository avaliacaoRepository,
            IFilmeRepository filmeRepository,
            ILogger<AvaliacaoService> logger)
        {
            _avaliacaoRepository = avaliacaoRepository;
            _filmeRepository = filmeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AvaliacaoResponseDto>> ObterPorFilmeIdAsync(int filmeId)
        {
            _logger.LogInformation("Listando avaliacoes do filme ID: {FilmeId}", filmeId);

            var avaliacoes = await _avaliacaoRepository.ObterPorFilmeIdAsync(filmeId);
            return avaliacoes.Select(a => a.ToDto());
        }

        public async Task<AvaliacaoResponseDto> CriarAsync(AvaliacaoCreateDto dto)
        {
            _logger.LogInformation("Adicionando avaliacao para o filme ID: {FilmeId} pelo usuario: {Usuario}", dto.FilmeId, dto.Usuario);

            var filmeExiste = await _filmeRepository.ExisteAsync(dto.FilmeId);
            if (!filmeExiste)
                throw new DomainValidationException($"O filme com ID {dto.FilmeId} não existe.");

            var avaliacao = dto.ToEntity();
            var criada = await _avaliacaoRepository.AdicionarAsync(avaliacao);

            return criada.ToDto();
        }

        public async Task<bool> RemoverAsync(int id)
        {
            _logger.LogInformation("Removendo avaliacao ID: {AvaliacaoId}", id);

            var avaliacao = await _avaliacaoRepository.ObterPorIdAsync(id);
            if (avaliacao == null)
            {
                _logger.LogWarning("Avaliacao ID {AvaliacaoId} nao encontrada para remocao", id);
                return false;
            }

            await _avaliacaoRepository.RemoverAsync(avaliacao);
            return true;
        }
    }
}
