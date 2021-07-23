# DatingApp

## The projct will be using the following technologies
- .NET (5.0)
- Angular
- Entity Framework

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


