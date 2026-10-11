# CoffeeNChill

## Project Description

offeeNChill is a cloud-based coffee shop management application developed using Azure Functions and Azure Storage services. The application provides menu management, staff document management, and order processing through HTTP-triggered and queue-triggered Azure Functions.

The application uses Azure Table Storage to store menu items and orders, Azure Blob Storage to manage staff documents, and Azure Queue Storage to process customer orders asynchronously. Docker and Docker Compose are used to run the application and its storage services locally.

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

### Order Processing

The Order Processing API provides:

-Place customer orders through an HTTP endpoint.
-Add orders to the Azure Storage Queue.
-Process queued orders using an Azure Queue-triggered function.
-Store order information in Azure Table Storage.
-Update order statuses as they progress through the processing workflow.

Order statuses include:
-Received
-Preparing
-Ready
-Collected


## Local Setup

### 1. Clone the Repository

Clone the CoffeeNChill repository from GitHub and open the project in Visual Studio.

```bash
git clone https://github.com/SeNeloo/CoffeeNChill---Cloud-development-.git

### 1. Clone the Repository

Clone the CoffeeNChill repository from GitHub and open the project in Visual Studio.

### 2. Run Azurite

Start Azurite locally to emulate Azure Storage services.

```bash
azurite

## Testing

The application was tested using Postman and Docker.

Testing included:

-Creating and retrieving menu items.
-Retrieving menu items by category.
-Updating and deleting menu items.
-Uploading, retrieving, and downloading staff documents.
-Submitting customer orders to the processing queue.
-Processing orders and updating their statuses.
