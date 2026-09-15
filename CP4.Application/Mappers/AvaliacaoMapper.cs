using CP4.Application.DTOs.Avaliacoes;
using CP4.Domain.Entities;

namespace CP4.Application.Mappers
{
    public static class AvaliacaoMapper
    {
        public static Avaliacao ToEntity(this AvaliacaoCreateDto dto)
        {
            return new Avaliacao(dto.Usuario, dto.Comentario, dto.Nota, dto.FilmeId);
        }

        public static AvaliacaoResponseDto ToDto(this Avaliacao avaliacao)
        {
            return new AvaliacaoResponseDto
            {
                Id = avaliacao.Id,
                Usuario = avaliacao.Usuario,
                Comentario = avaliacao.Comentario,
                Nota = avaliacao.Nota,
                FilmeId = avaliacao.FilmeId,
                DataAvaliacao = avaliacao.DataAvaliacao
            };
        }
    }
}
