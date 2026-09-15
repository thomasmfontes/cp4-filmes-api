using CP4.Domain.Entities;

namespace CP4.Domain.Interfaces
{
    public interface IAvaliacaoRepository
    {
        Task<Avaliacao?> ObterPorIdAsync(int id);
        Task<IEnumerable<Avaliacao>> ObterPorFilmeIdAsync(int filmeId);
        Task<Avaliacao> AdicionarAsync(Avaliacao avaliacao);
        Task RemoverAsync(Avaliacao avaliacao);
    }
}
