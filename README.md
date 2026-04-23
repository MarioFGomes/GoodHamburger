# GoodHamburger API

[![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)]()
[![Tests](https://img.shields.io/badge/Tests-103%20passing-brightgreen)]()
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

REST API para gestão de uma hamburgueria — pedidos, menu, acompanhamentos e clientes — construída com **Clean Architecture**, **Domain-Driven Design** e **TDD** em **.NET 7.0**.

---

## Índice

- [Visão Geral](#visão-geral)
- [Decisões de Arquitetura](#decisões-de-arquitetura)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Domínio e Regras de Negócio](#domínio-e-regras-de-negócio)
- [Endpoints](#endpoints)
- [Stack Tecnológica](#stack-tecnológica)
- [Padrões e Princípios](#padrões-e-princípios)
- [Testes](#testes)
- [Como Executar](#como-executar)
- [O que ficou de fora](#o-que-ficou-de-fora)

---

## Visão Geral

O GoodHamburger é uma API que permite gerir o ciclo completo de uma hamburgueria digital:

- Cadastro e gestão de **clientes**
- Criação e manutenção do **menu** de sanduíches
- Gestão de **acompanhamentos** (batata frita e bebida)
- Criação e gestão de **pedidos** com regras de desconto automáticas

A API foi desenhada com foco em **separação de responsabilidades**, **testabilidade** e **domínio expressivo** — as regras de negócio vivem nas entidades de domínio, não em controllers ou services genéricos.

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

Todos os repositórios implementam um `IBaseRepository<T>` genérico com operações comuns (Get, Add, Replace, Delete, Count, Any). Repositórios específicos (`IOrderRepository`) estendem com queries próprias (`GetWithItemsAsync`, `GetAllWithItemsAsync`).

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
│   │   │   │   │   └── Seeds/          # Dados de seed
│   │   │   │   └── Bootstrapper.cs
│   │   │   │
│   │   │   └── GoodHamburger.API/
│   │   │       ├── Controllers/        # 4 controllers (Customer, Menu, Order, SideDishes)
│   │   │       ├── Filters/            # ValidationFilter
│   │   │       ├── Middleware/         # GlobalExceptionHandler
│   │   │       └── Configuration/      # ApiBootstrapper (Swagger, CORS, Versioning)
│   │   │
│   │   └── test/
│   │       ├── UseCaseTest/            # Testes unitários de use cases (51 testes)
│   │       ├── Validators/             # Testes unitários de validators (52 testes)
│   │       └── Utils/                  # Builders e mocks reutilizáveis
│   │
│   └── web/                            # Frontend Blazor (em desenvolvimento)
│
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
| **SQL Server** | Produção |
| **InMemory (EF Core)** | Desenvolvimento e testes |

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
| `UseCaseTest` | **51** | Unitários — Use Cases com mocks |
| `Validators` | **52** | Unitários — Validators com dados reais |
| **Total** | **103** | — |

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

# Apenas Validators
dotnet test apps/api/test/validators/customer/Validators/Validators.csproj

# Apenas Use Cases
dotnet test apps/api/test/UseCases/UseCaseTest/UseCaseTest.csproj
```

---

## Como Executar

### Pré-requisitos

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- SQL Server (opcional — por defeito usa InMemory)

### Configuração

```json
// appsettings.json
{
  "ConnectionStrings": {
    "SQLServer": "Server=localhost;Database=GoodHamburger;Trusted_Connection=True;"
  },
  "Configurations": {
    "InMemoryDataBase": true
  }
}
```

Para usar SQL Server, altere `InMemoryDataBase` para `false` e aplique as migrations:

```bash
dotnet ef database update \
  --project GoodHamburger/apps/api/src/GoodHamburger.Infrastructure/GoodHamburger.Infrastructure.csproj \
  --startup-project GoodHamburger/apps/api/src/GoodHamburger.API/GoodHamburger.API.csproj
```

### Executar

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

---

## O que ficou de fora

Estas funcionalidades foram conscientemente deixadas de fora do âmbito atual do projeto:

| Funcionalidade | Motivo |
|---------------|--------|
| **Autenticação / Autorização** | Fora do âmbito da fase atual. A estrutura de middleware está preparada para adicionar JWT sem alterar use cases. |
| **Frontend Blazor** | Template inicial criado mas não integrado com a API. Interface planeada para fase posterior. |
| **Testes de Integração** | Apenas testes unitários existem. Testes de integração contra InMemory DB são o próximo passo natural. |
| **Testes de Domínio** | As regras de negócio das entidades (Order, OrderItem) não têm testes unitários diretos — são cobertas indiretamente pelos use case tests. |
| **PAID / READY / DELIVERED** | Os estados do ciclo de vida do pedido existem no enum e no domínio, mas não há use cases específicos para eles (ex: `MarkAsReadyUseCase`). |
| **Paginação por filtro** | As listagens paginadas não suportam filtros ou ordenação — retornam todos os registos paginados. |
| **Soft Delete** | A infraestrutura tem suporte parcial (`IgnoreQueryFilters`) mas não está activado. Eliminação é física. |
| **Rate Limiting** | Não implementado. Pode ser adicionado via middleware do ASP.NET Core sem impacto na arquitetura. |
| **Health Checks** | Não implementados. |
| **CI/CD** | Sem pipeline configurado. Os testes podem ser integrados facilmente em GitHub Actions ou Azure DevOps. |
