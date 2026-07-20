# Exit Caff — Backend API

ASP.NET Core 8 Web API for the Exit Caff Bakery & Café Management System. Clean Architecture, EF Core + PostgreSQL, JWT auth with refresh tokens, role-based authorization.

## Architecture

```
src/
  ExitCafe.Domain          Entities, enums, domain exceptions. No dependencies.
  ExitCafe.Application      DTOs, service interfaces/implementations, FluentValidation validators,
                             AutoMapper profiles, repository/UoW interfaces. Depends on Domain only.
  ExitCafe.Infrastructure   EF Core DbContext + configurations + migrations, repository/UoW
                             implementations, JWT/password/audit services. Depends on Application + Domain.
  ExitCafe.WebApi            Controllers, Program.cs composition root, middleware, Swagger.
```

Repository + Unit of Work pattern, service layer, DTOs at the API boundary, dependency injection
throughout, global exception middleware, Serilog structured logging, audit logging on state-changing
actions.

## Current scope (core slice)

Implemented: Auth (register/login/refresh/revoke), Categories, Products (with images, featured/best-seller/
new-arrival/today's-special flags, search/filter/sort/paginate), Orders (guest + registered checkout,
status workflow, cancel), Customers + addresses, audit logging, role seeding (SuperAdmin/Admin/Manager/
Staff/Customer).

Not yet built (tables/entities from the full spec not in this slice): Inventory, Ingredients, Suppliers,
Purchase Orders, Coupons/Promotions, Reviews, Blog, Gallery, Contact Messages, Custom Cake Orders,
Notifications, Settings, Reports/Analytics endpoints. Add these as additional entities + Application
services + controllers following the same layering.

## Prerequisites

- .NET 8 SDK
- PostgreSQL 16 (or Docker)

## Run locally

```bash
# 1. Start Postgres (or use your own instance and update appsettings)
docker run --name exitcaff-postgres -e POSTGRES_DB=exitcaffdb -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16-alpine

# 2. Set secrets (don't commit real secrets to appsettings.json)
cd src/ExitCafe.WebApi
dotnet user-secrets init
dotnet user-secrets set "Jwt:Secret" "<a-strong-random-32+-char-secret>"
dotnet user-secrets set "SeedAdmin:Password" "<a-strong-password>"

# 3. Run — migrations apply and seed data (roles + super admin) run automatically on startup
dotnet run --project src/ExitCafe.WebApi
```

API available at `https://localhost:{port}`, Swagger UI at `/swagger` in Development.

## Run with Docker Compose

```bash
cp .env.example .env   # fill in real secrets
docker compose up --build
```

## Migrations

```bash
dotnet ef migrations add <Name> \
  --project src/ExitCafe.Infrastructure/ExitCafe.Infrastructure.csproj \
  --startup-project src/ExitCafe.WebApi/ExitCafe.WebApi.csproj \
  --output-dir Persistence/Migrations
```

Migrations apply automatically on API startup (`DataSeeder.SeedAsync`); no manual `dotnet ef database update`
needed in normal dev flow.

## Auth

- `POST /api/auth/register`, `/login`, `/refresh-token`, `/revoke-token`
- Access token: JWT, 60 min default expiry. Refresh token: opaque random value, 7 day expiry, stored
  server-side (`RefreshTokens` table), rotated on each refresh.
- Roles: `SuperAdmin`, `Admin`, `Manager`, `Staff`, `Customer`. Authorization policies: `AdminOnly`
  (SuperAdmin/Admin), `StaffAndAbove` (SuperAdmin/Admin/Manager/Staff).
- A default SuperAdmin is seeded on first run using `SeedAdmin:Email` / `SeedAdmin:Password` — change the
  password immediately in a real deployment.

## Known items

- AutoMapper is pinned at 13.0.1. NuGet flags `GHSA-rvv3-g6hj-g44x` (unbounded-recursion DoS) against this
  version; the fix ships in 15.1.1+, which also introduced AutoMapper's commercial licensing. Our mapping
  profiles don't map attacker-controlled recursive graphs, so the practical risk is low — revisit if/when
  upgrading is worth the licensing tradeoff.
- Coupon/discount logic is stubbed (`CouponCode` is stored on the order but not validated against a
  `Coupons` table, since that module isn't in this slice yet).
- Tax (5%) and delivery fee (flat ₹40) are hardcoded in `OrderService` — move to `Settings` once that
  module exists.
