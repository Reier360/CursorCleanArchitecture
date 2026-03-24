# TestApi Solution Overview

This solution is a layered **.NET 8 Web API** for managing users and orders, with built-in token auth for testing.

## 1. API Host Project (`TestApi`)

- Boots ASP.NET Core, Swagger, JWT auth, and Duende IdentityServer in-memory config.
- Registers DI for user/order services and in-memory repositories.
- Key setup is in `TestApi/Program.cs`.

## 2. Authentication

- Uses local IdentityServer with a test client (`test-client` / `secret`) and one test user (`alice` / `password`).
- `POST /auth/login` requests a token from `/connect/token`.
- Config files:
  - `TestApi/Identity/Config.cs`
  - `TestApi/Identity/TestUsers.cs`
  - `TestApi/Controllers/AuthController.cs`

## 3. Business/API Features

- Protected endpoints (`[Authorize]`) for:
  - Users CRUD at `/api/users`
  - Orders CRUD + filter by user at `/api/orders` and `/api/orders/user/{userId}`
- Controllers:
  - `TestApi/Controllers/UsersController.cs`
  - `TestApi/Controllers/OrdersController.cs`
- Services enforce rules like unique/valid email and non-negative order totals:
  - `TestApi.Application/Users/UserService.cs`
  - `TestApi.Application/Orders/OrderService.cs`

## 4. Architecture

- `TestApi.Domain`: entities + repository interfaces.
- `TestApi.Application`: DTOs, request models, service logic.
- `TestApi.Infrastructure`: in-memory repository implementations (no database).
- In-memory repos mean data resets when the app restarts.

## 5. Tests

- `TestApi.Tests` has unit tests for user service, users controller, and in-memory user repository.
- Main test files include:
  - `TestApi.Tests/UserServiceTests.cs`
  - `TestApi.Tests/UsersControllerTests.cs`
