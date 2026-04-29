# MedCore

> This README is a work in progress, just like the project itself.

MedCore is a personal backend project built with ASP.NET Core (.NET 10).
The goal is to design a modular medical management API with clean architecture
and clear separation of concerns.

## What is built so far

### Identity Module

- JWT authentication with refresh token rotation
- SHA-256 refresh token hashing stored in PostgreSQL
- HttpOnly cookie-based token delivery
- Email confirmation flow via MailKit (Ethereal Email for development)
- Role-based access control (Admin, Patient, Doctor)
- Structured logging with Serilog
- RFC 7807 ProblemDetails error responses
- Background service for expired token cleanup
- API documentation via Scalar UI at `/scalar/v1`

## Tech Stack

- ASP.NET Core (.NET 10)
- PostgreSQL 17 (Docker)
- Entity Framework Core
- ASP.NET Core Identity
- Serilog
- MailKit

## Architecture

Modular monolith with clean layer separation.
Each module is designed to be extracted into a standalone microservice.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop

### Run the database

```bash
docker-compose up -d
```

### Apply migrations

Run from the `MedCore.Modules.Identity` project directory:

```bash
dotnet ef database update
```

### API docs

Start the app in Development mode and visit: https://localhost:7212/scalar/v1

## Status

Actively in development. New modules will be added progressively.

## Author

Jerald James Capao — [GitHub](https://github.com/your-username)