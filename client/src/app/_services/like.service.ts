import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { MembersService } from './members.service';

@Injectable({
  providedIn: 'root'
})
export class LikeService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private memberService: MembersService) { }

  // call endpoint to add likes (post)
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  // call endpoint to getlikes
  getLikes(predicate: string, pageNumber, pageSize) {

    let params = this.memberService.getPaginationHeader(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return this.memberService.getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params);
  }

}
