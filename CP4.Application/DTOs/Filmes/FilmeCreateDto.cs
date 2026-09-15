using System.ComponentModel.DataAnnotations;

namespace CP4.Application.DTOs.Filmes
{
    public class FilmeCreateDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O título deve ter entre 1 e 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O gênero é obrigatório.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O gênero deve ter entre 2 e 50 caracteres.")]
        public string Genero { get; set; } = string.Empty;

        [Range(1895, 2030, ErrorMessage = "Ano de lançamento deve ser entre 1895 e 2030.")]
        public int AnoLancamento { get; set; }

        [Range(0, 10, ErrorMessage = "A nota IMDb deve estar entre 0.0 e 10.0.")]
        public decimal NotaImdb { get; set; }
    }
}
