# 🎬 CP4 - API de Filmes e Avaliações
> **Advanced Business Development with .NET - FIAP 2026**  
> *Arquitetura Limpa, Otimização de Performance, Testes Automatizados e Observabilidade*

---

## 📌 1. Visão Geral do Projeto

Esta API RESTful corporativa foi desenvolvida em **.NET 8** utilizando **Controllers**, adotando os princípios do **Clean Architecture**, padrões enterprise de projeto (**Repository Pattern**, **DTOs tipados** com extensões de mapeamento) e recursos avançados de resiliência, performance e monitoramento.

O domínio aborda a gestão de um catálogo cinematográfico e suas respectivas avaliações da comunidade, composto por duas entidades fortemente relacionadas:
- **`Filme`**: Dados cadastrais do filme, gênero, ano de lançamento e nota IMDb.
- **`Avaliacao`**: Crítica de usuários com nota (1 a 5 estrelas) e comentário associado a um filme específico.

---

## 🏗️ 2. Arquitetura da Solução

A solução foi estruturada rigorosamente segundo os pilares do **Clean Architecture**, segregando responsabilidades em 6 projetos independentes:

```
CP4_FilmesApi/
│
├── 🧠 CP4.Domain/               # Camada de Domínio (Entidades, Interfaces e Exceções de Negócio)
│   ├── Entities/                # Filme.cs, Avaliacao.cs
│   ├── Exceptions/              # DomainValidationException.cs
│   └── Interfaces/              # IFilmeRepository.cs, IAvaliacaoRepository.cs
│
├── ⚙️ CP4.Application/          # Camada de Aplicação (Casos de Uso, DTOs, Mapeamentos)
│   ├── DTOs/                    # Common (PagedRequest, PagedResult), Filmes, Avaliacoes
│   ├── Interfaces/              # IFilmeService.cs, IAvaliacaoService.cs
│   ├── Mappers/                 # FilmeMapper.cs, AvaliacaoMapper.cs (Métodos de extensão puros)
│   └── Services/                # FilmeService.cs, AvaliacaoService.cs (Logging estruturado)
│
├── 🗄️ CP4.Infrastructure/       # Camada de Infraestrutura (Banco de Dados, EF Core, Repositórios)
│   ├── Data/                    # AppDbContext.cs
│   ├── Data/Configurations/     # FilmeConfiguration.cs, AvaliacaoConfiguration.cs (Índices e Fluent API)
│   └── Repositories/            # FilmeRepository.cs, AvaliacaoRepository.cs (Consultas paginadas)
│
├── 🌐 CP4.Api/                  # Camada de Apresentação / Web API (.NET 8 Controllers)
│   ├── Controllers/             # FilmesController.cs, AvaliacoesController.cs
│   ├── Middlewares/             # GlobalExceptionMiddleware.cs
│   ├── appsettings.json         # Configurações de Conexão e App Insights
│   └── Program.cs               # Pipeline HTTP, Rate Limiter, Health Checks, Compression
│
├── 🧪 CP4.UnitTests/            # Testes de Unidade (xUnit + Moq)
│   ├── Domain/                  # Validações de entidades e regras de negócio
│   └── Services/                # Serviços de aplicação isolados com mocks
│
└── 🔬 CP4.IntegrationTests/     # Testes de Integração / Funcionais (xUnit + WebApplicationFactory)
    ├── CustomWebApplicationFactory.cs
    ├── HealthCheckTests.cs      # Teste do endpoint /health
    └── FilmesEndpointsTests.cs  # Ciclo real HTTP de POST, GET paginado e validação 400
```

---

## 🚀 3. Funcionalidades e Requisitos Atendidos

| Requisito | Implementação Técnica |
|---|---|
| **Clean Architecture** | 4 camadas desacopladas (`Domain`, `Application`, `Infrastructure`, `Api`) |
| **Repository Pattern & DTOs** | Repositórios isolados, DTOs de entrada/saída, validações e proteção de entidades |
| **Paginação de Resultados** | `PagedRequest` (`PageNumber`, `PageSize`) com `Skip()` e `Take()` e envelope `PagedResult<T>` |
| **Índices de Banco de Dados** | Mapeamento Fluent API com índices (`IX_FILME_TITULO`, `IX_FILME_GENERO`, `IX_AVALIACAO_FILMEID`, `IX_AVALIACAO_USUARIO`) |
| **Response Compression** | Compressão dinâmica via **Brotli** e **Gzip** com prioridade de throughput |
| **Rate Limiting** | Limitador nativo do .NET 8 com janela fixa (10 req/10s) retornando HTTP `429 Too Many Requests` |
| **Swagger Avançado** | `Swashbuckle.AspNetCore.Annotations` (`[SwaggerOperation]`, `[ProducesResponseType]`, XML docs) |
| **Observabilidade (Health Checks)** | Endpoint `/health` monitorando integridade da aplicação e do `DbContext` |
| **Observabilidade (Logging)** | `ILogger` estruturado nos serviços e no middleware global de exceções |
| **Observabilidade (App Insights)** | Telemetria e métricas prontas para integração via connection string |
| **Testes Automatizados** | **14 testes** (11 unitários com Moq + 3 de integração com `WebApplicationFactory`) |

