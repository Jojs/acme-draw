# Acme Draw – Competition Entry

Small ASP.NET Core Razor Pages application for submitting entries to a competition using a serial number.

The implementation focuses on a simple, testable domain model and a clean separation between domain logic, web layer and persistence.

---

## Context
Technical assignment completed in March 2026 to demonstrate rapid technological onboarding in an unfamiliar stack (.NET 8). Completed in collaboration with AI for code reviews and architectural trade-off discussions.

---

## Technology

* .NET 8
* ASP.NET Core Razor Pages
* Entity Framework Core
* SQL Server
* xUnit

---

## Solution structure

The solution contains three projects.

### Acme.Draw.Core

Contains the domain logic and business rules.

Main components:

* `DrawEntryService`
* request/result models
* error codes
* repository interfaces

The domain layer has no dependency on EF or ASP.NET.
This makes the business logic easy to unit test.

### Acme.Draw.Web

ASP.NET Core Razor Pages application.

Responsibilities:

* UI for submitting competition entries
* EF Core persistence
* dependency injection configuration
* database migrations
* seed data for valid serial numbers
* paginated view of entries

### Acme.Draw.Tests

Unit tests for the domain logic.

The tests use simple in-memory repositories to verify business rules without requiring a database.

---

## Key design decisions

**Domain logic separated from infrastructure**

Business rules are implemented in `Acme.Draw.Core`.
The domain layer depends only on interfaces, which makes the logic easy to test and independent of the web framework or database.

**Repository abstraction**

Persistence is accessed through repository interfaces.
This allows the domain logic to be tested with in-memory implementations while the web application uses EF Core repositories.

**Simple MVP scope**

The solution intentionally keeps the implementation simple and focused on the assignment requirements.
Features such as authentication, user accounts or advanced validation are intentionally excluded.

**Testable business rules**

Unit tests cover the most important domain rules, including age validation, serial number validation and the maximum number of submissions per serial number.

**Progressive hardening**

Some production concerns (for example database-level concurrency protection) are acknowledged but intentionally left out of the MVP to keep the solution clear and focused.

---

## Domain model

A user can submit an entry with:

* First name
* Last name
* Email
* Date of birth
* Serial number

Each submission represents **one entry in the draw**.

---

## Business rules

The service enforces the following rules:

* First name and last name are required
* Email must be valid
* User must be **18 years or older**
* Serial number must exist
* A serial number can be used **a maximum of two times**

These rules are implemented in `DrawEntryService`.

---

## Concurrency note

In the current MVP, the rule *"maximum two entries per serial number"* is enforced in the application logic.

The service:

1. checks how many submissions already exist for the serial number
2. rejects the submission if the count is already two

This approach is sufficient for a simple application and keeps the domain logic easy to test.

In a production system the rule would likely also be enforced at the database level to fully protect against concurrent submissions.

Possible approaches could include:

* database constraints
* transactional inserts
* slot-based reservation models

These were intentionally left out to keep the implementation simple.

---

## Features

The application includes:

* competition entry form
* server and client validation
* pagination for viewing entries
* navigation between entry form and entries list
* seed data for valid serial numbers
* unit tests for business rules

---

## Running the application

### 1. Start SQL Server

Example using Docker:

```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Your_str0ng_Passw0rd!" \
  -p 1433:1433 \
  --name acme-draw-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

If the container already exists you can start it again with:

```bash
docker start acme-draw-sql
```

### 2. Run the application

```bash
dotnet run --project Acme.Draw.Web
```

The ASP.NET development server will start and print the listening URLs in the console output.

At startup the application will:

* apply database migrations
* seed a small set of valid serial numbers

The application will then be available locally via the development server.

---

## Running tests

Run all unit tests with:

```bash
dotnet test
```

The tests verify the core business rules including:

* invalid serial numbers
* age validation
* maximum two entries per serial number
* invalid email
* missing names
