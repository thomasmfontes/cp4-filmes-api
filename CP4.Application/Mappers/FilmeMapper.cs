using CP4.Application.DTOs.Avaliacoes;
using CP4.Application.DTOs.Filmes;
using CP4.Domain.Entities;

namespace CP4.Application.Mappers
{
    public static class FilmeMapper
    {
        public static Filme ToEntity(this FilmeCreateDto dto)
        {
            return new Filme(dto.Titulo, dto.Genero, dto.AnoLancamento, dto.NotaImdb);
        }

        public static FilmeResponseDto ToDto(this Filme filme)
        {
            return new FilmeResponseDto
            {
                Id = filme.Id,
                Titulo = filme.Titulo,
                Genero = filme.Genero,
                AnoLancamento = filme.AnoLancamento,
                NotaImdb = filme.NotaImdb,
                DataCadastro = filme.DataCadastro
            };
        }

        public static FilmeDetalhesResponseDto ToDetalhesDto(this Filme filme)
        {
            return new FilmeDetalhesResponseDto
            {
                Id = filme.Id,
                Titulo = filme.Titulo,
                Genero = filme.Genero,
                AnoLancamento = filme.AnoLancamento,
                NotaImdb = filme.NotaImdb,
                DataCadastro = filme.DataCadastro,
                Avaliacoes = filme.Avaliacoes?.Select(a => a.ToDto()) ?? Enumerable.Empty<AvaliacaoResponseDto>()
            };
        }
    }
}
