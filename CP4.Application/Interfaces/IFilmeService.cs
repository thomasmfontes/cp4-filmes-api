using CP4.Application.DTOs.Common;
using CP4.Application.DTOs.Filmes;

namespace CP4.Application.Interfaces
{
    public interface IFilmeService
    {
        Task<PagedResult<FilmeResponseDto>> ObterPaginadoAsync(PagedRequest request, string? genero = null);
        Task<FilmeDetalhesResponseDto?> ObterPorIdAsync(int id);
        Task<FilmeResponseDto> CriarAsync(FilmeCreateDto dto);
        Task<bool> AtualizarAsync(int id, FilmeUpdateDto dto);
        Task<bool> RemoverAsync(int id);
    }
}
