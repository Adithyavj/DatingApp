import { Component, OnInit } from '@angular/core';
import { observable, Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  // TODO Change this code and initialize this to null initially
  currentUser$: Observable<User>=new Observable;

  // inject accountservice for http request
  constructor(private accountService: AccountService) {
    
  }

  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
  }

  login() {
    // this returns an observable so we need to subscribe to listen to it
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    });
  }

  logout() {
    this.accountService.logout();
  }

}
