using CP4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CP4.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Filme> Filmes => Set<Filme>();
        public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as configurações de entidades (FilmeConfiguration e AvaliacaoConfiguration)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
