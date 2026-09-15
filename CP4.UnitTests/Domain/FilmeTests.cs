using CP4.Domain.Entities;
using CP4.Domain.Exceptions;

namespace CP4.UnitTests.Domain
{
    public class FilmeTests
    {
        [Fact]
        public void CriarFilme_ComDadosValidos_DeveInstanciarComSucesso()
        {
            // Arrange & Act
            var filme = new Filme("Interestelar", "Ficção Científica", 2014, 8.7m);

            // Assert
            Assert.NotNull(filme);
            Assert.Equal("Interestelar", filme.Titulo);
            Assert.Equal("Ficção Científica", filme.Genero);
            Assert.Equal(2014, filme.AnoLancamento);
            Assert.Equal(8.7m, filme.NotaImdb);
        }

        [Theory]
        [InlineData("", "Drama", 2020, 7.5)]
        [InlineData("   ", "Ação", 2021, 8.0)]
        public void CriarFilme_ComTituloVazio_DeveLancarDomainValidationException(
            string titulo, string genero, int ano, decimal nota)
        {
            // Act & Assert
            var ex = Assert.Throws<DomainValidationException>(() =>
                new Filme(titulo, genero, ano, nota));

            Assert.Contains("título do filme é obrigatório", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("Matrix", "Ficção", 1800, 8.5)] // Ano antes do cinema (1895)
        [InlineData("Avatar", "Ficção", 2099, 7.9)] // Ano muito no futuro
        public void CriarFilme_ComAnoInvalido_DeveLancarDomainValidationException(
            string titulo, string genero, int ano, decimal nota)
        {
            // Act & Assert
            Assert.Throws<DomainValidationException>(() =>
                new Filme(titulo, genero, ano, nota));
        }

        [Theory]
        [InlineData("O Poderoso Chefão", "Crime", 1972, -1.0)]
        [InlineData("O Poderoso Chefão", "Crime", 1972, 10.5)]
        public void CriarFilme_ComNotaImdbForaDoIntervalo_DeveLancarDomainValidationException(
            string titulo, string genero, int ano, decimal nota)
        {
            // Act & Assert
            Assert.Throws<DomainValidationException>(() =>
                new Filme(titulo, genero, ano, nota));
        }
    }
}
