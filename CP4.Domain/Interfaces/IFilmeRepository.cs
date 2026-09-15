using CP4.Domain.Entities;

namespace CP4.Domain.Interfaces
{
    public interface IFilmeRepository
    {
        Task<Filme?> ObterPorIdAsync(int id);
        Task<Filme?> ObterPorIdComAvaliacoesAsync(int id);
        Task<(IEnumerable<Filme> Items, int TotalCount)> ObterPaginadoAsync(int pageNumber, int pageSize, string? genero = null);
        Task<Filme> AdicionarAsync(Filme filme);
        Task AtualizarAsync(Filme filme);
        Task RemoverAsync(Filme filme);
        Task<bool> ExisteAsync(int id);
    }
}
