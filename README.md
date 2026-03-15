# SDMISAppQG - Backend (Command Center Management System)

SDMISAppQG is a critical backend application built with **ASP.NET Core 10.0** designed for managing firefighter interventions, vehicle tracking (GPS), and real-time communication between different operational modules. It serves as the core intelligence for the **Command Center (Poste de Commandement)**.

## 🚀 Technology Stack

- **Framework:** .NET 10.0 (ASP.NET Core)
- **Database:** PostgreSQL with **PostGIS** for geospatial data management.
- **ORM:** Entity Framework Core (EF Core) with NetTopologySuite.
- **Real-time:** SignalR (GpsHub, TelemetryHub).
- **Messaging:** RabbitMQ (for inter-service communication, e.g., with Java-based modules).
- **Security:** Keycloak (OIDC/JWT Authentication).
- **API Documentation:** OpenAPI (Swagger) with **Scalar** UI.
- **Containerization:** Docker & Docker Compose.

## 🏗️ Architecture

The project follows a modular and service-oriented architecture:

- **Controllers:** REST APIs for Incidents, Vehicles, Users, Interventions, etc.
- **Hubs:** SignalR Hubs for real-time telemetry and GPS updates.
- **Models:** 
  - `Entities`: EF Core database models.
  - `DTOs`: Data Transfer Objects for API and SignalR responses (strict typing).
- **Infrastructure:**
  - `Services`: Business logic (Intervention, Telemetry, RabbitMQ).
  - `Database`: EF Core `AppDbContext` and PostGIS configurations.

## ✨ Key Features

- **Geospatial Tracking:** Real-time tracking of vehicles using GPS coordinates stored as PostGIS points.
- **Incident Management:** Lifecycle management of emergency incidents (Declared -> Ongoing -> Completed).
- **Intervention Assignment:** Sophisticated assignment system ensuring vehicles aren't double-assigned and calculating itineraries.
- **Telemetry Processing:** Real-time ingestion of vehicle sensor data (fuel, consumables) via SignalR and RabbitMQ.
- **User Management:** Integration with Keycloak for secure access control.

## 🛠️ Setup & Installation

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- A running Keycloak instance (or update `appsettings.json` with your config)

### Running with Docker (Recommended)

The easiest way to start the environment (DB, RabbitMQ, API) is using Docker Compose:

```powershell
docker-compose up -d
```

### Manual Setup

1. **Configure Database:**
   Update `ConnectionStrings:DefaultConnection` in `appsettings.json`.

2. **Apply Migrations:**
   ```powershell
   dotnet ef database update
   ```

3. **Run the Application:**
   ```powershell
   dotnet run --project SDMISAppQG
   ```

4. **Access API Documentation:**
   Open `http://localhost:5078/scalar/v1` in your browser.

## 🧪 Testing

The project includes a comprehensive test suite (Unit & Integration):

```powershell
# Run all tests
dotnet test

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📡 API & Real-time Endpoints

- **REST API:** `http://localhost:5078/api/`
- **GpsHub (SignalR):** `/hubs/gps`
  - Client method: `ReceivePosition(VehicleDto vehicle)`
- **TelemetryHub (SignalR):** `/hubs/telemetry`

## 📁 Project Structure

```text
SDMISAppQG/
├── Controllers/         # API Endpoints
├── Database/            # DbContext & Migrations
├── Hubs/                # SignalR Hubs
├── Infrastructure/      # Services & RabbitMQ Logic
├── Interfaces/          # Hub and Service interfaces
├── Models/
│   ├── DTOs/            # Serialization-safe objects
│   ├── Entities/        # Database models
│   └── Enums/           # Business domain enums
└── Properties/          # Launch settings
```