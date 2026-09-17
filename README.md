# cochief
An open-source virtual Co-Leader for Clash of Clans that helps clan leaders monitor activity, analyze performance and make better management decisions.

## OpenAPI

Generate the OpenAPI 3.0 specification on demand:

```powershell
dotnet run --project src/Cochief.Api -- openapi
```

The command writes `openapi/cochief-api.json` without starting the API or exposing a documentation endpoint.

## Unit tests

Run the xUnit test suite from the repository root:

```powershell
dotnet test cochief.slnx
```

The application service tests use AutoFixture with AutoMoq to create and inject mocks for persistence, external APIs and token generation, plus a controlled clock, so they do not require PostgreSQL or Clash of Clans credentials.

Run the application integration tests with Docker available:

```powershell
dotnet test tests/Cochief.Application.IntegrationTests/Cochief.Application.IntegrationTests.csproj
```

The integration suite starts an isolated PostgreSQL 17 container, recreates and seeds its database before every test, and exercises every API controller endpoint through the real HTTP, persistence, authentication and security pipeline.

## Database migrations

Restore the repository-local EF Core tool and create a migration after changing the persistence model:

```powershell
dotnet tool restore
dotnet ef migrations add MigrationName --project src/Cochief.Infrastructure --startup-project src/Cochief.Api --context CochiefDbContext --output-dir Persistence/Migrations
```

Apply pending migrations to a local database:

```powershell
dotnet ef database update --project src/Cochief.Infrastructure --startup-project src/Cochief.Api --context CochiefDbContext --connection "<connection-string>"
```

Generate a reviewable, idempotent SQL script for a production deployment:

```powershell
dotnet ef migrations script --idempotent --project src/Cochief.Infrastructure --startup-project src/Cochief.Api --context CochiefDbContext --output artifacts/database-migrations.sql
```

Apply that script as a separate deployment step before starting the API. The API process does not create or migrate the database schema at startup.

Databases previously created with `EnsureCreated` have no migration history. Recreate disposable development databases before applying `InitialCreate`; databases containing data require a reviewed baseline procedure instead.
