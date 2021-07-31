import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating app';
  users: any;

  // we inject accountService which handles the http requests from api here
  constructor(private accountService: AccountService) { }
  // onInit is a lifecycle event and works after constructor
  ngOnInit() {
    this.setCurrentUser();
  }

  // on loading the app, if there is a logged in user detail in the localstorage, set him as current user else null.
  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));  // get the current user from localstorage and convert it to JSON before using it
    this.accountService.setCurrentUser(user);
  }
}
