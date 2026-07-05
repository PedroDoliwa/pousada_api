# Pousada API

API REST para gestão de pousadas: reservas, quartos, hóspedes, calendários e métricas. Desenvolvida em **ASP.NET Core 8** com **PostgreSQL**.

## O que faz

A API permite que um gerente cadastre e administre uma ou mais pousadas. Cada pousada possui quartos, hóspedes e reservas. Também oferece:

- **Autenticação JWT** — registro, login e recuperação de senha por e-mail
- **Disponibilidade** — verificação de conflitos e visualização de ocupação por período
- **Calendários externos** — importação de reservas via iCal (Airbnb, Booking, etc.) e exportação do calendário de cada quarto
- **Métricas** — indicadores de desempenho da pousada em um intervalo de datas
- **Consulta inteligente** — perguntas em linguagem natural sobre reservas, quartos e hóspedes (OpenAI)

## Estrutura do projeto

```
src/
├── Api/              → Controllers, middlewares e ponto de entrada (Program.cs)
├── Application/      → Serviços, DTOs e interfaces de negócio
├── Domain/           → Entidades e contratos de repositório
└── Infrastructure/   → Banco de dados (EF Core), JWT, integrações (OpenAI, iCal, e-mail)
```

A organização segue **Clean Architecture**: a camada `Api` recebe as requisições, `Application` contém a lógica de negócio, `Domain` define as entidades e `Infrastructure` implementa persistência e serviços externos.

## Principais recursos

| Recurso | Rota base | Descrição |
|---|---|---|
| Auth | `/api/auth` | Registro, login, esqueci/redefinir senha |
| Usuário | `/api/usuario` | Perfil, senha e foto do usuário logado |
| Pousadas | `/api/pousadas` | CRUD de pousadas e foto |
| Quartos | `/api/quartos` | CRUD de quartos e exportação iCal |
| Hóspedes | `/api/hospedes` | CRUD de hóspedes |
| Reservas | `/api/reservas` | CRUD, ocupação e verificação de disponibilidade |
| Calendários | `/api/calendarios` | Calendários externos e sincronização |
| Métricas | `/api/metricas` | Indicadores por pousada e período |
| Consulta | `/api/consulta` | Perguntas em linguagem natural (requer autenticação) |

Rotas protegidas exigem o header `Authorization: Bearer {token}`.

## Como funciona

1. O usuário se registra ou faz login em `/api/auth` e recebe um **token JWT**.
2. Com o token, acessa os endpoints de gestão (pousadas, quartos, reservas, etc.).
3. Cada usuário gerencia apenas suas próprias pousadas e dados vinculados a elas.
4. Ao criar uma reserva, a API valida se o quarto está disponível no período informado.
5. Calendários externos são sincronizados periodicamente (ou manualmente) e importam reservas de plataformas como Airbnb.
6. A consulta inteligente envia a pergunta à OpenAI, que consulta os dados reais da pousada via ferramentas internas e devolve a resposta.

## Como rodar

**Pré-requisitos:** .NET 8 SDK e PostgreSQL.

1. Clone o repositório e crie um arquivo `.env` na raiz com as variáveis necessárias:

```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=pousada_db;Username=postgres;Password=postgres
Jwt__SecretKey=sua-chave-secreta-com-pelo-menos-32-caracteres
OPENAI_API_KEY=sua-chave-openai
RESEND_API_KEY=sua-chave-resend
App__FrontendBaseUrl=http://localhost:3000
```

2. Execute a API:

```bash
dotnet run --project src/Api/PousadaApi.csproj
```

Em ambiente de desenvolvimento, as migrations do banco são aplicadas automaticamente na inicialização.

3. Acesse o Swagger em `http://localhost:5186/swagger` (a porta pode variar conforme o `.env`).

## Docker

```bash
docker build -t pousada-api .
docker run -p 8080:8080 --env-file .env pousada-api
```

A API sobe na porta **8080** dentro do container.
