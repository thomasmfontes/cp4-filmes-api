namespace CP4.Application.DTOs.Avaliacoes
{
    public class AvaliacaoResponseDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public int Nota { get; set; }
        public int FilmeId { get; set; }
        public DateTime DataAvaliacao { get; set; }
    }
}
