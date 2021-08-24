import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMessage(pageNumber, pageSize, container) {

    let params = getPaginationHeader(pageNumber, pageSize);
    params = params.append('container', container);

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }
}
