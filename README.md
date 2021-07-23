# DatingApp

## The project will be using the following technologies
- .NET (5.0)
- Angular
- Entity Framework
- Sqllite (As development database)

## The programming languages that will be used
- HTML
- BootStrap
- CSS
- TypeScript
- C#

## The coding of both API and FrontEnd will be done using VSCode

## .NET CLI
Basic commands
```dotnet --info```
```dotnet --version```
```dotnet -h``` 
```dotnet new -h```
```dotnet new -l```

To create a new solution using the CLI (name will be same as that of the folder which we are inside)
```dotnet new sln```
To create a new webapi project inside a folder called API
```dotnet new webapi -o API```
To add the project to the solution
```dotnet sln add API/```
To run the API Project
- cd into the API folder and run the command
```dotnet run```

when we run the command dotnet run, we first go inside the program.cs class and search for the Main method. 
This inturn calls the startup.cs class.
ConfigureServices method in startup.cs is like a DI container.
All services to be used across project can be defined here. .NET Core will take care of their creation and destruction.
Configure
configure httprequest pipeline, controller mapping etc

To make live changes to files during run, run using the following command
```dotnet watch run```

## Entity Framework
- An ORM (Object Relational Mapper)
- Translate our code into SQL commands that update our tables in the database.
- It has DbContext class from which we derive our context class to communicate with database.
This class acts as a bridge between our domain(Entity classes) and out database.

### Features of Entity Framework
- Query our database using LINQ (Language Integrated Queries).
- Change tracking. (Keep track of changes occuring).
- It allows to insert, update and delete from DB.
- Gives us a SaveChanges method to save these changes to the database.
- Concurrency.
- Provides automatic transaction management.
- Caching - Repeated quering return data from cache.
- Build-in conventions
- Configure entities to override the conventions.
- Migrations - create a db schema.

## Installing Entity Framework package
- Install Microsoft.EntityFrameworkCore.Sqlite

