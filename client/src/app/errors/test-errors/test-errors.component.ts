import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {

  baseUrl: string = "https://localhost:5001/api/";
  validationErrors: string[] = []; // to store the errors
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  // 404 - content not found error
  get404Error() {
    return this.http.get(this.baseUrl + 'buggy/not-found').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    });
  }

  // 400 - bad request error
  get400Error() {
    return this.http.get(this.baseUrl + 'buggy/bad-request').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    });
  }

  // 500 - internal server error
  get500Error() {
    return this.http.get(this.baseUrl + 'buggy/server-error').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    });
  }

  // 401 - Authorization error
  get401Error() {
    return this.http.get(this.baseUrl + 'buggy/auth').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    });
  }

  // 400 - Validation error
  get400ValidationError() {
    return this.http.post(this.baseUrl + 'account/register', {}).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
      this.validationErrors = error;
    });
  }
}
