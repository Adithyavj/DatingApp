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

## Installing dotnet ef tools
```dotnet tool install --global dotnet-ef --version 5.0.8```

## Creating Migrations
To create a migration and place it in the folder Data/Migrations, run the following command
```dotnet ef migrations add InitialCreate -o Data/Migrations```


## WebAPI
We make all the API calls asynchronous to make the app more scalable. If the no. of users increases and the app is handling 
the api calls synchronously, then the thread handling one request will be blocked and this reduces scalability.
So we use asynchronous methods

## Angular
To Install Angular globally,
```npm install -g @angular/cli```

- When running angular app using ```ng serve```, it compiles the ts files to js and serves them from memory.
- The js files that are compiled during runtime are injected to the index.html file using a utility called
webpack. This is done by the Angular CLI.
- The main.ts file bootstraps AppModule and inside the AppModule we bootstrap AppComponent.
AppComponent is declared as a selector inside index.html.

### Some good VSCode extensions for Angular
- Angular Language Service
- Angular Snippets (Version 12)
- Bracket Pair Colorizer 2

In Angular we communicate with the API using HttpClientModule.
we inject the http module in the constructor of app.component.ts using DI
http methods are asynchronous so we use them in the ngOnInit() lifecycle event which works
right after constructor.

## CORS (Cross Origin Resource Sharing)
It is a security mechanism built into all mordern web browsers.
It block the http requests from our front end (Angular app) to any api that is not
in the same origin and local host of webapi is 5001, front end is 4200.
We cannot access the api unless it provides a header saying that it's ok.

So we have to allow cors in .net api startup.cs and provide the origin of angular app.

## Adding Angular BootStrap package
[Angular BootStrap](https://valor-software.com)
Run the following command
```ng add ngx-bootstrap```

## Adding Font awesome
```npm install font-awesome```


## Storing password
Passwords can be saved in the database in 3 ways.
1. Clear text
2. Hashing password (use a hashing algorithm to encrypt the password before sending it to DB)
3. Hashing and salting password ()
We are using 3rd one.
Sqlite stored byte[] as BLOB - (Binary Large Objects)
