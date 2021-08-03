import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

// adding header to http request (here we are adding bearer token)
const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user')).token
  })
}
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {
    console.log(httpOptions);
  }

  // get all users, pass token
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users', httpOptions);
  }

  // get user by username
  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username, httpOptions);
  }
}
