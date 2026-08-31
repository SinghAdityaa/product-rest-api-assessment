# Product REST API – Technical Assessment

A production-minded RESTful backend API built with **.NET 8**, **ASP.NET Core Web API**, **SQL Server**, and **Entity Framework Core**.

The solution implements CRUD operations for Products and related Items with JWT authentication, refresh-token rotation, role-based authorization, validation, structured logging, API versioning, Swagger/OpenAPI documentation, automated tests, pagination, Docker support, and centralized error handling.

---

## Architecture

```text
Client / Swagger
       |
       v
ASP.NET Core Web API
  ├── API Versioning
  ├── JWT Authentication
  ├── Role Authorization
  ├── FluentValidation
  └── Exception Middleware
       |
       v
Application Layer
  ├── DTOs
  ├── Interfaces
  ├── Services
  └── Validators
       |
       v
Infrastructure Layer
  ├── EF Core Repositories
  ├── JWT / Refresh Tokens
  └── ApplicationDbContext
       |
       v
SQL Server
  ├── Product 1 ---- * Item
  └── AppUser  1 ---- * RefreshToken
```

The solution follows layered architecture with dependencies pointing inward:

- **API** → Application / Infrastructure
- **Infrastructure** → Application / Domain
- **Application** → Domain
- **Domain** → No infrastructure dependencies

---

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server 2022
- JWT Authentication
- FluentValidation
- xUnit
- Moq
- WebApplicationFactory
- Swagger / OpenAPI
- Serilog
- Docker & Docker Compose

---

## API Endpoints

| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Public | Authenticate user |
| POST | `/api/v1/auth/refresh` | Public | Rotate refresh token |
| GET | `/api/v1/products` | Public | Get paginated products |
| GET | `/api/v1/products/{id}` | Public | Get product by ID |
| POST | `/api/v1/products` | Admin | Create product |
| PUT | `/api/v1/products/{id}` | Admin | Update product |
| DELETE | `/api/v1/products/{id}` | Admin | Delete product |
| GET | `/api/v1/products/{productId}/items` | Public | Get product items |
| POST | `/api/v1/products/{productId}/items` | Admin | Create item |
| PUT | `/api/v1/products/{productId}/items/{itemId}` | Admin | Update item |
| DELETE | `/api/v1/products/{productId}/items/{itemId}` | Admin | Delete item |
| GET | `/health` | Public | API health check |

---

## Authentication

The API uses JWT authentication with refresh-token rotation.

### Authentication Flow

1. Authenticate using `POST /api/v1/auth/login`.
2. The server validates the credentials.
3. An access token and refresh token are returned.
4. Send the access token with protected requests:

```text
Authorization: Bearer <token>
```

5. When the access token expires, use `/api/v1/auth/refresh`.
6. The previous refresh token is revoked and replaced with a new token.

Write operations require the **Admin** role.

### Demo Credentials

For local assessment/demo purposes:

