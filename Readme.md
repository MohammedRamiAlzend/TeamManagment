# Team Management System (TMS)

Welcome to the **Team Management System (TMS)**! This is a robust and scalable application designed to make managing teams a breeze. Whether you're tracking tasks, assigning roles, or overseeing projects, TMS is here to streamline your workflows.

Built with **ASP.NET Core**, TMS embraces modern architectural practices like **Clean Architecture**. This means the project is carefully organized into distinct, modular layers, making it incredibly **maintainable**, easy to **scale** as your needs grow, and straightforward to **extend** with new features.

---

## Table of Contents

1.  [Getting Started](#getting-started)
    *   [Prerequisites](#prerequisites)
    *   [Setup](#setup)
    *   [Installing dotnet ef](#installing-dotnet-ef)
    *   [Creating and Applying Migrations](#creating-and-applying-migrations)
    *   [Running the Application](#running-the-application)
2.  [Project Structure - A Glimpse Behind the Scenes](#project-structure---a-glimpse-behind-the-scenes)
    *   [Core](#core)
    *   [TMS.Contract](#tmscontract)
    *   [TMS.Application](#tmsapplication)
    *   [TMS.Infrastructure](#tmsinfrastructure)
    *   [TMS.Server](#tmsserver)
3.  [Key Features](#key-features)
4.  [Default Users (For Development)](#default-users-for-development)
5.  [Contributing](#contributing)
6.  [Contact](#contact)

---

## Getting Started

Ready to get TMS up and running? Follow these simple steps!

### Prerequisites

Before you begin, make sure you have these installed:

*   **.NET SDK 9** (or a compatible version) – [Download .NET SDK](https://dotnet.microsoft.com/download)
*   A compatible database server (like **SQL Server**)

For handling database magic (migrations and updates), you'll also need the **Entity Framework Core** CLI tool, `dotnet ef`.

---

### Setup

Let's get this repository onto your machine and configured:

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/MohammedRamiAlzend/TeamManagment.git
    ```

2.  **Navigate to the solution directory:**
    ```bash
    cd TeamManagment
    ```

3.  **Restore project dependencies:**
    ```bash
    dotnet restore
    ```

4.  **Configure your database connection:**
    Open the `appsettings.json` file located in `TMS.Server`. Find the `ConnectionStrings` section and update it with your database details (server name, username, password, etc.).

---

### Installing dotnet ef

If you don't have it already, install the `dotnet-ef` CLI tool globally for seamless database migrations:

```bash
dotnet tool install --global dotnet-ef
```

To quickly check if it's installed correctly, run:

```bash
dotnet ef --version
```

If you see a version number, you're good to go!

---

### Creating and Applying Migrations

If this is your first time setting up or if the `Migrations` folder in `TMS.Infrastructure` is empty, follow these steps to prepare your database:

1.  **Generate a new migration:**
    ```bash
    dotnet ef migrations add InitialCreate --project TMS.Infrastructure
    ```
    This command will create a new migration file within the `Migrations` folder in the `TMS.Infrastructure` project. Just make sure your `DbContext` is correctly set up in that project!

2.  **Apply the migration to update your database:**
    ```bash
    dotnet ef database update --project TMS.Infrastructure
    ```
    This command will apply the latest database changes to your connected database.

---

### Running the Application

Once your database is ready, you can build and run the application:

1.  **Build the entire solution:**
    ```bash
    dotnet build
    ```

2.  **Run the API server:**
    ```bash
    dotnet run --project TMS.Server
    ```
    Your application will now be accessible via the configured port!

---

## Project Structure - A Glimpse Behind the Scenes

The TMS project is thoughtfully organized into distinct layers. This modular approach isn't just for show; it's designed to keep everything clean, understandable, and efficient for development. Each layer has a specific job, helping us separate concerns and boost productivity!

### 1. Core

This is the heart of our application! The `Core` layer holds all the fundamental business rules and domain logic. Think of it as the brain of TMS.

*   **Entities:** Our core data models and definitions.
*   **Interfaces:** Blueprints for services and repositories, ensuring clear contracts.
*   **AutoMapperClasses:** Helps us effortlessly transform data between different parts of the application.
*   **Support Classes:** Like `DbRequest.cs` for database interactions and `ApiResponse.cs` for consistent API responses.
*   **DependencyInjection.cs:** Sets up how different parts of `Core` talk to each other.

---

### 2. TMS.Contract

This layer is all about agreements! `TMS.Contract` defines the common interfaces, shared data structures (DTOs), and definitions that various parts of the application rely on. It's crucial for ensuring smooth and consistent communication across all layers.

*   **MediatR:** Shared definitions for the commands, queries, and responses that drive our application's actions.
*   **Entities:** Common entity definitions used throughout the system.
*   **CommunicationModels:** Models and DTOs specifically designed for inter-layer communication.
*   **AppPermissions.cs & AppRoles.cs:** Centralized definitions for application permissions and user roles.

---

### 3. TMS.Application

This layer acts as our intelligent coordinator. `TMS.Application` handles incoming requests, validates them, and orchestrates the necessary business logic to fulfill those requests. It's where our application's use cases come to life.

*   **Queries:** How we fetch data from the system (read-only operations).
*   **Commands:** How we make changes to the system's data and state.
*   **DependencyInjection.cs:** Registers services specific to our application's logic.

---

### 4. TMS.Infrastructure

This is where TMS connects with the outside world! `TMS.Infrastructure` manages all the necessary groundwork, like talking to the database, handling data storage, and integrating with any external services.

*   **Data:** Manages our database context (`DbContext`) and Entity Framework Core configurations.
*   **Repositories:** Our dedicated mechanism for accessing and persisting data smoothly.
*   **Migrations:** Keeps track of how our database schema evolves over time.
*   **DependencyInjection.cs:** Registers all our database and external service components.

---

### 5. TMS.Server

This is the public face of our application—the API layer! `TMS.Server` exposes all the endpoints that allow other systems or front-end applications to interact with TMS, fetching and modifying team data.

*   **Controllers:** Our API endpoints, designed following REST principles.
*   **Configuration Files:** Like `appsettings.json`, holding vital settings such as database connections and environment specifics.
*   **DependencyInjection.cs:** Registers services needed for our API and server operations.

---

## Key Features

The Team Management System is packed with modern features and built on solid principles:

*   **Clean and Modular Architecture:** Built with a strong emphasis on **Clean Architecture**, making the codebase easy to understand, manage, and expand.
*   **Smart Service Management:** Uses **Dependency Injection** to keep our services well-organized and easy to swap out.
*   **Efficient Command & Query Handling:** Implements the industry-standard **CQRS pattern** via MediatR for clear separation of reading and writing data.
*   **Seamless Data Transformation:** Leverages **AutoMapper** for dynamic and effortless object mapping between different layers.
*   **Evolving Database Schema:** **EF Core** handles database migrations, ensuring our database keeps pace with application changes.
*   **Clean API Endpoints:** Provides **RESTful API endpoints** with a clear separation of concerns, making integration straightforward.
*   **Ready for Growth:** Designed to be easily **extensible** with new features or services as your project evolves.

---

## Default Users (For Development)

For your convenience during testing and development, TMS includes a few pre-seeded default users. You can use these credentials to quickly explore the system:

### Sign-in Credentials

*   **Admin User**
    *   Username: `rami`
    *   Password: `123`
    *   Roles: Admin, TeamLeader

*   **Employee User 1**
    *   Username: `ibrahim`
    *   Password: `123`
    *   Roles: Employee

*   **Employee User 2**
    *   Username: `rama`
    *   Password: `123`
    *   Roles: Employee

*(Note: These default details can and should be customized or replaced for any production environment. For more information, check out the `UsersSeeder` in the project codebase.)*

---

## Contributing

We welcome contributions! Whether you're reporting bugs, suggesting features, or submitting pull requests, your input is highly valued. Please check out our [issues page](https://github.com/MohammedRamiAlzend/TeamManagment/issues) to see how you can help. Before contributing code, take a moment to review the project structure and code style guidelines.

---

## Contact

Got questions, need support, or just want to share feedback? Feel free to reach out:

**[ramialzend@gmail.com]**
