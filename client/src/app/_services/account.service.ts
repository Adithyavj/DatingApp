import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root' // specifies it is provided in the root. No need for us to add it in module.ts
})
export class AccountService {

  baseUrl = environment.apiUrl;
  // observable to store the user coming back from api after login method
  private currentUserSource = new ReplaySubject<User>(1); // buffer of size 1
  currentUser$ = this.currentUserSource.asObservable(); // observable $ sign specifies it's an observable
  // currentUser$ can have only 2 values - null or User


  // inject httpclient to ctor
  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      // once a user registers, we consider them to be logged in
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    )
  }

  // helper method
  setCurrentUser(user: User) {

    // set user roles
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role; // get the role from the token payload
    // if user has more than one role, it returns a role [] else a string
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user)); // setting user in localstorage using the key 'user' (we convert the object into string before sending it to server)
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user'); // remove user from localstorage
    this.currentUserSource.next(null);
  }

  getDecodedToken(token) {
    // atob - The atob() method decodes a base-64 encoded string.
    // decode the 2nd part of the token - PayLoad
    return JSON.parse(atob(token.split('.')[1]));
  }
}
