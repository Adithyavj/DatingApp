import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

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

  // passing value from child to parent component
  // when we click cancel, we need to emit a value using this so that register is hidden/removed 
  @Output() cancelRegister = new EventEmitter();

  constructor() {
    console.log(this.usersFromHomeComponent);
   }

  ngOnInit(): void {
  }

  register(){
    console.log(this.model);
  }

  cancel(){
    // when button is clicked we emit false
    this.cancelRegister.emit(false);
  }

}
