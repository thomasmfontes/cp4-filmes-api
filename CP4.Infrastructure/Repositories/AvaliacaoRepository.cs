using CP4.Domain.Entities;
using CP4.Domain.Interfaces;
using CP4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP4.Infrastructure.Repositories
{
    public class AvaliacaoRepository : IAvaliacaoRepository
    {
        private readonly AppDbContext _context;

        public AvaliacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Avaliacao?> ObterPorIdAsync(int id)
        {
            return await _context.Avaliacoes.FindAsync(id);
        }

        public async Task<IEnumerable<Avaliacao>> ObterPorFilmeIdAsync(int filmeId)
        {
            return await _context.Avaliacoes
                .AsNoTracking()
                .Where(a => a.FilmeId == filmeId)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();
        }

        public async Task<Avaliacao> AdicionarAsync(Avaliacao avaliacao)
        {
            await _context.Avaliacoes.AddAsync(avaliacao);
            await _context.SaveChangesAsync();
            return avaliacao;
        }

        public async Task RemoverAsync(Avaliacao avaliacao)
        {
            _context.Avaliacoes.Remove(avaliacao);
            await _context.SaveChangesAsync();
        }
    }
}
