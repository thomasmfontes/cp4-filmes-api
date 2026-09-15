using System.ComponentModel.DataAnnotations;

namespace CP4.Application.DTOs.Filmes
{
    public class FilmeUpdateDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, MinimumLength = 1)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O gênero é obrigatório.")]
        [StringLength(50, MinimumLength = 2)]
        public string Genero { get; set; } = string.Empty;

        [Range(1895, 2030)]
        public int AnoLancamento { get; set; }

        [Range(0, 10)]
        public decimal NotaImdb { get; set; }
    }
}
