namespace CP4.Application.DTOs.Filmes
{
    public class FilmeResponseDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int AnoLancamento { get; set; }
        public decimal NotaImdb { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
