# CoffeeNChill – Coffee Shop Management System

## 1. Project Overview

CoffeeNChill is a cloud-based coffee shop management system developed using C# and Microsoft Azure technologies.

The application provides functionality for managing coffee shop menu items, handling user registration and authentication, and controlling access to protected resources.

The system uses an API Gateway to manage incoming API requests and communicate with the backend Azure Functions.

## 2. Technologies Used

The project uses the following technologies:

- C# and .NET
- Azure Functions (.NET Isolated Worker)
- ASP.NET Core Web API
- Azure Table Storage
- Azure Blob Storage
- Azure Queue Storage
- Azure Storage Emulator (Azurite)
- Docker and Docker Compose
- Docker Hub
- Swagger / OpenAPI
- JWT Authentication
- Git and GitHub

## 3. System Architecture

The CoffeeNChill application consists of three main components:

### API Gateway
The API Gateway acts as the entry point for client requests. It handles incoming requests, forwards them to the appropriate backend services and provides a central location for API documentation and security configuration.

### Azure Functions
The backend consists of Azure Functions that handle the application's business logic, including menu management and user authentication.

### Azurite
Azurite provides local Azure Storage emulation for development and testing.

The Gateway communicates with the Functions application through internal Docker networking.

## 4. Main Features

### Menu Management
The system supports:

- Retrieving all menu items.
- Retrieving menu items by category.
- Adding new menu items.
- Updating existing menu items.
- Deleting menu items.

### User Authentication

The authentication functionality includes:

- User registration.
- User login.
- JWT token generation.
- Protected user profile access.
- Role-based access control.

### API Documentation

Swagger UI provides an interactive interface for viewing and testing the available API endpoints.

## 5. API Endpoints

The following endpoints are exposed through the API Gateway.

### Authentication

| HTTP Method | Endpoint | Description |
|---|---|---|
| POST | /api/users/register | Registers a new user |
| POST | /api/users/login | Authenticates an existing user |
| GET | /api/users/profile | Retrieves the authenticated user's profile |

### Menu Management

| HTTP Method | Endpoint | Description |
|---|---|---|
| GET | /api/menu | Retrieves all menu items |
| GET | /api/menu/category/{category} | Retrieves menu items by category |
| POST | /api/menu | Creates a new menu item |
| PUT | /api/menu/{id} | Updates an existing menu item |
| DELETE | /api/menu/{id} | Deletes a menu item |

Note: Access to certain menu operations is restricted according to user roles.

## 6. Authentication and Security

CoffeeNChill uses JSON Web Tokens (JWT) to authenticate users.

Users register with their details and log in using their registered email address and password.

After successful authentication, the system generates a JWT token. The token is used when accessing protected API endpoints.

The application also supports role-based authorization to restrict access to certain operations.

Configuration values such as JWT secrets should be stored securely using environment variables and should not be committed to GitHub.

## 7. Running the Application Locally

### Prerequisites

Before running the application, ensure that the following tools are installed:

- Visual Studio
- .NET SDK compatible with the projects
- Docker Desktop
- Docker Compose

### Running with Docker Compose

1. Open a terminal.
2. Navigate to the folder containing the Docker Compose file.
3. Ensure the required environment variables are configured.
4. Run:

```bash
docker compose build
```

5. Start the application:

```bash
docker compose up -d
```

6. Check the running containers:

```bash
docker compose ps
```

### Accessing Swagger

The API Gateway Swagger documentation is available at:

```text
http://localhost:5146/swagger
```

## 8. Docker Deployment

The application is containerised using Docker.

The following Docker images are published to Docker Hub:

- neloo/coffeenchill-functions:v3.0
- neloo/coffeenchill-gateway:v3.0

Docker Compose is used to coordinate the application containers, including the Azure Functions backend, API Gateway and Azurite storage emulator.

To retrieve the published images:

```bash
docker compose pull
```

To start the containers:

```bash
docker compose up -d --no-build
```

## 9. Testing

The application was tested using Swagger and API requests.

Testing covered:

- User registration.
- User login.
- JWT authentication.
- User profile access.
- Menu retrieval.
- Menu creation.
- Menu updating.
- Menu deletion.
- Docker container startup and operation.

Additional authentication and authorization verification should be documented with the final test results.

## 10. Continuous Integration and Continuous Deployment (CI/CD)

The project includes work towards automating the application build and deployment process.

The CI/CD implementation will be documented here once the workflow has been completed and tested.

## 11. Group Contributions

The project was developed collaboratively by two students.

### Student 1
- API Gateway implementation.
- Menu API integration.
- DTO mapping.
- Exception-handling middleware.
- Swagger configuration.
- Docker configuration and deployment.
- Project documentation.

### Student 2
- User authentication implementation.
- User registration and login.
- JWT-related functionality.
- User profile functionality.
- Role-based authorization.
- CI/CD implementation.

## 12. Conclusion

CoffeeNChill demonstrates the development of a cloud-based coffee shop management system using modern .NET technologies, Azure services, API Gateway architecture, authentication and containerisation.

The project demonstrates how different application components can work together to provide a centralised and scalable system.
