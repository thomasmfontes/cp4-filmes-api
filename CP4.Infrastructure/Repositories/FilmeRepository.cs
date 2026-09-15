using CP4.Domain.Entities;
using CP4.Domain.Interfaces;
using CP4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP4.Infrastructure.Repositories
{
    public class FilmeRepository : IFilmeRepository
    {
        private readonly AppDbContext _context;

        public FilmeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Filme?> ObterPorIdAsync(int id)
        {
            return await _context.Filmes.FindAsync(id);
        }

        public async Task<Filme?> ObterPorIdComAvaliacoesAsync(int id)
        {
            return await _context.Filmes
                .Include(f => f.Avaliacoes)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        // =======================================================
        // CRITÉRIO CP4: Paginação de Resultados no Banco de Dados
        // =======================================================
        public async Task<(IEnumerable<Filme> Items, int TotalCount)> ObterPaginadoAsync(
            int pageNumber, 
            int pageSize, 
            string? genero = null)
        {
            var query = _context.Filmes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(genero))
            {
                var generoLower = genero.Trim().ToLower();
                query = query.Where(f => f.Genero.ToLower() == generoLower);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(f => f.Titulo)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Filme> AdicionarAsync(Filme filme)
        {
            await _context.Filmes.AddAsync(filme);
            await _context.SaveChangesAsync();
            return filme;
        }

        public async Task AtualizarAsync(Filme filme)
        {
            _context.Filmes.Update(filme);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Filme filme)
        {
            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Filmes.AnyAsync(f => f.Id == id);
        }
    }
}
