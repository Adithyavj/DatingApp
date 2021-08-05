import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // get all users, pass token (this will be done by jwt interceptor)
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  // get user by username
  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  // update member
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member);
  }
}