---

## ⚙️ 4. Configuração do Ambiente

### 4.1 Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 ou Visual Studio Code
- (Opcional) Banco de Dados Oracle

### 4.2 Configuração do `appsettings.json` (`CP4.Api`)
Abra o arquivo `CP4.Api/appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "OracleConnection": "DATA SOURCE=oracle.fiap.com.br:1521/ORCL;USER ID=SEU_RM;PASSWORD=SUA_SENHA;"
  },
  "ApplicationInsights": {
    "ConnectionString": "SEU_APPLICATION_INSIGHTS_KEY"
  }
}
```

> 💡 **Nota de Resiliência (Fallback Automático):**  
> Caso a connection string do Oracle não seja preenchida ou contenha o placeholder `SEU_RM`, a API inicializa automaticamente com um **Banco em Memória (`UseInMemoryDatabase`)**. Isso permite testar e demonstrar o sistema mesmo offline ou fora do laboratório da FIAP!

---

## 💻 5. Como Executar o Projeto

### Pelo Visual Studio:
1. Abra a solução `CP4_FilmesApi.sln`.
2. Defina o projeto **`CP4.Api`** como *Startup Project* (botão direito -> *Set as Startup Project*).
3. Pressione `F5` ou `Ctrl + F5`.
4. O navegador abrirá automaticamente na interface do **Swagger** em `http://localhost:5000/` (ou porta configurada).

### Pelo Terminal / CLI:
```bash
# Restaurar dependências
dotnet restore

# Compilar toda a solução
dotnet build

# Executar a API
cd CP4.Api
dotnet run
```

---

## 🧪 6. Como Executar os Testes Automatizados

Para rodar todos os testes de unidade e integração:

```bash
dotnet test
```

Saída esperada:
```text
Passed! - Failed: 0, Passed: 14, Skipped: 0, Total: 14, Duration: ~1.2s
```

---

## 📡 7. Documentação dos Endpoints Principais

### 🎞️ Filmes (`/api/filmes`)

#### `GET /api/filmes?pageNumber=1&pageSize=10&genero=Ficção`
Lista filmes com paginação e filtro opcional.
**Exemplo de Resposta (200 OK):**
```json
{
  "items": [
    {
      "id": 1,
      "titulo": "Interestelar",
      "genero": "Ficção Científica",
      "anoLancamento": 2014,
      "notaImdb": 8.7,
      "dataCadastro": "2026-09-15T15:00:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

#### `GET /api/filmes/{id}`
Obtém os detalhes do filme e a lista de suas avaliações.

#### `POST /api/filmes`
Cadastra um novo filme.
**Payload:**
```json
{
  "titulo": "Duna: Parte 2",
  "genero": "Ficção Científica",
  "anoLancamento": 2024,
  "notaImdb": 8.6
}
```

#### `PUT /api/filmes/{id}`
Atualiza os dados de um filme existente.

#### `DELETE /api/filmes/{id}`
Remove um filme e todas as suas avaliações associadas (Cascade).

---

### ⭐ Avaliações (`/api/avaliacoes`)

#### `GET /api/avaliacoes/filme/{filmeId}`
Lista as avaliações de um filme específico.

#### `POST /api/avaliacoes`
Registra uma crítica de usuário para determinado filme.
**Payload:**
```json
{
  "filmeId": 1,
  "usuario": "Thomas",
  "comentario": "Obra-prima do cinema moderno!",
  "nota": 5
}
```

#### `DELETE /api/avaliacoes/{id}`
Remove uma avaliação pelo ID.

---

### 🩺 Monitoramento (`/health`)

#### `GET /health`
Verifica a integridade da API e do Banco de Dados.
**Exemplo de Resposta (200 OK):**
```json
{
  "status": "Healthy",
  "totalDuration": "4.2 ms",
  "dependencies": [
    {
      "name": "Banco_De_Dados",
      "status": "Healthy",
      "duration": "3.8 ms"
    }
  ]
}
```
