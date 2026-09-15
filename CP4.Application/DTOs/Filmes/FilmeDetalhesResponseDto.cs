using CP4.Application.DTOs.Avaliacoes;

namespace CP4.Application.DTOs.Filmes
{
    public class FilmeDetalhesResponseDto : FilmeResponseDto
    {
        public IEnumerable<AvaliacaoResponseDto> Avaliacoes { get; set; } = new List<AvaliacaoResponseDto>();
    }
}
