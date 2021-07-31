import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  // passing values from parent component to child (here home to register)
  // in this case this input() will get the users from home
  @Input() usersFromHomeComponent: any;

  constructor() {
    console.log(this.usersFromHomeComponent);
   }

  ngOnInit(): void {
  }

  register(){
    console.log(this.model);
  }

  cancel(){
    console.log('cancelled');
  }

}
