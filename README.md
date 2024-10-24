# ToDo App

Simple todo application built with .NET 8 minimal API and PostgreSQL providing task management capabilities.

## Features

- Get all todos with pagination
- Get specific todo by id
- Get incoming todos
- Create new todo
- Update existing todo
- Update completion percentage
- Mark todo as done
- Delete todo

## Tech stack

- ASP.NET Core 8 with minimal API
- PostgreSQL
- Entity Framework Core
- Fluent Validation
- xUnit
- Testcontainers

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/)
- [Docker (optional)](https://www.docker.com/)

## Getting started with docker

1. Clone the repository:

```bash
git clone https://github.com/Nilsharr/ToDoApp.git
cd ToDoApp
```

2. Run the application using docker compose:

```bash
docker compose up
```

3. Navigate to http://localhost:5862/swagger/index.html

## Getting started locally

1. Clone the repository:

```bash
git clone https://github.com/Nilsharr/ToDoApp.git
cd ToDoApp
```

2. Set up the database connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ToDoDbConnection": "Host=localhost;Database=todos;Username=your_username;Password=your_password"
  }
}
```

3. Run the application:

```bash
dotnet run --project src/ToDo.API
```

4. Navigate to http://localhost:5234/swagger/index.html

## Tests

### Running Tests

The project uses Testcontainers for integration tests, which requires Docker to be running.

```bash
dotnet test
```

