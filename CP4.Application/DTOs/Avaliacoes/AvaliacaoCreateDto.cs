using System.ComponentModel.DataAnnotations;

namespace CP4.Application.DTOs.Avaliacoes
{
    public class AvaliacaoCreateDto
    {
        [Required(ErrorMessage = "O usuário é obrigatório.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "O usuário deve ter entre 2 e 80 caracteres.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O comentário é obrigatório.")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "O comentário deve ter entre 3 e 500 caracteres.")]
        public string Comentario { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "A nota deve ser um número inteiro de 1 a 5 estrelas.")]
        public int Nota { get; set; }

        [Required(ErrorMessage = "O ID do filme é obrigatório.")]
        public int FilmeId { get; set; }
    }
}
