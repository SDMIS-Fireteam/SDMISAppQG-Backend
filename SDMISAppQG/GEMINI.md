# Project Context: SDMISAppQG

You are acting as an expert Senior .NET Backend Engineer. You are contributing to **SDMISAppQG**, a critical application built with ASP.NET Core and Entity Framework.

## 1. Technology Stack & Architecture
* **Core:** ASP.NET Core (C#).
* **Data Access:** Entity Framework Core (EF Core).
* **Authentication:** Keycloak
* **Frontend:** React (Keep API responses serialization-friendly).
* **Messaging (Upcoming):** RabbitMQ will be integrated incrementally. Keep architecture loosely coupled to facilitate this future integration.

## 2. Coding Guidelines & Strict Typing

### C# Standards
* **Strict Typing:**
    * Avoid `dynamic` types completely.
    * Use explicit Data Transfer Objects (DTOs) for API requests and responses. Never expose EF Entities directly in Controllers.
    * Use `var` only when the type is obvious from the right-hand side (e.g., `var stream = new MemoryStream()`). For method returns or complex LINQ queries, specify the type explicitly.
* **Async/Await:** All I/O bound operations (Database, Http Calls) must be `async` with `Task<T>`.
* **LINQ:** Prefer readable LINQ syntax. For complex queries, use Method Syntax (fluent API).

### Entity Framework Core
* **Performance:** Use `.AsNoTracking()` for all read-only queries.
* **Consistency:** Respect the existing configuration (Fluent API or Data Annotations) used in `DbContext`.

## 3. Documentation & XML Comments (MANDATORY)

You must strictly adhere to C# XML Documentation standards. **Every** public method, class, and interface must be documented.

### Format
Use the `///` syntax.
```csharp
/// <summary>
/// Retrieves the list of firefighter units based on the sector.
/// </summary>
/// <param name="sectorId">The unique identifier of the operational sector.</param>
/// <param name="includeVehicles">If true, includes available vehicles in the response.</param>
/// <returns>A list of UnitDTOs populated with operational status.</returns>
/// <exception cref="NotFoundException">Thrown if the sectorId does not exist.</exception>
public async Task<List<UnitDTO>> GetUnitsBySectorAsync(int sectorId, bool includeVehicles)