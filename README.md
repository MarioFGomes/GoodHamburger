## GoodHamburger — Entendimento Geral


Arquitetura: Clean Architecture + DDD, 4 camadas (Domain → Application → Infrastructure → API), .NET 7.0.

O que está completo:

Domain model rico: Order com regras de negócio (AddSandwich, Confirm, Cancel, descontos 10/15/20%)
Todos os repositórios (6 entidades)
CRUD completo de Customer (use cases + endpoints)
GlobalExceptionHandler, ValidationFilter, Swagger com versionamento
Bugs conhecidos:

GetCustomerByIdUseCase.cs — null check invertido (is not null devia ser is null) → GET por ID sempre retorna 404
O que está em falta:

Use cases e endpoints para Order, Menu, SideDishes (domain pronto, application/API não)
Autenticação (registada mas vazia)
Frontend Blazor (template vazio, não integrado com API)
Migrations do EF Core
Testes (nenhum projeto de testes)
