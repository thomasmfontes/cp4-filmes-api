using CP4.Application.DTOs.Common;
using CP4.Application.DTOs.Filmes;
using System.Net;
using System.Net.Http.Json;

namespace CP4.IntegrationTests
{
    public class FilmesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public FilmesEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CicloCompletoFilmes_CriarListarEObterPorId()
        {
            // 1. POST - Cadastrar Filme
            var novoFilme = new FilmeCreateDto
            {
                Titulo = "Gladiador II",
                Genero = "Ação",
                AnoLancamento = 2024,
                NotaImdb = 8.0m
            };

            var postResponse = await _client.PostAsJsonAsync("/api/filmes", novoFilme);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var filmeCriado = await postResponse.Content.ReadFromJsonAsync<FilmeResponseDto>();
            Assert.NotNull(filmeCriado);
            Assert.True(filmeCriado.Id > 0);
            Assert.Equal(novoFilme.Titulo, filmeCriado.Titulo);

            // 2. GET - Listar Paginado
            var getListResponse = await _client.GetAsync("/api/filmes?pageNumber=1&pageSize=10");
            Assert.Equal(HttpStatusCode.OK, getListResponse.StatusCode);

            var resultadoPaginado = await getListResponse.Content.ReadFromJsonAsync<PagedResult<FilmeResponseDto>>();
            Assert.NotNull(resultadoPaginado);
            Assert.True(resultadoPaginado.TotalCount >= 1);
            Assert.Contains(resultadoPaginado.Items, f => f.Id == filmeCriado.Id);

            // 3. GET por ID
            var getByIdResponse = await _client.GetAsync($"/api/filmes/{filmeCriado.Id}");
            Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

            var filmeObtido = await getByIdResponse.Content.ReadFromJsonAsync<FilmeDetalhesResponseDto>();
            Assert.NotNull(filmeObtido);
            Assert.Equal(filmeCriado.Id, filmeObtido.Id);
        }

        [Fact]
        public async Task PostFilme_ComDadosInvalidos_DeveRetornarBadRequest()
        {
            // Arrange (Título vazio e nota inválida)
            var filmeInvalido = new FilmeCreateDto
            {
                Titulo = "",
                Genero = "Drama",
                AnoLancamento = 2020,
                NotaImdb = 15.0m // Máximo é 10
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/filmes", filmeInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
