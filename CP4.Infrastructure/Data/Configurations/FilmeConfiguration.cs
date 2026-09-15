using CP4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP4.Infrastructure.Data.Configurations
{
    public class FilmeConfiguration : IEntityTypeConfiguration<Filme>
    {
        public void Configure(EntityTypeBuilder<Filme> builder)
        {
            builder.ToTable("TB_CP4_FILMES");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Titulo)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(f => f.Genero)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(f => f.AnoLancamento)
                   .IsRequired();

            builder.Property(f => f.NotaImdb)
                   .HasPrecision(3, 1)
                   .IsRequired();

            builder.Property(f => f.DataCadastro)
                   .IsRequired();

            // =======================================================
            // CRITÉRIO CP4: Índices de Banco para Otimização de Busca
            // =======================================================
            // 1. Índice no Título para agilizar buscas textuais e ordenações
            builder.HasIndex(f => f.Titulo)
                   .HasDatabaseName("IX_FILME_TITULO");

            // 2. Índice no Gênero para otimizar as consultas filtradas por categoria/gênero
            builder.HasIndex(f => f.Genero)
                   .HasDatabaseName("IX_FILME_GENERO");

            // Relacionamento 1 : N
            builder.HasMany(f => f.Avaliacoes)
                   .WithOne(a => a.Filme)
                   .HasForeignKey(a => a.FilmeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
