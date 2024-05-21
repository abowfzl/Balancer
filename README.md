# Balancer (A Clean Architecture .NET Web API Application)

Welcome to the Balancer! This project is designed to provide a robust and maintainable solution for managing user accounts, transactions, configuration, background services and unit tests using the Clean Architecture pattern.

## Table of Contents

1. [Introduction](#introduction)
2. [Background](#background)
3. [Features](#features)
4. [Technologies Used](#technologies-used)
5. [Structure](#structure)
6. [Getting Started](#getting-started)
7. [Testing](#testing)
8. [Contributing](#contributing)
9. [License](#license)

## Introduction

This .NET web API application is built using the principles of Clean Architecture, which emphasizes separation of concerns, testability, and maintainability. It provides a flexible and scalable solution for managing user accounts, transactions, and configurations.

## Background

Managing user accounts and transactions is a common requirement for many applications. In this project, I focus on implementing functionalities to handle user account charges, including inserting, withdrawing, and injecting money transactions. Additionally, CRUD APIs are provided for managing accounts and configurations. A key aspect of this project is ensuring the security and integrity of user data while maintaining a high level of performance.

## Features

- **Clean Architecture**: The project follows a layered architecture, consisting of Presentation, Application, Domain, and Infrastructure layers, promoting separation of concerns and maintainability.
- **Web API**: Utilizes ASP.NET Core to expose RESTful APIs for communication, ensuring compatibility and scalability.
- **Account Management**: CRUD APIs for managing user accounts, allowing creation, retrieval, update, and deletion of accounts.
- **Transaction Handling**: APIs for inserting, withdrawing, and depositing transactions to manage account balances securely.
- **Configuration Management**: APIs to manage application configurations, providing flexibility in customization.
- **Account Status Retrieval**: API to retrieve the status of a user account, enabling real-time monitoring and analysis.

## Technologies Used

- **ASP.NET Core**: Framework for building cross-platform web APIs, offering high performance and scalability.
- **Entity Framework Core**: Object-relational mapping (ORM) framework for database interaction, simplifying data access and manipulation.
- **Swagger**: Tool for documenting and testing APIs, enhancing API discoverability and developer experience.
- **Dependency Injection**: Utilized for managing dependencies and promoting loose coupling, facilitating easier maintenance and testing.
- **Unit Testing**: Xunit and Moq for unit testing components, ensuring code quality and reliability.

## Structure
```
clean-architecture-dotnet/
│
├── src/
│   ├── Application/
│   │   ├── Interfaces/
│   │   │   ├── IAccountService.cs
│   │   │   ├── ITransactionService.cs
│   │   │   └── ...
│   │   ├── Services/
│   │   │   ├── AccountService.cs
│   │   │   ├── TransactionService.cs
│   │   │   └── ...
│   │   └── ...
│   │
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── Account.cs
│   │   │   ├── Transaction.cs
│   │   │   └── ...
│   │   ├── Interfaces/
│   │   │   ├── IAccountRepository.cs
│   │   │   ├── ITransactionRepository.cs
│   │   │   └── ...
│   │   └── ...
│   │
│   ├── Infrastructure/
│   │   ├── Data/
│   │   │   ├── Repositories/
│   │   │   │   ├── AccountRepository.cs
│   │   │   │   ├── TransactionRepository.cs
│   │   │   │   └── ...
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── ...
│   │   └── ...
│   │
│   ├── Presentation/
│   │   ├── Controllers/
│   │   │   ├── AccountController.cs
│   │   │   ├── TransactionController.cs
│   │   │   └── ...
│   │   └── ...
│   │
│   └── ...
│
├── tests/
│   ├── Application.Tests/
│   ├── Domain.Tests/
│   ├── Infrastructure.Tests/
│   └── ...
│
├── README.md
├── LICENSE
└── ...

```

## Getting Started

**Prerequisites**: Ensure you have .NET Core SDK installed on your machine.
- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)
  
### Environment Variables
Create a .env file in the root of the project and add any necessary environment variables. Here is an example:
```
# .env file example
DATABASE_URL=postgres://user:password@db:5432/mydatabase
SECRET_KEY=your_secret_key

```
### Build and Run the Containers
To build and run the Docker containers, use the following command:
```
docker-compose up --build
```
This will build the Docker images (if they are not already built) and start the containers defined in the docker-compose.yml file.
### Accessing the Application
Once the containers are up and running, you can access the application at:
```
Accessing the Application
Once the containers are up and running, you can access the application at:
```
## Testing

The project includes unit tests to ensure code quality and reliability. To run the tests, execute the following command:

```bash
dotnet test
```

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvement, please submit a pull request or open an issue on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
