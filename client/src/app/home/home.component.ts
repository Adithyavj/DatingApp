import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode: boolean = false;
  users: any;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getUsers();
  }

  // change register mode to opposite (toggle between the modes)
  registerToggle() {
    this.registerMode = !this.registerMode;
  }


  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(users=>{
      this.users = users; // assign the users that come back to this.users method here
    })
  }

}
