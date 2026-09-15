namespace CP4.Domain.Entities
{
    public class Filme
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int AnoLancamento { get; set; }
        public decimal NotaImdb { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        // Relacionamento 1 : N (Um Filme tem muitas Avaliações)
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

        public Filme() { }

        public Filme(string titulo, string genero, int anoLancamento, decimal notaImdb)
        {
            Validar(titulo, genero, anoLancamento, notaImdb);
            Titulo = titulo;
            Genero = genero;
            AnoLancamento = anoLancamento;
            NotaImdb = notaImdb;
            DataCadastro = DateTime.UtcNow;
        }

        public void Atualizar(string titulo, string genero, int anoLancamento, decimal notaImdb)
        {
            Validar(titulo, genero, anoLancamento, notaImdb);
            Titulo = titulo;
            Genero = genero;
            AnoLancamento = anoLancamento;
            NotaImdb = notaImdb;
        }

        private static void Validar(string titulo, string genero, int anoLancamento, decimal notaImdb)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new Exceptions.DomainValidationException("O título do filme é obrigatório.");

            if (string.IsNullOrWhiteSpace(genero))
                throw new Exceptions.DomainValidationException("O gênero do filme é obrigatório.");

            if (anoLancamento < 1895 || anoLancamento > DateTime.UtcNow.Year + 5)
                throw new Exceptions.DomainValidationException("Ano de lançamento inválido.");

            if (notaImdb < 0 || notaImdb > 10)
                throw new Exceptions.DomainValidationException("A nota IMDb deve estar entre 0 e 10.");
        }
    }
}
