# GoodHamburger

[![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)]()
[![Tests](https://img.shields.io/badge/Tests-141%20passing-brightgreen)]()
[![MudBlazor](https://img.shields.io/badge/UI-MudBlazor%206-594AE2?logo=blazor)](https://mudblazor.com/)
[![CI](https://img.shields.io/badge/CI-GitHub%20Actions-2088FF?logo=githubactions)](https://github.com/features/actions)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Aplicação full-stack de gestão de hamburgueria — pedidos, menu, acompanhamentos e clientes — construída com **Clean Architecture**, **Domain-Driven Design** e **TDD** em **.NET 7.0**, com interface **Blazor Server + MudBlazor**.

---

## Índice

- [Visão Geral](#visão-geral)
- [Decisões de Arquitetura](#decisões-de-arquitetura)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Domínio e Regras de Negócio](#domínio-e-regras-de-negócio)
- [Endpoints](#endpoints)
- [Stack Tecnológica](#stack-tecnológica)
- [Frontend Blazor](#frontend-blazor)
- [Padrões e Princípios](#padrões-e-princípios)
- [Testes](#testes)
- [Como Executar](#como-executar)
- [CI/CD](#cicd)
- [O que ficou de fora](#o-que-ficou-de-fora)

---

## Visão Geral

O GoodHamburger é uma aplicação full-stack que permite gerir o ciclo completo de uma hamburgueria digital:

- Cadastro e gestão de **clientes**
- Criação e manutenção do **menu** de sanduíches
- Gestão de **acompanhamentos** (batata frita e bebida)
- Criação e gestão de **pedidos** com regras de desconto automáticas

A API foi desenhada com foco em **separação de responsabilidades**, **testabilidade** e **domínio expressivo** — as regras de negócio vivem nas entidades de domínio, não em controllers ou services genéricos. O frontend em Blazor Server consome a API via HTTP e expõe uma interface Material Design completa para todas as operações CRUD.

---

## Decisões de Arquitetura

### Clean Architecture + Use Case Pattern

A solução está organizada em 4 camadas com dependências unidirecionais:

```
GoodHamburger.API  ──→  GoodHamburger.Application  ──→  GoodHamburger.Domain
                                                   ↗
                    GoodHamburger.Infrastructure  ─────→  GoodHamburger.Domain
```

| Camada | Responsabilidade |
|--------|-----------------|
| **Domain** | Entidades, interfaces de repositório, exceções de domínio, enums. Zero dependências externas. |
| **Application** | Use Cases, DTOs, Validators, Mappers, exceções de aplicação. Orquestra o domínio sem conhecer infraestrutura. |
| **Infrastructure** | Implementações de repositórios com EF Core, DbContext, Seeds, Unit of Work. |
| **API** | Controllers, Action Filters, Middleware global de exceções, configuração de Swagger e versionamento. |

**Por que Use Case Pattern e não Service Pattern?**
Cada operação tem a sua própria classe (`CreateOrderUseCase`, `ConfirmOrderUseCase`...). Isso garante que cada classe tem uma única razão para mudar (SRP) e evita a tendência natural de *services* crescerem indefinidamente com métodos acumulados.

### Domínio Rico (não anémico)

As entidades de domínio **não são apenas DTOs com propriedades**. A entidade `Order`, por exemplo, encapsula toda a lógica de negócio:

```csharp
order.AddSandwich(menuId, price);
order.AddSideDish(sideDishId, SideDishCategory.FRIES, price);
order.Confirm();
order.Cancel();
```

Os cálculos de desconto, as validações de transição de estado e as regras de composição de pedido vivem **dentro da entidade**, protegidas por encapsulamento (`private set`, métodos de domínio).

### Repository Pattern + Unit of Work

Todos os repositórios implementam um `IBaseRepository<T>` genérico com operações comuns (Get, Add, Replace, Delete, Count, Any). Repositórios específicos (`IOrderRepository`) estendem com queries próprias (`GetWithItemsAsync`, `GetAllWithItemsAsync`, `NextOrderNumberAsync`).

O `IUnitOfWork` abstrai transações — nas operações críticas (criação e eliminação de pedidos) usa-se `BeginTransaction → Commit / Rollback`. O padrão suporta tanto SQL Server como InMemory (para testes e desenvolvimento).

### Validação Centralizada com Action Filter

O `ValidationFilter` intercepta cada request e executa o `AbstractValidator<T>` correspondente **antes** de chegar ao use case. O resultado é um `ValidationProblemDetails` (RFC 7807) com erros por campo. Os Use Cases não precisam de validar inputs — confiam que chegam dados já validados.

### Tratamento Global de Exceções

O `GlobalExceptionHandler` mapeia cada tipo de exceção de domínio/aplicação para o status HTTP correto:

| Exceção | HTTP Status |
|---------|-------------|
| `NotFoundException` | 404 Not Found |
| `ResourceAlreadyExists` | 409 Conflict |
| `DomainException` / `BusinessRuleException` | 422 Unprocessable Entity |
| `ValidationException` (FluentValidation) | 400 Bad Request |
| Qualquer outra | 500 Internal Server Error |

### Versionamento de API

A API usa `Asp.Versioning` com versão por URL (`/api/v1/...`). O Swagger expõe documentação separada por versão. Preparado para adicionar `v2` sem quebrar clientes existentes.

---

## Estrutura de Pastas

```
GoodHamburger/
├── apps/
│   ├── api/
│   │   ├── src/
│   │   │   ├── GoodHamburger.Domain/
│   │   │   │   ├── Entities/          # Order, Customer, Menu, SideDishes, OrderItem, OrderSideDishes
│   │   │   │   ├── Repositories/      # IBaseRepository<T>, IOrderRepository, ICustomerRepository...
│   │   │   │   ├── Exceptions/        # DomainException
│   │   │   │   └── Enum/              # OrderStatus, SideDishCategory, Currency, MenuStatus
│   │   │   │
│   │   │   ├── GoodHamburger.Application/
│   │   │   │   ├── UseCases/          # 21 Use Cases organizados por entidade
│   │   │   │   ├── DTOs/              # Requests e Responses
│   │   │   │   ├── Validators/        # FluentValidation validators (auto-registados)
│   │   │   │   ├── Mappers/           # Extension methods de conversão Domain ↔ DTO
│   │   │   │   ├── Exceptions/        # NotFoundException, ResourceAlreadyExists, BusinessRuleException
│   │   │   │   └── Bootstrapper.cs    # Registo de DI (use cases + validators)
│   │   │   │
│   │   │   ├── GoodHamburger.Infrastructure/
│   │   │   │   ├── DataAcess/
│   │   │   │   │   ├── GoodHamburgerContext.cs
│   │   │   │   │   ├── Configurations/ # Fluent API do EF Core por entidade
│   │   │   │   │   ├── Repositories/   # Implementações concretas
│   │   │   │   │   ├── Seeds/          # SeedData.cs (GUIDs fixos, chamado em OnModelCreating)
│   │   │   │   │   └── Migrations/     # InitialCreate + SeedInitialData
│   │   │   │   └── Bootstrapper.cs
│   │   │   │
│   │   │   └── GoodHamburger.API/
│   │   │       ├── Controllers/        # 4 controllers (Customer, Menu, Order, SideDishes)
│   │   │       ├── Filters/            # ValidationFilter
│   │   │       ├── Middleware/         # GlobalExceptionHandler
│   │   │       └── Configuration/      # ApiBootstrapper (Swagger, CORS, Versioning)
│   │   │
│   │   └── test/
│   │       ├── DomainTest/             # 38 testes unitários de entidades de domínio
│   │       ├── UseCaseTest/            # 51 testes unitários de use cases
│   │       ├── Validators/             # 52 testes unitários de validators
│   │       └── Utils/                  # Builders e mocks reutilizáveis
│   │
│   └── web/
│       └── src/
│           └── WebGoodHamburger/
│               ├── Pages/
│               │   ├── Index.razor             # Dashboard
│               │   ├── Customers/              # List, Create, Edit
│               │   ├── Menus/                  # List, Create, Edit
│               │   ├── SideDishes/             # List, Create, Edit
│               │   └── Orders/                 # List, Create, Detail
│               ├── Shared/
│               │   ├── MainLayout.razor        # MudLayout + tema personalizado
│               │   ├── NavMenu.razor           # MudNavMenu
│               │   └── Pagination.razor        # MudPagination
│               ├── Services/                   # HTTP clients para a API
│               │   └── ApiErrorParser.cs       # Extrai "detail" do ProblemDetails JSON
│               ├── Models/                     # DTOs do lado web (espelham respostas da API)
│               └── Program.cs
│
├── Dockerfile.api
├── Dockerfile.web
├── docker-compose.yml
├── GoodHamburger.postman_collection.json
└── GoodHamburger.sln
```

---

## Domínio e Regras de Negócio

### Entidades

```
EntityBase (Id: Guid, CreatedAt, UpdatedAt)
├── Customer       (FirstName, LastName, Email, Phone*, Address)
├── Menu           (Name*, Description, Price, Currency, Status)
├── SideDishes     (Name*, Description, Price, Category, Currency, Status)
└── Order          (CustomerID, OrderNumber, Subtotal, Discount, Total, Status)
    └── OrderItem  (MenuId, Qtd, UnitPrice)
        └── OrderSideDishes (SideDishesId, Category, Qtd, UnitPrice)

* campo único por regra de negócio (validado na Application)
```

### Regras de Desconto (Order)

O desconto é calculado automaticamente com base nos acompanhamentos do pedido:

| Acompanhamentos | Desconto |
|-----------------|---------|
| Batata frita + Bebida (combo) | **20%** |
| Apenas Bebida | **15%** |
| Apenas Batata Frita | **10%** |
| Nenhum | 0% |

### Transições de Estado do Pedido

```
PENDING ──→ CONFIRMED ──→ PAID ──→ READY ──→ DELIVERED
   │
   └──→ CANCELLED (de qualquer estado exceto DELIVERED)
```

### Invariantes de Domínio

- Um pedido só pode ter **um sanduíche** (sem duplicatas)
- Um pedido só pode ter **um acompanhamento por categoria** (1 FRIES + 1 DRINK no máximo)
- Não é possível adicionar acompanhamentos antes de adicionar um sanduíche
- Pedidos entregues (DELIVERED) não podem ser cancelados
- Só pedidos PENDING ou CANCELLED podem ser eliminados
- Preços não podem ser negativos

---

## Endpoints

Base URL: `/api/v1`

### Customers

| Método | Rota | Descrição | Resposta |
|--------|------|-----------|---------|
| `POST` | `/customers` | Criar cliente | 201 CustomerResponse |
| `GET` | `/customers` | Listar (paginado) | 200 PagedResponse\<CustomerResponse\> |
| `GET` | `/customers/{id}` | Obter por ID | 200 CustomerResponse |
| `PUT` | `/customers/{id}` | Atualizar | 200 CustomerResponse |
| `DELETE` | `/customers/{id}` | Eliminar | 204 |

### Menus

| Método | Rota | Descrição | Resposta |
|--------|------|-----------|---------|
| `POST` | `/menus` | Criar item de menu | 201 MenuResponse |
| `GET` | `/menus` | Listar (paginado) | 200 PagedResponse\<MenuResponse\> |
| `GET` | `/menus/{id}` | Obter por ID | 200 MenuResponse |
| `PUT` | `/menus/{id}` | Atualizar | 200 MenuResponse |
| `DELETE` | `/menus/{id}` | Eliminar | 204 |

### Side Dishes

| Método | Rota | Descrição | Resposta |
|--------|------|-----------|---------|
| `POST` | `/sidedishes` | Criar acompanhamento | 201 SideDishesResponse |
| `GET` | `/sidedishes` | Listar (paginado) | 200 PagedResponse\<SideDishesResponse\> |
| `GET` | `/sidedishes/{id}` | Obter por ID | 200 SideDishesResponse |
| `PUT` | `/sidedishes/{id}` | Atualizar | 200 SideDishesResponse |
| `DELETE` | `/sidedishes/{id}` | Eliminar | 204 |

### Orders

| Método | Rota | Descrição | Resposta |
|--------|------|-----------|---------|
| `POST` | `/orders` | Criar pedido | 201 OrderResponse |
| `GET` | `/orders` | Listar (paginado) | 200 PagedResponse\<OrderResponse\> |
| `GET` | `/orders/{id}` | Obter por ID | 200 OrderResponse |
| `PUT` | `/orders/{id}/confirm` | Confirmar pedido | 200 OrderResponse |
| `PUT` | `/orders/{id}/cancel` | Cancelar pedido | 200 OrderResponse |
| `DELETE` | `/orders/{id}` | Eliminar pedido | 204 |

### Respostas de Erro

| Status | Quando ocorre |
|--------|--------------|
| 400 | Validação de input falhou (campo inválido, formato errado) |
| 404 | Recurso não encontrado |
| 409 | Conflito — recurso já existe (phone duplicado, nome duplicado) |
| 422 | Violação de regra de negócio (ex: confirmar pedido vazio) |
| 500 | Erro interno inesperado |

---

## Stack Tecnológica

### Backend

| Tecnologia | Versão | Utilização |
|-----------|--------|-----------|
| **.NET** | 7.0 | Plataforma principal |
| **ASP.NET Core** | 7.0 | Web API |
| **Entity Framework Core** | 7.0.20 | ORM — SQL Server e InMemory |
| **FluentValidation** | 11.9.0 | Validação de DTOs |
| **Asp.Versioning** | 7.1.1 | Versionamento de API |
| **Swashbuckle** | 6.5.0 | Documentação OpenAPI/Swagger |

### Frontend

| Tecnologia | Versão | Utilização |
|-----------|--------|-----------|
| **Blazor Server** | .NET 7.0 | Framework frontend server-side |
| **MudBlazor** | 6.21.0 | Componentes Material Design |

### Testes

| Tecnologia | Versão | Utilização |
|-----------|--------|-----------|
| **xUnit** | 2.4.2 | Framework de testes |
| **FluentAssertions** | 8.9.0 | Assertions expressivas |
| **Moq** | 4.20.72 | Mocking de dependências |
| **Bogus** | 35.6.5 | Geração de dados fake (pt_BR) |
| **coverlet** | 3.2.0 | Cobertura de código |

### Bases de Dados

| Modo | Utilização |
|------|-----------|
| **SQL Server** | Produção / Docker |
| **InMemory (EF Core)** | Desenvolvimento local e testes |

---

## Frontend Blazor

O frontend é uma aplicação **Blazor Server** que consome a API REST e oferece uma interface Material Design completa.

### Tema e Design

- **AppBar e Drawer**: fundo escuro `#181512` (charcoal)
- **Primary**: amber `#feae2c` com texto escuro — botões de ação, paginação, chips ativos
- **Layout**: `MudDrawer` responsivo (colapsa em mobile), `MudAppBar` fixo no topo
- **Notificações**: `ISnackbar` (MudBlazor) para erros e confirmações — toasts automáticos
- **Confirmação de delete**: `IDialogService.ShowMessageBox` — diálogo nativo do MudBlazor

### Páginas

| Página | Descrição |
|--------|-----------|
| **Dashboard** | 4 cards com contagem de Customers, Menus, Side Dishes e Orders |
| **Customers** | Lista (MudTable), criar, editar |
| **Menus** | Lista com badge de status (Available/Unavailable), criar, editar |
| **Side Dishes** | Lista com badge de categoria (FRIES/DRINK) e status, criar, editar |
| **Orders** | Lista com status colorido (PENDING/CONFIRMED/CANCELLED/...) e total |
| **New Order** | Seleção de cliente e sanduíche, escolha de acompanhamentos com regras de desconto visíveis |
| **Order Detail** | Header com ações (Confirm/Cancel), métricas (Subtotal/Desconto/Total), itens com acompanhamentos |

### Comunicação com a API

Os serviços HTTP (`CustomerService`, `MenuService`, `SideDishService`, `OrderService`) usam um `HttpClient` com nome configurado:

```csharp
// Program.cs
builder.Services.AddHttpClient("GoodHamburgerApi", client => {
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
});
```

Erros HTTP são tratados via `ApiErrorParser.Extract()` que extrai apenas o campo `detail` do JSON ProblemDetails, evitando mostrar stack traces ao utilizador.

---

## Padrões e Princípios

### SOLID

| Princípio | Como foi aplicado |
|-----------|------------------|
| **SRP** | Cada Use Case tem uma única responsabilidade. Controllers apenas recebem e delegam. |
| **OCP** | Novos Use Cases não modificam os existentes — basta registar no Bootstrapper. |
| **LSP** | `IBaseRepository<T>` pode ser substituído por qualquer implementação concreta. |
| **ISP** | `IOrderRepository` estende `IBaseRepository<Order>` apenas com o necessário. |
| **DIP** | Use Cases dependem de interfaces (ICustomerRepository), não de implementações concretas. |

### Design Patterns

- **Use Case Pattern** — uma classe por operação de negócio
- **Repository Pattern** — abstração de acesso a dados
- **Unit of Work** — gestão de transações
- **Builder Pattern** — nos testes, para construção fluente de entidades e mocks
- **Extension Methods (Mapper)** — conversão fluente entre Domain e DTOs
- **Chain of Responsibility** — middleware pipeline do ASP.NET Core
- **Factory Method** — criação de entidades via construtores de domínio

### Metodologias

- **TDD** — testes escritos para validar cada comportamento dos Use Cases e Validators
- **DDD** — entidades com comportamento, não apenas dados
- **Clean Architecture** — dependências apontam sempre para dentro
- **Conventional Commits** — commits organizados por tipo (feature, bug-fix, hotfix)

---

## Testes

### Cobertura

| Projeto | Testes | Tipo |
|---------|--------|------|
| `DomainTest` | **38** | Unitários — regras de negócio das entidades (Order, OrderItem, OrderSideDishes) |
| `UseCaseTest` | **51** | Unitários — Use Cases com mocks |
| `Validators` | **52** | Unitários — Validators com dados reais |
| **Total** | **141** | — |

### Estratégia

**Validator Tests:** Cada regra de validação tem um teste de falha dedicado. Por exemplo, para `CreateCustomerRequestValidator`:
- `ValidateSuccess` — dados completamente válidos
- `ValidateFirstNameEmpty`, `ValidateLastNameEmpty` — campos obrigatórios
- `ValidatePhoneInvalidFormat` — regex de telefone
- `ValidateEmailInvalidFormat` — formato de email
- `ValidateFirstNameExceedsMaxLength`, `ValidateLastNameExceedsMaxLength` — limites de caracteres

**Use Case Tests:** Cada Use Case tem o caminho feliz e todos os caminhos de erro testados:
- Sucesso (mock retorna dados esperados)
- NotFound (repositório retorna null → `NotFoundException`)
- Conflito (repositório indica duplicado → `ResourceAlreadyExists`)
- Regra de negócio (ex: eliminar pedido CONFIRMED → `BusinessRuleException`)

Para Order, os 4 cenários de desconto são cobertos individualmente (sem acompanhamento, só FRIES, só DRINK, combo).

### Utilitários de Teste (`Utils`)

```
Utils/
├── Entities/
│   ├── CustomerBuilder    — gera Customer fake com Bogus (pt_BR)
│   ├── MenuBuilder        — gera Menu fake + ToRequest() / ToUpdateRequest()
│   ├── SideDishesBuilder  — gera SideDishes fake, CreateFries(), CreateDrink()
│   └── OrderBuilder       — Create(), CreateWithFries(), CreateWithDrink(), CreateWithCombo()
└── Repositories/
    ├── CustomerRepositoryBuilder  — mock fluente com WithCustomer(), WithPhoneExists(), ...
    ├── MenuRepositoryBuilder      — mock fluente com WithMenu(), WithNameExists(), ...
    ├── SideDishesRepositoryBuilder
    ├── OrderRepositoryBuilder     — WithOrder(), WithOrders(), WithCount()
    ├── OrderItemRepositoryBuilder
    ├── OrderSideDishesRepositoryBuilder
    └── UnitOfWorkBuilder
```

O padrão de mock é fluente:
```csharp
var repo = CustomerRepositoryBuilder.Instance()
    .WithCustomer(existingCustomer)
    .WithPhoneExists(true)
    .Build();
```

### Executar os Testes

```bash
# Todos os testes
dotnet test GoodHamburger/GoodHamburger.sln

# Com cobertura
dotnet test GoodHamburger/GoodHamburger.sln --collect:"XPlat Code Coverage"

# Apenas Domain (entidades)
dotnet test GoodHamburger/apps/api/test/DomainTest/DomainTest.csproj

# Apenas Validators
dotnet test GoodHamburger/apps/api/test/validators/customer/Validators/Validators.csproj

# Apenas Use Cases
dotnet test GoodHamburger/apps/api/test/UseCases/UseCaseTest/UseCaseTest.csproj
```

---

## Como Executar

### Pré-requisitos

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- SQL Server (opcional — por defeito usa InMemory)
- [Docker Desktop](https://www.docker.com/) (recomendado — forma mais simples de executar toda a stack)

### Executar com Docker (recomendado)

```bash
cd GoodHamburger
docker compose up --build
```

| Serviço | URL |
|---------|-----|
| **API** | `http://localhost:5000` |
| **Frontend (Blazor)** | `http://localhost:5001` |
| **Swagger UI** | `http://localhost:5000/swagger` |
| **SQL Server** | `localhost:1433` |

As migrations e os dados de seed são aplicados automaticamente no startup da API. A stack inclui health check no SQL Server — a API só arranca depois de a base de dados estar disponível.

### Executar localmente (apenas API)

```bash
# Restaurar dependências
dotnet restore GoodHamburger/GoodHamburger.sln

# Compilar
dotnet build GoodHamburger/GoodHamburger.sln

# Iniciar API
dotnet run --project GoodHamburger/apps/api/src/GoodHamburger.API
```

A API fica disponível em `https://localhost:7162`.  
Swagger UI em `https://localhost:7162/swagger`.

Por defeito, `InMemoryDataBase: true` — não é necessário SQL Server.

### Usar SQL Server

Altere `InMemoryDataBase` para `false` em `appsettings.json` e aplique as migrations:

```bash
dotnet ef database update \
  --project GoodHamburger/apps/api/src/GoodHamburger.Infrastructure/GoodHamburger.Infrastructure.csproj \
  --startup-project GoodHamburger/apps/api/src/GoodHamburger.API/GoodHamburger.API.csproj
```

### Dados de Seed

Com SQL Server ativo (InMemoryDataBase: false), a migration `SeedInitialData` popula automaticamente a base de dados com:
- 3 sanduíches (X Burger, X Bacon, X Egg)
- 2 acompanhamentos (Batata Frita, Coca-Cola)
- 2 clientes de exemplo

### Testar com Postman

Importar o ficheiro `GoodHamburger.postman_collection.json` (raiz do repositório) no Postman.  
Contém **60 requests** distribuídos por 4 pastas (Customers, Menus, Side Dishes, Orders), cobrindo todos os cenários de sucesso, validação, not found e regras de negócio.

---

## CI/CD

O projeto usa **GitHub Actions** para integração contínua.

**Ficheiro:** `.github/workflows/dotnet-desktop.yml`

### Pipeline

```
push/PR → main ou develop
    │
    ├─→ build-and-test (ubuntu-latest)
    │     ├── Setup .NET 7.0.x
    │     ├── dotnet restore
    │     ├── dotnet build --configuration Release
    │     ├── dotnet test (trx logger, upload artefactos)
    │     └── Upload test results (.trx)
    │
    └─→ docker (needs: build-and-test)
          ├── docker build Dockerfile.api
          └── docker build Dockerfile.web
```

O job `docker` valida que ambas as imagens compilam com sucesso, mas não faz push para nenhum registry.

---

## O que ficou de fora

Estas funcionalidades foram conscientemente deixadas de fora do âmbito atual do projeto:

| Funcionalidade | Motivo |
|---------------|--------|
| **Autenticação / Autorização** | Fora do âmbito da fase atual. A estrutura de middleware está preparada para adicionar JWT sem alterar use cases. |
| **Testes de Integração** | Apenas testes unitários existem. Testes de integração contra InMemory DB são o próximo passo natural. |
| **PAID / READY / DELIVERED** | Os estados do ciclo de vida do pedido existem no enum e no domínio, mas não há use cases específicos para eles (ex: `MarkAsReadyUseCase`). |
| **Paginação por filtro** | As listagens paginadas não suportam filtros ou ordenação — retornam todos os registos paginados. |
| **Soft Delete** | A infraestrutura tem suporte parcial (`IgnoreQueryFilters`) mas não está activado. Eliminação é física. |
| **Rate Limiting** | Não implementado. Pode ser adicionado via middleware do ASP.NET Core sem impacto na arquitetura. |
| **Health Checks (API)** | A API não expõe `/health`. O Docker Compose faz health check diretamente no SQL Server. |
