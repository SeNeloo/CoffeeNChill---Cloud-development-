# CoffeeNChill

## Project Description

CoffeeNChill is a cloud-based coffee shop management application developed using Azure Functions and Azure Storage services. The application provides menu management and staff document management through HTTP-triggered APIs.

## Technologies Used

- .NET 10
- Azure Functions v4 (Isolated Worker)
- Azure Table Storage
- Azure Blob Storage
- Azurite
- Docker
- Docker Hub
- Postman
- GitHub

## Features

### Menu Management

The Menu API provides:

- Create a menu item
- Retrieve all menu items
- Retrieve menu items by category
- Update a menu item
- Delete a menu item

### Staff Documents

The Staff Documents API provides:

- Upload staff documents
- Retrieve a list of staff documents
- Download staff documents

## Local Setup

### 1. Clone the Repository

Clone the CoffeeNChill repository from GitHub and open the project in Visual Studio.

### 2. Run Azurite

Start Azurite locally to emulate Azure Storage services.

```bash
azurite