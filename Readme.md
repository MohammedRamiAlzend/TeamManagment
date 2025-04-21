# Team Management System (TMS)

The **Team Management System (TMS)** is a robust and scalable application designed to streamline team management workflows. Leveraging modern architectural practices, TMS is structured into modular and well-defined layers to ensure **maintainability**, **scalability**, and **ease of extension**. It is built using **ASP.NET Core** with a keen focus on applying clean architecture principles.

---

## Table of Contents

1. [Project Structure](#project-structure)
    - [Core](#core)
    - [TMS.Application](#tmsapplication)
    - [TMS.Infrastructure](#tmsinfrastructure)
    - [TMS.Server](#tmsserver)
2. [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Setup](#setup)
    - [Installing dotnet ef](#installing-dotnet-ef)
    - [Creating and Applying Migrations](#creating-and-applying-migrations)
3. [Technologies Used](#technologies-used)
4. [Features](#features)
5. [Directory Structure](#directory-structure)
6. [Contributing](#contributing)
7. [Contact](#contact)

---

## Project Structure

The project is organized into **distinct layers**, each responsible for specific functionalities. This modular approach enhances separation of concerns and enriches development productivity.

### 1. Core

- **Purpose**: Encapsulates the application's primary business logic and its domain-specific rules.
- **Key Components**:
    - **Entities**: Represents essential domain models and entities.
    - **Interfaces**: Provides abstraction through service and repository contracts.
    - **AutoMapperClasses**: Houses configuration for mapping objects between various layers using AutoMapper.
    - **Support Classes**:
        - `DbRequest.cs`: Supports handling of database-related requests.
        - `ApiResponse.cs`: Standardized format for wrapping API responses.
    - **DependencyInjection.cs**: Enables dependency injection for services within the core layer.

---

### 2. TMS.Application

- **Purpose**: Acts as a middle layer to handle requests (commands/queries), validate inputs, and invoke the appropriate business logic.
- **Key Components**:
    - **Queries**: Implements query patterns for retrieving application data (read-only).
    - **Commands**: Manages operations that modify state or data within the system.
    - **DependencyInjection.cs**: Registers dependencies for application services into the service container.

---

### 3. TMS.Infrastructure

- **Purpose**: Handles application infrastructure requirements such as database interaction, data persistence, and integration with external services.
- **Key Components**:
    - **Data**: Manages database contexts (`DbContext`) and configurations for Entity Framework Core.
    - **Repositories**: Implements the repository pattern for seamless data access.
    - **Migrations**: Contains migration files to evolve the database schema as changes are made.
    - **DependencyInjection.cs**: Registers repositories, data-access services, and other infrastructure components.

---

### 4. TMS.Server

- **Purpose**: Provides the backend API layer. It exposes RESTful endpoints for accessing and manipulating system data.
- **Key Components**:
    - **Controllers**: Defines API endpoints for system interaction, adhering to REST principles.
    - **Configuration Files**:
        - `appsettings.json`: Holds application-specific configurations (database connection, environment settings, etc.).
    - **DependencyInjection.cs**: Registers HTTP-specific or server-side services for dependency injection.

---

## Getting Started

### Prerequisites

Before setting up the project, ensure the following are installed:
- **.NET SDK 9** (compatible version required) – [Download .NET SDK](https://dotnet.microsoft.com/download)
- A compatible database server (e.g., **SQL Server**)

For database migrations and updates, **Entity Framework Core**'s CLI tool `dotnet ef` is required.

---

### Setup

Follow these steps to clone, configure, and run the project:

1. Clone the repository to your local machine:
   ```bash
   git clone https://github.com/MohammedRamiAlzend/TeamManagment.git
   ```

2. Navigate to the solution directory:
   ```bash
   cd TeamManagment
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Configure the database connection in the `appsettings.json` file located in **TMS.Server**. Update the `ConnectionStrings` field with your database details (server name, username, password, etc.).

---

### Installing dotnet ef

To enable database migrations and updates, install the `dotnet-ef` CLI tool globally:
```bash 
dotnet tool install --global dotnet-ef
```
To verify the installation, run:
```bash 
dotnet ef --version
```
If the command outputs the version of the `dotnet ef` tool, it was installed successfully!

---

### Creating and Applying Migrations

If the project does not already include the database migrations (`Migrations` folder is missing or empty), follow these steps to create and apply them:

1. **Generate a new migration**:
   ```bash
   dotnet ef migrations add InitialCreate --project TMS.Infrastructure
   ```

    - This command will create a migration file under a newly created `Migrations` folder in the **TMS.Infrastructure** project.

    - Ensure the `DbContext` is properly configured in the **TMS.Infrastructure** project.

2. **Apply the migration to update the database**:
   ```bash
   dotnet ef database update --project TMS.Infrastructure
   ```

    - This command will apply the latest migration to your database.

---

### Running the Application

Once the migrations are applied, you can build and run the project as follows:

1. Build the solution:
   ```bash
   dotnet build
   ```

2. Run the API server:
   ```bash
   dotnet run --project TMS.Server
   ```

   The application will now be accessible from your configured port.

---


## Technologies Used

The following technologies and tools are utilized in the project:

- **.NET Core**: Framework for cross-platform development.
- **ASP.NET Core**: Backend development framework for building RESTful APIs and web applications.
- **Entity Framework Core**: ORM framework for database interactions and migrations.
- **MediatR**: Implements the CQRS pattern and request/response functionality.
- **AutoMapper**: For seamless object mapping between models.
- **Dependency Injection**: Used extensively across all layers for better testability and modularity.

---

## Features

- Modularized architecture following **clean architecture principles**.
- Dependency injection for service management.
- Industry-standard **CQRS pattern** implementation via MediatR.
- Dynamic object mapping using **AutoMapper**.
- Database migration and schema evolution with **EF Core**.
- RESTful API endpoints with clear separation of concerns.
- Extensible through additional features or services.

---

## Directory Structure plaintext
TeamManagement/ ├── TMS.Core/ ├── TMS.Application/ ├── TMS.Infrastructure/ ├── TMS.Server/ └── README.md

- **TMS.Core**: Business logic and domain rules.
- **TMS.Application**: Handles request pipelines using CQRS.
- **TMS.Infrastructure**: Database access and external service interactions.
- **TMS.Server**: API controllers and server configurations.

---

## Contributing

Contributions are highly welcomed! Whether it's bug reports, feature requests, or pull requests, feel free to engage via the repository. Start by checking the [issues page](https://github.com/MohammedRamiAlzend/TeamManagment/issues). Prior to contributing, review the project structure and code style guidelines.

---

## Contact

For questions, support, or feedback, contact:  
**[ramialzend@gmail.com]**
