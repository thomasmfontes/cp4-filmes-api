# 🎙️ DOSSIÊ COMPLETO DE APRESENTAÇÃO E DEFESA TÉCNICA - CP4 (2026)
> **Advanced Business Development with .NET - FIAP**  
> **Tema:** API RESTful de Filmes e Avaliações (Clean Architecture, Performance, Resiliência e Observabilidade)  
> **Uso:** Este documento foi estruturado tanto para **treinar e guiar os integrantes do grupo** durante a apresentação presencial quanto para **alimentar prompts de IAs** caso necessitem gerar simulações de perguntas e respostas.

---

## 📑 ÍNDICE RÁPIDO
1. [Visão Executiva do Projeto](#-1-visão-executiva-do-projeto)
2. [Arquitetura & Decisões Técnicas (A Teoria Exigida)](#-2-arquitetura--decisões-técnicas)
3. [Roteiro da Apresentação de 5 Minutos (Passo a Passo Cronometrado)](#-3-roteiro-da-apresentação-de-5-minutos)
4. [Guia de Defesa: Perguntas Prováveis do Professor & Respostas Prontas](#-4-guia-de-defesa-perguntas-do-professor--respostas)
5. [Checklist Pré-Apresentação (Para não errar na hora)](#-5-checklist-pré-apresentação)

---

## 🏛️ 1. VISÃO EXECUTIVA DO PROJETO

- **Objetivo:** Construir uma Web API empresarial em **.NET 8** utilizando **Controllers**, aplicando **Clean Architecture**, **Repository Pattern**, **DTOs tipados**, **Paginação com Índices**, **Response Compression**, **Rate Limiting**, **Observabilidade** (Health Checks, Logging e Application Insights) e **Testes Automatizados** (Unidade e Integração).
- **Domínio Escolhido:** Catálogo de Filmes e Avaliações de Usuários (relacionamento `1 : N`).
  - `Filme`: Id, Titulo, Genero, AnoLancamento, NotaImdb, DataCadastro, Avaliacoes.
  - `Avaliacao`: Id, Usuario, Comentario, Nota (1 a 5), FilmeId, DataAvaliacao.
- **Estrutura da Solution (`CP4_FilmesApi.sln`):** 6 projetos desacoplados:
  1. `CP4.Domain` (Core puro sem dependências externas)
  2. `CP4.Application` (Casos de uso, DTOs, Mappers, Interfaces de serviço)
  3. `CP4.Infrastructure` (EF Core, Oracle/InMemory fallback, Repositórios, Mapeamentos Fluent API)
  4. `CP4.Api` (Controllers, Middlewares, Rate Limiter, Compression, Health Checks)
  5. `CP4.UnitTests` (11 testes com xUnit e Moq para regras de domínio e serviços)
  6. `CP4.IntegrationTests` (3 testes com xUnit e WebApplicationFactory para ciclo HTTP completo)

---

## 🧠 2. ARQUITETURA & DECISÕES TÉCNICAS

### 2.1 Por que Clean Architecture?
- **Regra de Dependência:** O domínio está no centro. Nenhuma camada interna conhece as camadas externas.
- **Independência de Frameworks e Banco:** O banco de dados ou a API podem mudar sem alterar as regras de negócio centrais (`Domain`).
- **Testabilidade:** Conseguimos testar 100% das regras de negócio mockando apenas contratos (`Interfaces`).

### 2.2 Repository Pattern & DTOs
- **Por que usamos Repositórios?** Isolamos o Entity Framework Core da camada de negócio (`Application`). Caso troquemos o ORM ou façamos consultas complexas via Dapper, a camada de negócio permanece intocada.
- **Por que usamos DTOs em vez de expor as Entidades?**
  - Evita **Over-Posting** e **Mass Assignment** (um usuário malicioso não consegue alterar o `Id` ou a `DataCadastro`).
  - Evita problemas de **referência cíclica** na serialização JSON (`Filme -> Avaliacoes -> Filme`).
  - Oculta propriedades internas de auditoria ou regras que não dizem respeito ao consumidor da API.
- **Estratégia de Mapeamento:** Optamos por **métodos de extensão manuais** (`FilmeMapper.cs` e `AvaliacaoMapper.cs`). São fortemente tipados, não geram overhead em tempo de execução como AutoMapper e são fáceis de debugar e explicar.

### 2.3 Paginação & Índices de Banco
- **Paginação:** Implementada via `Skip((pageNumber - 1) * pageSize).Take(pageSize)` na query do EF Core com `AsNoTracking()` para otimizar alocação de memória e não rastrear entidades de leitura.
- **Envelope de Resposta (`PagedResult<T>`):** Retorna não apenas a lista, mas também o `TotalCount`, `PageNumber`, `PageSize`, `TotalPages`, `HasPreviousPage` e `HasNextPage`.
- **Índices de Banco (Fluent API):**
  - `IX_FILME_TITULO`: Otimiza buscas e a ordenação padrão por título.
  - `IX_FILME_GENERO`: Evita varredura de tabela completa (*Table Scan*) ao filtrar filmes por gênero.
  - `IX_AVALIACAO_FILMEID`: Acelera a consulta de todas as avaliações de um filme específico (chave estrangeira indexada).
  - `IX_AVALIACAO_USUARIO`: Acelera buscas por autor da crítica.

### 2.4 Performance & Resiliência
- **Response Compression:** Registramos os provedores nativos **Brotli** e **Gzip**. O Brotli oferece taxas de compressão superiores para dados textuais/JSON, enquanto o Gzip garante compatibilidade universal com clientes legados.
- **Rate Limiting:** Implementado com a biblioteca nativa do .NET 8 (`System.Threading.RateLimiting`), configurado com a política de **Janela Fixa (Fixed Window)** de **10 requisições a cada 10 segundos** por cliente, rejeitando excessos com HTTP `429 Too Many Requests`.

### 2.5 Observabilidade
- **Health Checks (`/health`):** Retorna um payload JSON customizado contendo o status global (`Healthy`), tempo total de resposta e a saúde individual do componente `Banco_De_Dados` (`AddDbContextCheck<AppDbContext>`).
- **Logging Estruturado:** Injeção de `ILogger<T>` em todos os serviços e controllers, além do `GlobalExceptionMiddleware`, registrando mensagens semânticas com parâmetros (`{Titulo}`, `{FilmeId}`).
- **Application Insights:** Configurado no `Program.cs` para coleta de telemetria de requisições, dependências e exceções no Azure.

---

## ⏱️ 3. ROTEIRO DA APRESENTAÇÃO DE 5 MINUTOS

> 💡 **Dica para o Grupo:** Deixem o Visual Studio aberto com a Solution compilada e o navegador aberto no Swagger antes de começar.

---

### ⏱️ Minuto 1: Introdução & Arquitetura (Apresentador 1)
- **O que falar:**
  > *"Boa tarde, professor! Nosso projeto é uma API de Filmes e Avaliações desenvolvida em .NET 8 com foco em Clean Architecture, alta performance, testes e observabilidade.*  
  > *Como o senhor pode ver no Solution Explorer, dividimos a solução em 6 projetos:*  
  > *O **Domain**, que é isolado e contém nossas entidades `Filme` e `Avaliacao` e as interfaces dos repositórios;*  
  > *O **Application**, onde residem os DTOs de entrada/saída, mapeadores e serviços de negócio com logging estruturado;*  
  > *O **Infrastructure**, responsável pelo `AppDbContext`, mapeamentos via Fluent API com índices e a persistência;*  
  > *E a camada **Api**, que expõe os controllers com Swagger avançado, Rate Limiting e compressão."*
- **O que mostrar na tela:** Mostrar o Solution Explorer expandido com os 6 projetos.

---

### ⏱️ Minuto 2: Execução dos Testes Automatizados (Apresentador 1 ou 2)
- **O que falar:**
  > *"Antes de demonstrar os endpoints, gostaríamos de mostrar a nossa suíte de testes automatizados. Temos 14 testes cobrindo unidade e integração.*  
  > *Nos testes de unidade com xUnit e Moq, testamos as validações das entidades (como regras de ano e nota IMDb) e os serviços isolados.*  
  > *Nos testes de integração com `WebApplicationFactory`, validamos o ciclo real HTTP dos endpoints e a resposta do Health Check."*
- **O que mostrar na tela:** Abrir o **Test Explorer** e clicar em **Run All** (ou rodar `dotnet test` no terminal). Mostrar que todos os 14 testes passaram com sucesso (100% verde).

---

### ⏱️ Minuto 3: Demonstração do Swagger & Paginação (Apresentador 2)
- **O que falar:**
  > *"Agora com a API em execução, estamos no Swagger. Enriquecemos a documentação utilizando Swagger Annotations, especificando os summaries, descrições e status codes possíveis.*  
  > *Vamos demonstrar a **Paginação de Resultados**: no endpoint `GET /api/filmes`, enviamos `pageNumber = 1` e `pageSize = 2`. Repare que o retorno traz o envelope completo `PagedResult` com os itens, totalCount de registros e controle de páginas anteriores e próximas.*  
  > *No banco, mapeamos índices específicos para essa busca por gênero e título no `FilmeConfiguration`."*
- **O que mostrar na tela:** Fazer a requisição no Swagger em `GET /api/filmes` passando os parâmetros de paginação e mostrar o JSON formatado.

---

### ⏱️ Minuto 4: Demonstração do Rate Limiting e Response Compression (Apresentador 2 ou 3)
- **O que falar:**
  > *"Para segurança e proteção contra abusos, implementamos o **Rate Limiting** nativo do .NET 8 com janela fixa de 10 requisições por 10 segundos.*  
  > *Se dispararmos várias requisições consecutivas no Swagger... pronto: a API bloqueia imediatamente e retorna o status HTTP `429 Too Many Requests` com uma mensagem informativa.*  
  > *Além disso, temos o middleware de **Response Compression** ativo com Brotli e Gzip, reduzindo o payload de rede para clientes que suportam compressão."*
- **O que mostrar na tela:** Clicar rapidamente no botão "Execute" do Swagger até disparar o `429 Too Many Requests`.

---

### ⏱️ Minuto 5: Health Check & Observabilidade (Apresentador 3)
- **O que falar:**
  > *"Por fim, na parte de observabilidade, implementamos o endpoint `/health`.*  
  > *Ele monitora não apenas o runtime da aplicação, mas também a integridade da conexão com a dependência do banco de dados.*  
  > *Também temos o Application Insights configurado para coleta de telemetria e o `GlobalExceptionMiddleware` com injeção de `ILogger` para captura de logs estruturados de qualquer falha na aplicação.*  
  > *Com isso, atendemos 100% dos requisitos de arquitetura, qualidade e resiliência propostos pelo CP4. Estamos abertos a dúvidas!"*
- **O que mostrar na tela:** Acessar a rota `http://localhost:XXXX/health` no navegador e mostrar o JSON com `"status": "Healthy"`.

---

## 🛡️ 4. GUIA DE DEFESA: PERGUNTAS DO PROFESSOR & RESPOSTAS

### ❓ Pergunta 1: "Por que vocês usaram DTOs e não usaram as entidades diretamente no Controller?"
**Resposta Recomendada:**
> *"Usamos DTOs por três motivos principais:  
> 1. **Segurança (Over-Posting):** Evita que o cliente envie campos que não deveriam ser alterados diretamente, como IDs autoincrementais ou datas de cadastro.  
> 2. **Desacoplamento e Encapsulamento:** As entidades de domínio possuem regras e comportamentos internos; os DTOs representam apenas os contratos públicos da API.  
> 3. **Serialização:** Evita o problema clássico de referência circular infinita entre Filme e Avaliação durante a serialização JSON."*

---

### ❓ Pergunta 2: "Como foi implementada a paginação e como ela se relaciona com os índices de banco?"
**Resposta Recomendada:**
> *"A paginação foi implementada a nível de banco de dados no repositório utilizando `Skip((pageNumber - 1) * pageSize).Take(pageSize)` combinado com `AsNoTracking()`. Isso garante que apenas os registros daquela página sejam carregados do banco para a memória da API.  
> E para que essa query seja rápida mesmo com milhões de registros, configuramos no Fluent API os índices `IX_FILME_TITULO` e `IX_FILME_GENERO`, evitando que o banco faça um Table Scan completo na ordenação e no filtro por gênero."*

---

### ❓ Pergunta 3: "O que acontece se uma requisição violar uma regra de negócio? Onde isso é tratado?"
**Resposta Recomendada:**
> *"Quando uma entidade ou serviço detecta uma inconsistência (como nota fora do intervalo ou título vazio), é disparada a exceção customizada `DomainValidationException`.  
> Essa exceção é capturada pelo nosso `GlobalExceptionMiddleware`, que gera um log de aviso estruturado via `ILogger` e converte a resposta em um HTTP `400 Bad Request` padronizado em JSON, sem expor o stack trace interno do servidor."*

---

### ❓ Pergunta 4: "Como vocês configuraram e testaram o Rate Limiting?"
**Resposta Recomendada:**
> *"Utilizamos a biblioteca nativa do .NET 8 (`Microsoft.AspNetCore.RateLimiting`). Configuramos uma política de janela fixa (`AddFixedWindowLimiter`) permitindo até 10 requisições por janela de 10 segundos, definindo explicitamente o `RejectionStatusCode = StatusCodes.Status429TooManyRequests`. Testamos disparando mais de 10 chamadas no Swagger dentro da janela, e a API retornou o status 429 com o cabeçalho de rejeição."*

---

### ❓ Pergunta 5: "Qual a diferença entre os testes que vocês criaram em `UnitTests` e em `IntegrationTests`?"
**Resposta Recomendada:**
> *"No `UnitTests`, testamos classes isoladas em milissegundos sem subir nenhum servidor real nem banco de dados. Usamos o `Moq` para simular as dependências (como repositórios e loggers), focando puramente nas regras de negócio e validações.  
> Já no `IntegrationTests`, utilizamos o `WebApplicationFactory<Program>` para instanciar a API inteira em memória, testando o pipeline real de requisições HTTP: roteamento, model binding, validação, serialização de DTOs e resposta de status codes reais."*

---

### ❓ Pergunta 6: "Como está configurado o banco de dados e como funciona caso estejamos sem acesso ao Oracle da FIAP?"
**Resposta Recomendada:**
> *"No `Program.cs`, configuramos o Entity Framework Core com o provedor da Oracle (`Oracle.EntityFrameworkCore`). Porém, como boa prática de resiliência e portabilidade para testes e apresentações, incluímos um fallback condicional: se a connection string do Oracle não estiver ativa ou contiver o placeholder padrão, a API inicializa com um banco em memória (`UseInMemoryDatabase`). Isso garante que a aplicação execute com 100% de confiabilidade em qualquer ambiente."*

---

## ✅ 5. CHECKLIST PRÉ-APRESENTAÇÃO

1. [ ] **Verificar compilação:** Dar `Ctrl + Shift + B` no Visual Studio para garantir **0 erros e 0 avisos**.
2. [ ] **Rodar os testes:** No Test Explorer, garantir que os 14 testes estão passando.
3. [ ] **Iniciar a API:** Pressionar `Ctrl + F5` para subir a API sem o depurador preso.
4. [ ] **Deixar abertas as seguintes abas no navegador:**
   - Aba 1: `http://localhost:XXXX/` (Swagger UI)
   - Aba 2: `http://localhost:XXXX/health` (Health Check)
5. [ ] **Testar o Rate Limit previamente:** Clicar rapidamente no Swagger para ter certeza de que o 429 é exibido.
6. [ ] **Respirar fundo:** O projeto está 100% aderente a todos os 10 critérios da grade do professor!
