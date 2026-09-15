using CP4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP4.Infrastructure.Data.Configurations
{
    public class AvaliacaoConfiguration : IEntityTypeConfiguration<Avaliacao>
    {
        public void Configure(EntityTypeBuilder<Avaliacao> builder)
        {
            builder.ToTable("TB_CP4_AVALIACOES");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Usuario)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.Property(a => a.Comentario)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(a => a.Nota)
                   .IsRequired();

            builder.Property(a => a.DataAvaliacao)
                   .IsRequired();

            // =======================================================
            // CRITÉRIO CP4: Índices de Banco para Otimização de Busca
            // =======================================================
            // 1. Índice na FK FilmeId para agilizar JOINs e listagens de avaliações por filme
            builder.HasIndex(a => a.FilmeId)
                   .HasDatabaseName("IX_AVALIACAO_FILMEID");

            // 2. Índice no Usuario para pesquisas de histórico por usuário
            builder.HasIndex(a => a.Usuario)
                   .HasDatabaseName("IX_AVALIACAO_USUARIO");
        }
    }
}
