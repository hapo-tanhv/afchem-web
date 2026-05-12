# Claude Project Rules - ASP.NET Core C#

## General Rules

- Always follow SOLID principles
- Prefer clean architecture
- Avoid duplicated code
- Use dependency injection
- Use async/await consistently
- Never use `.Result` or `.Wait()`
- Keep methods under 50 lines if possible
- Use meaningful naming
- Avoid magic strings and magic numbers
- Use `var` only when type is obvious

---

# Architecture

Project structure:

- API Layer
- Application Layer
- Domain Layer
- Infrastructure Layer

Rules:

- Controllers must be thin
- Business logic belongs in services
- Repository only handles data access
- Domain entities must not depend on infrastructure
- DTOs must not leak EF entities

---

# ASP.NET Core Rules

- Use RESTful naming
- Use IActionResult for APIs
- Validate requests using FluentValidation
- Use middleware for exception handling
- Use Serilog for logging
- Use AutoMapper only for simple mapping
- Prefer minimal APIs only for small endpoints

---

# Entity Framework Rules

- Always use AsNoTracking() for readonly queries
- Avoid N+1 queries
- Use projection instead of Include when possible
- Use transactions for multi-step updates
- Never expose DbContext directly outside infrastructure layer

---

# Naming Convention

## Classes

- PascalCase

## Private fields

- _camelCase

## Interfaces

- Prefix with I

## Async methods

- Suffix Async

## DTOs

- Suffix Request / Response

---

# Unit Test Rules

- Use xUnit
- Use FluentAssertions
- Naming format:
  MethodName_State_ExpectedResult

- Avoid mocking entities
- Mock only external dependencies
- One assert purpose per test

---

# Performance Rules

- Avoid unnecessary allocations
- Use pagination for list APIs
- Cache expensive queries
- Avoid loading large object graphs

---

# Security Rules

- Never trust client input
- Validate all uploads
- Sanitize logs
- Do not expose stack traces
- Use authorization attributes

---

# Code Generation Rules

When generating code:

- Generate complete production-ready code
- Include namespaces
- Include dependency injection registration
- Include XML comments for public methods
- Include cancellation token for async methods
- Include validation
- Include error handling

---

# Forbidden

- Do not generate placeholder TODO code
- Do not generate fake implementations
- Do not skip null handling
- Do not use static mutable state

Before writing code:

- First analyze existing project patterns
- Reuse existing abstractions
- Follow existing naming conventions
- Do not introduce new architecture styles
- Do not create unnecessary base classes
- Do not add new libraries unless necessary

When editing code:

- Modify minimum required lines
- Preserve existing behavior
- Preserve backward compatibility
- Do not refactor unrelated code

When querying database:
- Prefer projection
- Avoid Include chains
- Use pagination
- Prevent N+1 queries
- Use AsNoTracking for readonly
