# Requisitos do Sistema — GoodHamburger

REST API para gestão de uma hamburgueria digital, construída com Clean Architecture e .NET 7.0.

---

## Índice

- [Requisitos Funcionais](#requisitos-funcionais)
  - [RF01 — Gestão de Clientes](#rf01--gestão-de-clientes)
  - [RF02 — Gestão de Menu](#rf02--gestão-de-menu)
  - [RF03 — Gestão de Acompanhamentos](#rf03--gestão-de-acompanhamentos)
  - [RF04 — Gestão de Pedidos](#rf04--gestão-de-pedidos)
  - [RF05 — Regras de Desconto](#rf05--regras-de-desconto)
  - [RF06 — Ciclo de Vida do Pedido](#rf06--ciclo-de-vida-do-pedido)
  - [RF07 — Validação de Dados](#rf07--validação-de-dados)
  - [RF08 — Paginação de Listagens](#rf08--paginação-de-listagens)
- [Requisitos Não Funcionais](#requisitos-não-funcionais)
  - [RNF01 — Arquitetura](#rnf01--arquitetura)
  - [RNF02 — Persistência de Dados](#rnf02--persistência-de-dados)
  - [RNF03 — API e Contratos HTTP](#rnf03--api-e-contratos-http)
  - [RNF04 — Tratamento de Erros](#rnf04--tratamento-de-erros)
  - [RNF05 — Segurança](#rnf05--segurança)
  - [RNF06 — Desempenho](#rnf06--desempenho)
  - [RNF07 — Observabilidade](#rnf07--observabilidade)
  - [RNF08 — Testabilidade](#rnf08--testabilidade)
  - [RNF09 — Containerização](#rnf09--containerização)
  - [RNF10 — Configuração e Ambiente](#rnf10--configuração-e-ambiente)
- [Modelo de Dados](#modelo-de-dados)
- [Mapeamento de Exceções para HTTP](#mapeamento-de-exceções-para-http)

---

## Requisitos Funcionais

### RF01 — Gestão de Clientes

#### RF01.1 — Criar Cliente
- O sistema deve permitir criar um cliente fornecendo: **nome**, **apelido**, **email**, **telefone** e **morada** (opcional).
- O **telefone** deve ser único no sistema — não é permitido registar dois clientes com o mesmo número.
- Em caso de telefone duplicado, o sistema retorna **409 Conflict**.
- Em caso de sucesso, retorna os dados do cliente criado com **201 Created**.

#### RF01.2 — Consultar Cliente por ID
- O sistema deve retornar os dados de um cliente dado o seu identificador único (UUID).
- Se o cliente não existir, retorna **404 Not Found**.

#### RF01.3 — Listar Clientes (paginado)
- O sistema deve retornar uma lista paginada de todos os clientes.
- Parâmetros de paginação: `page` (padrão: 1, mínimo: 1) e `pageSize` (padrão: 10, máximo: 100).
- A resposta inclui: itens, página atual, tamanho da página, total de itens e total de páginas.

#### RF01.4 — Atualizar Cliente
- O sistema deve permitir atualizar todos os dados de um cliente existente.
- O cliente deve existir — caso contrário, **404 Not Found**.
- O novo telefone deve ser único, excluindo o próprio cliente — caso contrário, **409 Conflict**.

#### RF01.5 — Eliminar Cliente
- O sistema deve permitir eliminar um cliente pelo seu ID.
- O cliente deve existir — caso contrário, **404 Not Found**.
- A eliminação é física (sem soft delete).

---

### RF02 — Gestão de Menu

#### RF02.1 — Criar Item de Menu
- O sistema deve permitir criar um item de menu (sanduíche) com: **nome**, **descrição** (opcional), **preço** e **moeda**.
- O **nome** deve ser único no sistema.
- O **preço** deve ser superior a zero.
- A **moeda** deve ser um valor válido do enum `Currency` (USD ou BRL).
- Em caso de nome duplicado, retorna **409 Conflict**.
- Em caso de sucesso, retorna **201 Created**.

#### RF02.2 — Consultar Item de Menu por ID
- O sistema deve retornar os dados de um item de menu dado o seu ID.
- Se não existir, retorna **404 Not Found**.

#### RF02.3 — Listar Itens de Menu (paginado)
- Listagem paginada de todos os itens de menu (parâmetros idênticos ao RF01.3).

#### RF02.4 — Atualizar Item de Menu
- O sistema deve permitir atualizar: nome, descrição, preço, moeda e estado (`Available` / `Unavailable`).
- O item deve existir — caso contrário, **404 Not Found**.
- O nome deve continuar único (excluindo o próprio item) — caso contrário, **409 Conflict**.

#### RF02.5 — Eliminar Item de Menu
- O sistema deve permitir eliminar um item de menu pelo seu ID.
- O item deve existir — caso contrário, **404 Not Found**.
- A eliminação é física.

---

### RF03 — Gestão de Acompanhamentos

Os acompanhamentos (`SideDishes`) representam os extras que podem ser adicionados a um pedido: **batata frita** (`FRIES`) e **bebida** (`DRINK`).

#### RF03.1 — Criar Acompanhamento
- O sistema deve permitir criar um acompanhamento com: **nome**, **descrição** (opcional), **preço**, **categoria** e **moeda**.
- O **nome** deve ser único no sistema.
- O **preço** deve ser superior a zero.
- A **categoria** deve ser `FRIES` ou `DRINK` (enum `SideDishCategory`).
- A **moeda** deve ser um valor válido do enum `Currency`.
- Em caso de nome duplicado, retorna **409 Conflict**.
- Em caso de sucesso, retorna **201 Created**.

#### RF03.2 — Consultar Acompanhamento por ID
- O sistema deve retornar os dados de um acompanhamento dado o seu ID.
- Se não existir, retorna **404 Not Found**.

#### RF03.3 — Listar Acompanhamentos (paginado)
- Listagem paginada de todos os acompanhamentos.

#### RF03.4 — Atualizar Acompanhamento
- O sistema deve permitir atualizar: nome, descrição, preço, categoria, moeda e estado.
- O item deve existir — caso contrário, **404 Not Found**.
- O nome deve continuar único — caso contrário, **409 Conflict**.

#### RF03.5 — Eliminar Acompanhamento
- O sistema deve permitir eliminar um acompanhamento pelo seu ID.
- O item deve existir — caso contrário, **404 Not Found**.

---

### RF04 — Gestão de Pedidos

#### RF04.1 — Criar Pedido
- O sistema deve permitir criar um pedido com: **ID do cliente**, **ID do menu** (sanduíche) e uma lista opcional de **IDs de acompanhamentos** (máximo 2).
- O cliente deve existir — caso contrário, **404 Not Found**.
- O item de menu deve existir — caso contrário, **404 Not Found**.
- Cada acompanhamento indicado deve existir — caso contrário, **404 Not Found**.
- O `OrderNumber` é gerado automaticamente como o total de pedidos existentes + 1.
- Os totais (subtotal, desconto, total) são calculados automaticamente no momento da criação.
- Um pedido é criado sempre com o estado `PENDING`.
- A operação usa transação de base de dados com rollback em caso de falha.
- Em caso de sucesso, retorna **201 Created** com os dados completos do pedido (itens e acompanhamentos incluídos).

#### RF04.2 — Consultar Pedido por ID
- O sistema deve retornar os dados completos de um pedido dado o seu ID, incluindo os itens e os acompanhamentos de cada item.
- Se não existir, retorna **404 Not Found**.

#### RF04.3 — Listar Pedidos (paginado)
- Listagem paginada de todos os pedidos, com os seus itens e acompanhamentos carregados.

#### RF04.4 — Confirmar Pedido
- O sistema deve permitir confirmar um pedido pelo seu ID.
- O pedido deve existir — caso contrário, **404 Not Found**.
- O pedido deve estar no estado `PENDING` e conter pelo menos um item — caso contrário, **422 Unprocessable Entity**.
- Após confirmação, o estado passa para `CONFIRMED`.

#### RF04.5 — Cancelar Pedido
- O sistema deve permitir cancelar um pedido pelo seu ID.
- O pedido deve existir — caso contrário, **404 Not Found**.
- Um pedido com estado `DELIVERED` não pode ser cancelado — **422 Unprocessable Entity**.
- Um pedido já no estado `CANCELLED` não pode ser cancelado novamente — **422 Unprocessable Entity**.
- Após cancelamento, o estado passa para `CANCELLED`.

#### RF04.6 — Eliminar Pedido
- O sistema deve permitir eliminar um pedido pelo seu ID.
- O pedido deve existir — caso contrário, **404 Not Found**.
- Apenas pedidos nos estados `PENDING` ou `CANCELLED` podem ser eliminados — caso contrário, **422 Unprocessable Entity**.
- A eliminação remove em cascata: acompanhamentos dos itens, itens e o próprio pedido.
- A operação usa transação de base de dados com rollback em caso de falha.

---

### RF05 — Regras de Desconto

O desconto é calculado automaticamente com base nos acompanhamentos incluídos no pedido. É aplicado sobre o **subtotal** (soma de todos os itens e acompanhamentos).

| Acompanhamentos incluídos | Desconto aplicado |
|---------------------------|:-----------------:|
| Batata frita (`FRIES`) + Bebida (`DRINK`) — combo | **20%** |
| Apenas bebida (`DRINK`) | **15%** |
| Apenas batata frita (`FRIES`) | **10%** |
| Nenhum acompanhamento | **0%** |

- O desconto é armazenado como percentagem (ex: `20`, não `0.20`).
- O valor de `Total = Subtotal × (1 - Desconto / 100)`.
- O recálculo é realizado automaticamente a cada alteração dos itens do pedido.

**Invariantes:**
- Um pedido pode ter **no máximo um acompanhamento por categoria** (1 FRIES + 1 DRINK).
- Não é possível adicionar dois acompanhamentos da mesma categoria — **422 Unprocessable Entity**.
- Não é possível adicionar acompanhamentos sem antes adicionar um sanduíche — **422 Unprocessable Entity**.
- Um pedido só pode ter **um único sanduíche** — não é permitido adicionar dois itens de menu ao mesmo pedido.

---

### RF06 — Ciclo de Vida do Pedido

```
PENDING ──→ CONFIRMED ──→ PAID ──→ READY ──→ DELIVERED
   │
   └──→ CANCELLED  (a partir de qualquer estado exceto DELIVERED)
```

| Transição | Condição | Operação |
|-----------|----------|----------|
| `PENDING` → `CONFIRMED` | Pedido com itens | `PUT /orders/{id}/confirm` |
| `PENDING` → `CANCELLED` | — | `PUT /orders/{id}/cancel` |
| `CONFIRMED` → `CANCELLED` | — | `PUT /orders/{id}/cancel` |
| `DELIVERED` → qualquer | **Proibido** | Retorna 422 |
| `CANCELLED` → `CANCELLED` | **Proibido** | Retorna 422 |

> Os estados `PAID`, `READY` e `DELIVERED` existem no domínio mas não têm use cases dedicados na versão atual.

---

### RF07 — Validação de Dados

Toda a validação é feita no lado do servidor, antes de chegar ao use case. Campos inválidos retornam **400 Bad Request** com erros agrupados por campo.

#### Cliente
| Campo | Regras |
|-------|--------|
| `FirstName` | Obrigatório, máximo 100 caracteres |
| `LastName` | Obrigatório, máximo 100 caracteres |
| `Phone` | Obrigatório, regex `^\+?[0-9]{9,15}$` (9–15 dígitos, prefixo `+` opcional) |
| `Email` | Obrigatório, formato de email válido |

#### Menu
| Campo | Regras |
|-------|--------|
| `Name` | Obrigatório, máximo 100 caracteres |
| `Price` | Obrigatório, maior que zero |
| `Currency` | Valor válido do enum `Currency` (USD=1, BRL=2) |
| `Status` (update) | Valor válido do enum `MenuStatus` |

#### Acompanhamento
| Campo | Regras |
|-------|--------|
| `Name` | Obrigatório, máximo 100 caracteres |
| `Price` | Obrigatório, maior que zero |
| `Category` | Valor válido do enum `SideDishCategory` (`FRIES` ou `DRINK`) |
| `Currency` | Valor válido do enum `Currency` |
| `Status` (update) | Valor válido do enum `MenuStatus` |

#### Pedido
| Campo | Regras |
|-------|--------|
| `CustomerId` | Obrigatório, UUID não vazio |
| `MenuId` | Obrigatório, UUID não vazio |
| `SideDishIds` | Lista opcional, máximo 2 itens |

---

### RF08 — Paginação de Listagens

Todos os endpoints de listagem suportam paginação via query parameters:

| Parâmetro | Padrão | Mínimo | Máximo |
|-----------|:------:|:------:|:------:|
| `page` | 1 | 1 | — |
| `pageSize` | 10 | 1 | 100 |

Valores inválidos (ex: `page=0`) são auto-corrigidos para os valores mínimos aceites.

**Estrutura da resposta paginada:**
```json
{
  "page": 1,
  "pageSize": 10,
  "totalItems": 42,
  "totalPages": 5,
  "items": [ ... ]
}
```

---

## Requisitos Não Funcionais

### RNF01 — Arquitetura

- **RNF01.1** — O sistema deve seguir os princípios de **Clean Architecture**, com dependências unidirecionais apontando sempre para o domínio.
- **RNF01.2** — O código deve estar organizado em 4 camadas isoladas: `Domain`, `Application`, `Infrastructure` e `API`.
- **RNF01.3** — Cada operação de negócio deve ser implementada como um **Use Case** independente (SRP — Single Responsibility Principle), evitando a acumulação de responsabilidades em services genéricos.
- **RNF01.4** — As entidades de domínio devem encapsular as regras de negócio — o domínio não deve ser anémico. Propriedades de leitura protegidas por `private set`.
- **RNF01.5** — Os Use Cases devem depender de **interfaces de repositório** (DIP), nunca de implementações concretas de infraestrutura.
- **RNF01.6** — A conversão entre entidades de domínio e DTOs deve ser feita por **Mappers estáticos** com extension methods, sem dependência de bibliotecas de mapeamento automático.
- **RNF01.7** — Os validators devem ser registados automaticamente via **reflection** no bootstrapper, sem necessidade de registo manual por classe.

---

### RNF02 — Persistência de Dados

- **RNF02.1** — O sistema deve suportar dois modos de persistência configuráveis: **SQL Server** (produção) e **InMemory EF Core** (desenvolvimento e testes), alternáveis via configuração sem alteração de código.
- **RNF02.2** — O acesso à base de dados deve ser abstraído pelo **Repository Pattern**, com um `IBaseRepository<T>` genérico e repositórios específicos para queries avançadas.
- **RNF02.3** — Operações críticas (criação e eliminação de pedidos) devem usar **transações explícitas** com `BeginTransactionAsync` / `CommitAsync` / `RollbackAsync`.
- **RNF02.4** — O mapeamento relacional deve ser configurado exclusivamente via **Fluent API** do EF Core (sem Data Annotations nas entidades).
- **RNF02.5** — As regras de cascade devem ser:
  - `Order → OrderItems`: **Cascade Delete**
  - `OrderItem → OrderSideDishes`: **Cascade Delete**
  - `Order → Customer`: **Restrict** (não elimina o cliente ao eliminar o pedido)
  - `OrderItem → Menu`: **Restrict**
- **RNF02.6** — O campo `OrderNumber` deve ter um **índice único** na base de dados.
- **RNF02.7** — Os enums devem ser armazenados como **strings** (não inteiros) na base de dados, com tamanho máximo configurado por Fluent API.
- **RNF02.8** — Os valores decimais de preço devem ser armazenados com precisão `decimal(18,2)`; o desconto com `decimal(5,2)`.
- **RNF02.9** — As **migrations** devem ser aplicadas automaticamente no startup da aplicação quando o modo SQL Server está ativo.

---

### RNF03 — API e Contratos HTTP

- **RNF03.1** — A API deve seguir os princípios **REST**, com verbos HTTP semânticos e rotas orientadas a recursos.
- **RNF03.2** — A API deve suportar **versionamento por URL** (`/api/v{version}/...`), com versão padrão `1.0`.
- **RNF03.3** — Todas as respostas devem usar o content-type `application/json` com serialização **camelCase**.
- **RNF03.4** — Campos `null` não devem ser incluídos nas respostas JSON (`WhenWritingNull`).
- **RNF03.5** — Referências circulares em objetos JSON devem ser ignoradas (`IgnoreCycles`).
- **RNF03.6** — Os códigos de resposta HTTP devem seguir a convenção:

| Situação | Código |
|----------|:------:|
| Criação bem-sucedida | 201 Created |
| Consulta / atualização bem-sucedida | 200 OK |
| Eliminação bem-sucedida | 204 No Content |
| Dados de input inválidos | 400 Bad Request |
| Não autenticado | 401 Unauthorized |
| Recurso não encontrado | 404 Not Found |
| Recurso já existe | 409 Conflict |
| Violação de regra de domínio/negócio | 422 Unprocessable Entity |
| Erro interno do servidor | 500 Internal Server Error |

- **RNF03.7** — A API deve expor documentação **OpenAPI/Swagger**, disponível no endpoint `/swagger` em ambiente de desenvolvimento.
- **RNF03.8** — Todas as respostas de erro devem seguir o formato **RFC 7807 ProblemDetails**, incluindo `status`, `title`, `detail`, `type`, `instance` e `traceId`.

---

### RNF04 — Tratamento de Erros

- **RNF04.1** — Todas as exceções não tratadas devem ser capturadas por um **middleware global** (`GlobalExceptionHandler`), que deve ser o primeiro da pipeline.
- **RNF04.2** — O middleware não deve reescrever respostas já iniciadas.
- **RNF04.3** — Em ambiente de produção, o `detail` das respostas de erro 500 deve ser genérico ("Ocorreu um erro interno. Contate o suporte."); em desenvolvimento, pode expor a mensagem real.
- **RNF04.4** — Erros de validação (400) devem incluir os erros agrupados por nome de campo no objeto `errors` da resposta.
- **RNF04.5** — A hierarquia de exceções customizadas deve ser:
  - `DomainException` — violações de regras de domínio → 422
  - `BusinessRuleException` — violações de regras de negócio da aplicação → 422
  - `NotFoundException` — recurso não encontrado → 404
  - `ResourceAlreadyExists` — conflito de unicidade → 409

---

### RNF05 — Segurança

- **RNF05.1** — A API deve ter **CORS** configurado, permitindo apenas origens explicitamente autorizadas (`https://localhost:7162`, `https://meuapp.com`).
- **RNF05.2** — O middleware de **redirecionamento HTTPS** deve estar ativo.
- **RNF05.3** — A API deve suportar **autorização** via middleware (pipeline preparada para adicionar JWT sem alteração de use cases).
- **RNF05.4** — As credenciais da base de dados não devem estar hardcoded no código-fonte — devem ser injetadas via variáveis de ambiente ou ficheiros de configuração externos.

---

### RNF06 — Desempenho

- **RNF06.1** — Todas as operações de I/O (base de dados, escrita de resposta) devem ser **assíncronas** (`async/await`).
- **RNF06.2** — Todos os métodos que acedam a recursos externos devem suportar **`CancellationToken`** para cancelamento cooperativo.
- **RNF06.3** — As listagens devem ser sempre **paginadas**, com um máximo de 100 itens por página, para evitar leituras completas de tabelas grandes.
- **RNF06.4** — As coleções de itens nas entidades (`OrderItems`, `OrderSideDishes`) devem usar **field accessor** no EF Core, protegendo o encapsulamento sem penalizar a performance de carregamento.
- **RNF06.5** — O campo `SideDishesId` na tabela `OrderSideDishes` deve ser **indexado** para acelerar queries por acompanhamento.

---

### RNF07 — Observabilidade

- **RNF07.1** — Todas as operações de use case devem registar eventos no log com **nível estruturado** (Information, Warning, Error).
- **RNF07.2** — Operações bem-sucedidas devem registar logs com **parâmetros nomeados** (ex: `Id={CustomerId}`, `Name={FirstName}`) para facilitar correlação.
- **RNF07.3** — Tentativas de criação de recursos duplicados devem gerar logs de nível **Warning**, não Error.
- **RNF07.4** — Todas as respostas de erro devem incluir o **`traceId`** da requisição HTTP nas extensões do ProblemDetails, para rastreabilidade.

---

### RNF08 — Testabilidade

- **RNF08.1** — O sistema deve ter **141 testes automatizados**, todos a passar, organizados em 3 projetos:
  - `DomainTest` — 38 testes unitários de entidades de domínio (sem mocks, lógica pura)
  - `Validators` — 52 testes unitários de validators FluentValidation
  - `UseCaseTest` — 51 testes unitários de use cases com mocks
- **RNF08.2** — Os testes de use case devem usar **mocks de repositório** (Moq) e nunca aceder à base de dados real.
- **RNF08.3** — Os testes de domínio devem instanciar as entidades diretamente, sem mocks, para verificar as regras de negócio em isolamento.
- **RNF08.4** — Os dados de teste devem ser gerados com **Bogus** (localidade `pt_BR`) para produzir dados realistas.
- **RNF08.5** — Os mocks de repositório devem seguir o **Builder Pattern fluente**, reutilizável entre testes (ex: `CustomerRepositoryBuilder.Instance().WithPhoneExists(true).Build()`).
- **RNF08.6** — Cada cenário de validação deve ter o seu próprio teste dedicado (uma falha por teste, sem agrupar múltiplas asserções sobre campos diferentes).
- **RNF08.7** — O sistema deve suportar recolha de **cobertura de código** via `coverlet`.

---

### RNF09 — Containerização

- **RNF09.1** — A API e o Frontend devem ter **Dockerfiles** com build multi-stage: imagem SDK para compilação, imagem `aspnet:7.0` runtime para execução (~200 MB).
- **RNF09.2** — O **docker-compose** deve orquestrar 3 serviços: `sqlserver`, `api` e `web`, em rede isolada `goodhamburger-net`.
- **RNF09.3** — O serviço `api` deve depender do `sqlserver` com condição `service_healthy`, garantindo que a base de dados está pronta antes de arrancar.
- **RNF09.4** — O SQL Server deve ter um **health check** configurado (query `SELECT 1` via `sqlcmd`, intervalo 15 s, timeout 10 s, 5 retentativas, período inicial 30 s).
- **RNF09.5** — Os dados do SQL Server devem ser persistidos num **volume nomeado** (`sqlserver_data`), sobrevivendo a reinícios de containers.
- **RNF09.6** — A porta interna dos containers da aplicação deve ser **8080** (`ASPNETCORE_URLS=http://+:8080`); as portas externas devem ser `5000` (API) e `5001` (Web).
- **RNF09.7** — O `docker-compose` deve injetar a connection string e a variável `InMemoryDataBase=false` via variáveis de ambiente, sem alterar ficheiros de configuração.

---

### RNF10 — Configuração e Ambiente

- **RNF10.1** — A aplicação deve suportar múltiplos ambientes (`Development`, `Production`) via `ASPNETCORE_ENVIRONMENT`.
- **RNF10.2** — O Swagger UI deve estar disponível **apenas em ambiente de Development**.
- **RNF10.3** — A flag `Configurations:InMemoryDataBase` deve controlar o modo de persistência: `true` usa EF Core InMemory (sem necessidade de servidor de base de dados), `false` usa SQL Server.
- **RNF10.4** — As migrations devem ser **ignoradas** quando o modo InMemory está ativo (`context.Database.IsRelational()` como guard).
- **RNF10.5** — As configurações de connection string devem suportar override completo via variáveis de ambiente no formato `ConnectionStrings__SQLServer` (sintaxe Docker/Kubernetes).

---

## Modelo de Dados

```
EntityBase
├── Id          : Guid       (PK, auto-gerado)
├── CreatedAt   : DateTime   (UTC, imutável após criação)
└── UpdatedAt   : DateTime   (UTC, atualizado em cada alteração)

Customer  [Tabela: Customer]
├── FirstName   : string     (max 100, required)
├── LastName    : string     (max 200)
├── Phone       : string     (max 50, unique)
├── Email       : string     (max 200)
├── Address     : string     (max 200)
└── Orders      : Order[]    (1:N)

Menu  [Tabela: Menus]
├── Name        : string     (max 100, required, unique)
├── Description : string     (max 500)
├── Price       : decimal    (18,2, required)
├── Currency    : string     (10 — "USD" | "BRL")
└── Status      : string     (20 — "Available" | "Unavailable")

SideDishes  [Tabela: SideDishes]
├── Name        : string     (max 100, required, unique)
├── Description : string     (max 500)
├── Price       : decimal    (18,2, required)
├── Category    : string     (20 — "FRIES" | "DRINK")
├── Currency    : string     (10)
└── Status      : string     (20)

Order  [Tabela: Orders]
├── CustomerID  : Guid       (FK → Customer, Restrict)
├── OrderNumber : int        (required, unique index)
├── Subtotal    : decimal    (18,2, calculado)
├── Discount    : decimal    (5,2 — percentagem ex: 20.00)
├── Total       : decimal    (18,2, calculado)
├── Status      : string     (20 — "PENDING" | "CONFIRMED" | ... )
└── OrderItems  : OrderItem[] (1:N, cascade delete)

OrderItem  [Tabela: OrderItems]
├── OrderId         : Guid   (FK → Order, cascade)
├── MenuId          : Guid   (FK → Menu, Restrict)
├── Qtd             : int    (default: 1)
├── UnitPrice       : decimal (18,2, >= 0)
└── OrderSideDishes : OrderSideDishes[] (1:N, cascade delete)

OrderSideDishes  [Tabela: OrderSideDishes]
├── OrderItemId  : Guid      (FK → OrderItem, cascade)
├── SideDishesId : Guid      (FK → SideDishes, index)
├── Category     : string    (20 — "FRIES" | "DRINK")
├── Qtd          : int       (default: 1)
└── UnitPrice    : decimal   (18,2, >= 0)
```

---

## Mapeamento de Exceções para HTTP

| Exceção | Camada | HTTP Status | Título |
|---------|--------|:-----------:|--------|
| `ValidationException` (FluentValidation) | API Filter | 400 | Erro de validação |
| `UnauthorizedAccessException` | — | 401 | Acesso não autorizado |
| `NotFoundException` | Application | 404 | Recurso não encontrado |
| `ResourceAlreadyExists` | Application | 409 | Recurso já existe |
| `DomainException` | Domain | 422 | Regra de domínio violada |
| `BusinessRuleException` | Application | 422 | Regra de negócio violada |
| Qualquer outra exceção | — | 500 | Erro interno do servidor |
