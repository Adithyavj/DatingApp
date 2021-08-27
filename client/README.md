# Client

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 12.1.1.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.


## Create a component without test file
```ng g c <name> --skip-tests```


## Create a service without test file
```ng g s <name> --skip-tests```

## Angular Concepts:
How do we tell a form that it is an Angular Form?
For this we can use the template reference variable using the # followed by the formName and assign it as an ngForm
Template reference variable - #<formname>="ngForm"

Angular allows 2 way binding between properties and html
()- binds the html to ts (used when data goes from template(html) to the component)
[]-binds the ts to html (used for recieving date from our component to template(html))
[()] -2 way binding between template and component

for using 2 way binding in an input, we need to 
- give it a name
- [(ngModel)]="binding" (we assign ngModel to the property in the ts file that it is bound to)


## Service
It has an @Injectable decorator - the service can be injected to other services or components.
Angular service is a Singleton - When we goto a component and initialize the service, it will stay initialized till the 
app is disposed of.
Services are :
- Injectable
- Singleton - data stored inside service doesn't get destroyed until the app is closed.

## Structural directive
They modify the dom
*ngIf
*ngFor

## Directives
simple text inside html tag 
eg:- 
```
<div class="btn-group" dropdown> <!-- here dropdown is a directive -->

```
## Observables
New standard for managing async data. Included in ES7.
Observables are lazy collections of multiple values over time.
We use them for http requests and when components need to observe a value
You can think of it as a newsletter
 - only subscribers of the newsletter recieve it.
 In js, we use promise to handle async code.

 ### Promise vs Observables
 1. Provides a single future value          1. Emits multiple values over time
 2. Are not Lazy                            2. Are Lazy
 3. We cannot cancel it                     3. Able to cancel because they use streams of data
                                            4. can be used with map,filter,reduce and other operators. (using RxJS)

We have to subscribe to an obervable to do something

## RxJS (Reactive extensions for javascript)
They work with observables. We can transform data before subscribing to an observable by adding a pipe() method on the observable
```
    getMembers(){
        return this.http.get('api/users').pipe( // pipe is a method in RxJs we can chain as many methods as we want inside it
            map(members => {
                console.log(member.id)
                return member.id
            })
        )
    }
```

***Observable Signature:***
```
   getMembers(){
       this.service.getMembers().subscribe(members => { // what to do next
           this.members = members
       }, error =>{ //what to do if there is an error
           console.log(error);
       }, ()=>{ // when subscription is complete
           console.log('completed);
       })
   } 
```

We can get data from observables by 
1. Subscribe
2. Promise()
3. Async pipe - automatically subscribes/unsubscribes from the observable



## To add notification toaster
```npm install ngx-toastr```

## Creating guards
```ng g guard <name>```

## Install bootswatch (Bootstrap theme)
```npm install bootswatch```
add the theme you want to angular.json styles[]
eg:-
```    
    "styles": [
              "./node_modules/font-awesome/css/font-awesome.css",
            ],
    // Add it beneath the bootstrap.min.css
```

## We can tidy up our module.ts file by moving some of the code to a new module
creating a new module file:
```
    ng g m <name> --flat // flat is used so that it doesn't create a folder to store the module inside it
```

## Error Handling (Interceptors)
We handle the errors globally in Angular using http interceptors
```
    ng g interceptor <name>
```
We also send the token along with the http headers in all requests, this is done using interceptor (jwt interceptor)

## Installing photo gallery
[Source Page: npmjs](https://www.npmjs.com/package/@kolkov/ngx-gallery)
```
    npm install @kolkov/ngx-gallery
```

## Installing spinner
[Source Page: npmjs](https://www.npmjs.com/package/ngx-spinner)
```
    npm install @angular/cdk
    ng add ngx-spinner
```

## State management
Redux/Mobex state management, but angular has services that can manage states


## REST API
Representational State Transfer is a software architecture style that defines a set of constraints to be used
when creating web servies.

## Storing images
Options available:
- We can store it in the Database as Binary Large Objects (BLOB)
- Use file systems on server to store and return the images.
- Using a cloud service. (Cloudinary)

Here, we use Cloudinary:-
1. Client uploads photo to API with JWT 
2. Server uploads photo to cloudinary
3. Cloudinary store the photo, sends response
4. API saves photo URL and Public ID to DB
5. Saved in DB and gives auto generated ID
6. 201 response created and send to client with location in header

## For file uploading
we use [ng2-file-upload](https://valor-software.com/ng2-file-upload/)
```
    npm install ng2-file-upload
```

## For Last active we can use timeago module
we user ngx-timeago
```
    npm i ngx-timeago
```

## Route Resolvers
Allow to Get access to data before component is created.