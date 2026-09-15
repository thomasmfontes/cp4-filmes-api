namespace CP4.Domain.Entities
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public int Nota { get; set; }
        public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;

        // Chave estrangeira para Filme
        public int FilmeId { get; set; }
        public Filme? Filme { get; set; }

        public Avaliacao() { }

        public Avaliacao(string usuario, string comentario, int nota, int filmeId)
        {
            Validar(usuario, comentario, nota);
            Usuario = usuario;
            Comentario = comentario;
            Nota = nota;
            FilmeId = filmeId;
            DataAvaliacao = DateTime.UtcNow;
        }

        private static void Validar(string usuario, string comentario, int nota)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new Exceptions.DomainValidationException("O nome do usuário é obrigatório.");

            if (string.IsNullOrWhiteSpace(comentario))
                throw new Exceptions.DomainValidationException("O comentário é obrigatório.");

            if (nota < 1 || nota > 5)
                throw new Exceptions.DomainValidationException("A nota deve ser um valor inteiro entre 1 e 5.");
        }
    }
}
