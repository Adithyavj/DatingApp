import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }


  // get all users, pass token (this will be done by jwt interceptor)
  getMembers(userParams: UserParams) {
    let params = this.getPaginationHeader(userParams.pageNumber, userParams.pageSize);

    // adding all required params in query string
    params.append('minAge', userParams.minAge.toString());
    params.append('maxAge', userParams.maxAge.toString());
    params.append('gender', userParams.gender);

    // passing in params also with the get request, so we get full response back and need to get the body from it ourselves
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params);
  }

  // get user by username
  getMember(username: string) {
    const member = this.members.find(x => x.userName === username)
    // if the user is present in the members array, then no need to make api call
    if (member !== undefined) {
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  // update member
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member; // updating the state of members[] in case of an update without using api call
      })
    )
  }

  // to update the main photo
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }


  private getPaginatedResult<T>(url, params) {

    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>(); // store results in this.

    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        // add the response body to paginatedResult body
        paginatedResult.result = response.body;

        // add the response pagination header after JSON deserilizing to paginatedResult pagination
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  // seperate function to get the params.
  getPaginationHeader(pageNumber: number, pageSize: number) {

    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
  }


}