```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

> These credentials are intended only for local assessment/demo use.

---

## Running with Docker

### Prerequisites

- Docker Desktop

Clone the repository:

```bash
git clone https://github.com/SinghAdityaa/product-rest-api-assessment.git
cd product-rest-api-assessment
```

Start the application:

```bash
docker compose up --build
```

Docker Compose starts:

- ASP.NET Core API
- SQL Server 2022
- Product assessment database

Open Swagger:

```text
http://localhost:8080/swagger
```

Health check:

```text
http://localhost:8080/health
```

---

## Running Locally

### Prerequisites

- .NET 8 SDK
- SQL Server

Restore dependencies:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Run tests:

```bash
dotnet test
```

Start the API:

```bash
dotnet run --project src/API/API.csproj --launch-profile http
```

Swagger will be available at:

```text
http://localhost:5099/swagger
```

---

## Example Requests

### Get Products

```bash
curl "http://localhost:8080/api/v1/products?pageNumber=1&pageSize=20"
```

### Login

```bash
curl -X POST "http://localhost:8080/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'
```

### Create Product

Replace `<JWT>` with the access token returned by the login endpoint.

```bash
curl -X POST "http://localhost:8080/api/v1/products" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{"productName":"Monitor"}'
```

---

## Validation & Error Handling

Request DTOs are validated using **FluentValidation**.

A centralized exception-handling middleware provides consistent `application/problem+json` error responses.

Typical HTTP responses include:

| Status | Meaning |
|---|---|
| `200 OK` | Successful request |
| `201 Created` | Resource created |
| `204 No Content` | Successful deletion/update where applicable |
| `400 Bad Request` | Validation failure |
| `401 Unauthorized` | Authentication required |
| `403 Forbidden` | Insufficient permissions |
| `404 Not Found` | Resource not found |
| `500 Internal Server Error` | Unexpected server error |

---

## Performance Considerations

The implementation includes:

- `AsNoTracking()` for read-only EF Core queries
- Async database operations
- Pagination for collection endpoints
- Maximum page-size protection
- Database indexes
- Response compression
- DTO-based responses to avoid unnecessary entity serialization

---

## Security

Security measures include:

- JWT signature, issuer, audience, and lifetime validation
- Short-lived access tokens
- Refresh-token rotation
- Role-based authorization
- ASP.NET Core password hashing
- FluentValidation input validation
- CORS configuration
- HTTPS redirection
- Security response headers
- Environment-variable support for sensitive configuration

Production deployments should store database credentials and JWT secrets in a secure secret-management solution.

---

## Testing

The solution contains both unit and integration tests.

### Application Tests

Built with:

- xUnit
- Moq

These tests verify application/service behavior independently from infrastructure.

### API Integration Tests

Built with:

- xUnit
- `WebApplicationFactory`
- EF Core InMemory provider

The tests verify:

- Product API behavior
- Authentication requirements
- HTTP status codes
- Health endpoint

Run all tests with:

```bash
dotnet test
```

---

## Project Structure

```text
product-rest-api-assessment/
│
├── src/
│   ├── API/
│   │   ├── Controllers/
│   │   ├── Extensions/
│   │   ├── Filters/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── Dockerfile
│   │
│   ├── Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   │
│   ├── Domain/
│   │   ├── Entities/
│   │   └── Exceptions/
│   │
│   └── Infrastructure/
│       ├── Data/
│       ├── Identity/
│       └── Logging/
│
├── tests/
│   ├── API.Tests/
│   └── Application.Tests/
│
├── docs/
│   └── schema.sql
│
├── docker-compose.yml
├── ProductApiAssessment.sln
└── README.md
```

---

## Database

The primary relationship follows the assessment specification:

```text
Product
   |
   | 1
   |
   | *
 Item
```

A Product can contain multiple Items, while each Item belongs to one Product.

The SQL schema is also available at:

```text
docs/schema.sql
```

---

## Deployment Considerations

For a production deployment:

1. Store JWT keys and database credentials in a secret manager.
2. Run `dotnet test` in the CI/CD pipeline.
3. Build and scan the Docker image.
4. Push the image to a container registry.
5. Deploy behind HTTPS using a reverse proxy or load balancer.
6. Use managed SQL Server/Azure SQL.
7. Configure centralized logging and health monitoring.
8. Configure environment-specific CORS policies.

Possible deployment targets include Azure App Service, Azure Container Apps, AKS, AWS ECS, or similar container platforms.

---

## Assessment Requirements Covered

- RESTful Product CRUD API
- Product → Item relationship
- .NET 8 / ASP.NET Core
- SQL Server
- Entity Framework Core
- Repository pattern
- Service layer
- JWT authentication
- Refresh-token strategy
- Role-based authorization
- FluentValidation
- Centralized error handling
- API versioning
- Swagger/OpenAPI
- Pagination
- Structured logging
- Unit tests
- Integration tests
- Docker
- Docker Compose
- Health endpoint
- High-level architecture
- Security considerations
- Deployment documentation

---

## Author

**Aditya Singh**

GitHub: `SinghAdityaa`