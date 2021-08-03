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
          localStorage.setItem('user', JSON.stringify(user)); // setting user in localstorage using the key 'user' (we convert the object into string before sending it to server)
          this.currentUserSource.next(user);
        }
      })
    )
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      // once a user registers, we consider them to be logged in
      map((user: User) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        return user;
      })
    )
  }



  // helper method
  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user'); // remove user from localstorage
    this.currentUserSource.next(null);
  }
}
