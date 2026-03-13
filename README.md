# TrailerTrack

TrailerTrack is a fleet management system built with .NET 10, Blazor Server, and Clean Architecture. I built this project to deepen my understanding of the .NET ecosystem and its tooling, while producing something practical and relevant to the kind of internal operations tools that companies like HenCo Group use day-to-day.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | Blazor Server (.NET 10) |
| Styling | Tailwind CSS (CDN) |
| Backend | ASP.NET Core (.NET 10) |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core |
| CQRS | MediatR |
| Validation | FluentValidation |
| Authentication | ASP.NET Core Identity |
| Testing | xUnit, Moq, FluentAssertions |
| Containerisation | Docker + Docker Compose |

## Architecture

TrailerTrack is structured using Clean Architecture, separating concerns into four distinct layers:
```
TrailerTrack.Domain          → Entities, enums, repository interfaces. Zero dependencies.
TrailerTrack.Application     → CQRS commands/queries, validators, DTOs. Depends on Domain only.
TrailerTrack.Infrastructure  → EF Core, PostgreSQL, Identity, repository implementations.
TrailerTrack.Web             → Blazor Server UI, Razor Pages for auth, service implementations.
TrailerTrack.Tests           → xUnit unit tests for Application layer.
```

**Key patterns used:**

- **CQRS** — commands and queries are separated using MediatR. Each operation has its own handler, validator, and request record.
- **Result Pattern** — all handlers return `Result` or `Result<T>` instead of throwing exceptions, making error handling explicit and predictable.
- **Repository Pattern** — domain layer defines repository interfaces, infrastructure implements them. Handlers never touch EF Core directly.
- **Clean Architecture** — inner layers have zero knowledge of outer layers. The Application layer defines `ICurrentUserService` which the Web layer implements, keeping ASP.NET Core concerns out of business logic.

## Features

- **Asset Management** — create and manage trailer fleet assets with codes, types, locations and statuses
- **Hire Lifecycle** — hire out assets to customers and process returns with full event history
- **Maintenance Logging** — start and complete maintenance jobs with descriptions, dates and costs
- **Asset Retirement** — permanently retire assets from the fleet
- **Role Based Access Control** — Admin and Staff roles with UI and handler level enforcement
- **Column Filters** — filter assets by type, status and location inline in the asset table
- **Audit History** — full hire event and maintenance log history per asset

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16](https://www.postgresql.org/download/) (for local development)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Docker)

### Running Locally

1. Clone the repository:
```bash
git clone https://github.com/yourusername/TrailerTrack.git
cd TrailerTrack
```

2. Update the connection string in `TrailerTrack.Web/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=trailertrack;Username=postgres;Password=yourpassword"
}
```

3. Apply migrations and run:
```bash
dotnet watch --project TrailerTrack.Web
```

Migrations and seeding run automatically on startup. The following accounts are seeded:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@trailertrack.com | Admin1234! |
| Staff | staff@trailertrack.com | Staff1234! |

### Running with Docker

1. Clone the repository:
```bash
git clone https://github.com/yourusername/TrailerTrack.git
cd TrailerTrack
```

2. Build and run:
```bash
docker-compose up --build
```

3. Visit `http://localhost:8080`

The database is created, migrations applied, and seed data inserted automatically on first run.

To stop:
```bash
docker-compose down
```

To stop and wipe the database:
```bash
docker-compose down -v
```

## Testing

Unit tests cover the Application layer — command handlers and validators.

Run all tests:
```bash
dotnet test
```

**Test coverage includes:**

- All command handlers — happy path, not found, invalid status, unauthorised, validation failure
- Validator tests for complex rules — date ordering, numeric boundaries, required fields
- Mocked dependencies using Moq — no database required for unit tests

**Patterns used:**

- Arrange/Act/Assert
- Moq for dependency mocking
- FluentAssertions for readable assertions
- `[Theory]` with `[InlineData]` for boundary and input variation tests
