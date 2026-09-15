using CP4.Application.DTOs.Avaliacoes;
using CP4.Application.Services;
using CP4.Domain.Exceptions;
using CP4.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CP4.UnitTests.Services
{
    public class AvaliacaoServiceTests
    {
        private readonly Mock<IAvaliacaoRepository> _avaliacaoRepoMock;
        private readonly Mock<IFilmeRepository> _filmeRepoMock;
        private readonly Mock<ILogger<AvaliacaoService>> _loggerMock;
        private readonly AvaliacaoService _service;

        public AvaliacaoServiceTests()
        {
            _avaliacaoRepoMock = new Mock<IAvaliacaoRepository>();
            _filmeRepoMock = new Mock<IFilmeRepository>();
            _loggerMock = new Mock<ILogger<AvaliacaoService>>();
            _service = new AvaliacaoService(_avaliacaoRepoMock.Object, _filmeRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CriarAsync_QuandoFilmeNaoExiste_DeveLancarDomainValidationException()
        {
            // Arrange
            _filmeRepoMock
                .Setup(r => r.ExisteAsync(999))
                .ReturnsAsync(false);

            var dto = new AvaliacaoCreateDto
            {
                FilmeId = 999,
                Usuario = "Thomas",
                Comentario = "Excelente filme!",
                Nota = 5
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<DomainValidationException>(() => _service.CriarAsync(dto));
            Assert.Contains("não existe", ex.Message);
        }
    }
}
