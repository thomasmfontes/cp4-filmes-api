using CP4.Application.DTOs.Common;
using CP4.Application.DTOs.Filmes;
using CP4.Application.Services;
using CP4.Domain.Entities;
using CP4.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CP4.UnitTests.Services
{
    public class FilmeServiceTests
    {
        private readonly Mock<IFilmeRepository> _filmeRepoMock;
        private readonly Mock<ILogger<FilmeService>> _loggerMock;
        private readonly FilmeService _filmeService;

        public FilmeServiceTests()
        {
            _filmeRepoMock = new Mock<IFilmeRepository>();
            _loggerMock = new Mock<ILogger<FilmeService>>();
            _filmeService = new FilmeService(_filmeRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ObterPaginadoAsync_DeveRetornarEnvelopeComMetadadosDePaginacao()
        {
            // Arrange
            var filmesFake = new List<Filme>
            {
                new Filme("Inception", "Ficção", 2010, 8.8m) { Id = 1 },
                new Filme("Tenet", "Ficção", 2020, 7.3m) { Id = 2 }
            };

            _filmeRepoMock
                .Setup(r => r.ObterPaginadoAsync(1, 10, null))
                .ReturnsAsync((filmesFake, 2));

            var request = new PagedRequest { PageNumber = 1, PageSize = 10 };

            // Act
            var resultado = await _filmeService.ObterPaginadoAsync(request, null);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.TotalCount);
            Assert.Equal(1, resultado.PageNumber);
            Assert.Equal(10, resultado.PageSize);
            Assert.Equal(2, resultado.Items.Count());
            Assert.False(resultado.HasPreviousPage);
            Assert.False(resultado.HasNextPage);
        }

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveSalvarERetornarDto()
        {
            // Arrange
            var dto = new FilmeCreateDto
            {
                Titulo = "Duna 2",
                Genero = "Ficção",
                AnoLancamento = 2024,
                NotaImdb = 8.6m
            };

            _filmeRepoMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Filme>()))
                .ReturnsAsync((Filme f) => { f.Id = 10; return f; });

            // Act
            var resultado = await _filmeService.CriarAsync(dto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            Assert.Equal("Duna 2", resultado.Titulo);
            _filmeRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Filme>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_QuandoFilmeNaoExiste_DeveRetornarFalso()
        {
            // Arrange
            _filmeRepoMock
                .Setup(r => r.ObterPorIdAsync(99))
                .ReturnsAsync((Filme?)null);

            var updateDto = new FilmeUpdateDto
            {
                Titulo = "Novo Nome",
                Genero = "Ação",
                AnoLancamento = 2022,
                NotaImdb = 7.0m
            };

            // Act
            var resultado = await _filmeService.AtualizarAsync(99, updateDto);

            // Assert
            Assert.False(resultado);
            _filmeRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Filme>()), Times.Never);
        }
    }
}
