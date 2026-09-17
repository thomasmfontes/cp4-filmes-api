using CP4.Application.DTOs.Avaliacoes;

namespace CP4.Application.Interfaces
{
    public interface IAvaliacaoService
    {
        Task<IEnumerable<AvaliacaoResponseDto>?> ObterPorFilmeIdAsync(int filmeId);
        Task<AvaliacaoResponseDto> CriarAsync(AvaliacaoCreateDto dto);
        Task<bool> RemoverAsync(int id);
    }
}
