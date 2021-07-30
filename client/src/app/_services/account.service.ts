import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = 'https://localhost:5001/api/';
  // observable to store the user in
  private currentUserSource = new ReplaySubject<User>(1); // buffer of size 1
  currentUser$ = this.currentUserSource.asObservable(); // observable $ sign specifies it's an observable

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

  // helper method
  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user'); // remove user from localstorage
    this.currentUserSource.next(null);
  }
}
