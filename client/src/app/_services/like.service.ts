import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class LikeService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private accountService: AccountService) { }

  // call endpoint to add likes (post)
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  // call endpoint to getlikes
  getLikes(predicate: string) {
    return this.http.get(this.baseUrl + 'likes?=' + predicate);
  }

}