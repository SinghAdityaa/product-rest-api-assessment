# Product REST API – Technical Assessment

A production-minded RESTful backend API built with **.NET 8 / ASP.NET Core Web API**, **SQL Server**, and **Entity Framework Core**. It implements Products and related Items, JWT authentication with rotating refresh tokens, role-based authorization, validation, structured logging, API versioning, Swagger/OpenAPI, tests, pagination, Docker, and centralized error handling.

## High-level architecture

```text
Client / Swagger
      |
      v
ASP.NET Core API
  - API versioning
  - JWT auth + Admin policy
  - FluentValidation
  - Exception middleware
      |
      v
Application Layer
  - DTOs
  - service interfaces
  - business services
      |
      v
Infrastructure Layer
  - EF Core repositories
  - JWT / refresh-token service
  - SQL Server DbContext
      |
      v
SQL Server
  Product 1 ---- * Item
  AppUser  1 ---- * RefreshToken
```

Dependencies point inward: API -> Application/Infrastructure, Infrastructure -> Application/Domain, Application -> Domain. Domain has no infrastructure dependency.

## API endpoints

| Method | Route | Access | Purpose |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Public | Get JWT + refresh token |
| POST | `/api/v1/auth/refresh` | Public | Rotate refresh token |
| GET | `/api/v1/products?pageNumber=1&pageSize=20` | Public | Paginated products |
| GET | `/api/v1/products/{id}` | Public | Product by id |
| POST | `/api/v1/products` | Admin | Create product |
| PUT | `/api/v1/products/{id}` | Admin | Update product |
| DELETE | `/api/v1/products/{id}` | Admin | Delete product |
| GET | `/api/v1/products/{productId}/items` | Public | Related items |
| POST | `/api/v1/products/{productId}/items` | Admin | Create related item |
| PUT | `/api/v1/products/{productId}/items/{itemId}` | Admin | Update item |
| DELETE | `/api/v1/products/{productId}/items/{itemId}` | Admin | Delete item |
| GET | `/health` | Public | Health check |

## Authentication flow

1. Call `POST /api/v1/auth/login` with the seeded local admin credentials.
2. The server verifies the password hash and returns a short-lived access token plus a longer-lived refresh token.
3. Send the JWT as `Authorization: Bearer <token>` for protected endpoints.
4. When the access token expires, call `/auth/refresh`. The old refresh token is revoked and replaced, implementing refresh-token rotation.
5. Write operations require the `Admin` role. Public reads remain available for easy API evaluation.

### Local demo credentials

```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

These are seeded for assessment/demo use only. Do not use them in production.

## Run with Docker (recommended)

Prerequisites: Docker Desktop.

```bash
docker compose up --build
```

Then open:

- Swagger: `http://localhost:8080/swagger`
- Health: `http://localhost:8080/health`

The compose file starts SQL Server 2022 and the API. The API creates the assessment schema and demo records on first startup.

## Run with local .NET SDK

Prerequisites: .NET 8 SDK and SQL Server reachable on `localhost:1433`.

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/API/API.csproj --launch-profile http
```

Swagger opens at `http://localhost:5099/swagger`.

If your SQL Server connection differs, update `ConnectionStrings:DefaultConnection` in `src/API/appsettings.json` or provide it as an environment variable.

## Example requests

Login:

```bash
curl -X POST http://localhost:5099/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'
```

Public products:

```bash
curl "http://localhost:5099/api/v1/products?pageNumber=1&pageSize=20"
```

Create a product (replace `<JWT>`):

```bash
curl -X POST http://localhost:5099/api/v1/products \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{"productName":"Monitor"}'
```

## Validation and errors

FluentValidation validates request DTOs through a custom MVC action filter (avoiding the deprecated FluentValidation.AspNetCore auto-validation package). Errors and unhandled exceptions are normalized to RFC-style `application/problem+json` responses by custom middleware. Missing domain resources return HTTP 404, invalid input returns HTTP 400, failed authentication returns HTTP 401, and successful creation/deletion use 201/204 respectively.

## Performance considerations

Read-only EF Core queries use `AsNoTracking()`. Product collections are paginated and capped at 100 records per request. ProductName and Item.ProductId are indexed. All database calls are asynchronous, response compression is enabled, and DTOs prevent accidental over-fetching/serialization of EF navigation graphs.

## Security measures

- JWT issuer/audience/signature/lifetime validation
- 15-minute access token and rotating refresh tokens
- Role-based authorization on write operations
- Password hashing via ASP.NET Core `PasswordHasher<T>`
- FluentValidation input validation
- Explicit CORS policy
- HTTPS redirection for normal local/hosted execution
- Basic security response headers
- Secrets can be overridden by environment variables and should not be committed for production

## Testing strategy

`Application.Tests` uses xUnit + Moq to unit-test application services in isolation. `API.Tests` uses `WebApplicationFactory` and EF Core InMemory to verify HTTP behavior, seed data, authorization, and the health endpoint.

```bash
dotnet test
```

## Project structure

```text
src/
  API/              Controllers, middleware, DI extensions, Program
  Application/      DTOs, interfaces, services, validators
  Domain/           Entities and domain exceptions
  Infrastructure/   EF Core, repositories, identity/JWT, logging marker
tests/
  Application.Tests/
  API.Tests/
docs/
  schema.sql
docker-compose.yml
```

## Deployment (high level)

1. Store the SQL connection string and JWT signing key in a secret manager / deployment environment variables.
2. Build and scan the Docker image in CI.
3. Run `dotnet test` as a required CI gate.
4. Push the image to the organization container registry.
5. Deploy behind an HTTPS reverse proxy/load balancer to Azure Container Apps, App Service, AKS, ECS, or a comparable platform.
6. Use a managed SQL Server/Azure SQL instance, centralized logs, health probes, and environment-specific CORS origins.

## Submission screenshot

After starting the application, capture **Swagger UI at `/swagger`** with the Product, Item, and Auth endpoints expanded enough to show the API is running. A second terminal/Swagger screenshot showing `GET /health` = `200` or `GET /api/v1/products` = `200` is also useful.

## Notes

- `DbSeeder` uses `EnsureCreated()` to keep evaluator setup one command for this assessment. In a long-lived production service, replace this with reviewed EF Core migrations executed through the deployment pipeline.
- The supplied `docs/schema.sql` mirrors the requested Product/Item structure and includes the recommended indexes.
# product-rest-api-assessment
