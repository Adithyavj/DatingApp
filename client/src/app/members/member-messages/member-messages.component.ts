import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  // access the form
  @ViewChild('messageForm') messageForm: NgForm

  @Input() messages: Message[] = [];
  @Input() username: string;
  messageContent: string;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
  }

  // send messages
  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(response => {
      this.messages.push(response);
      this.messageForm.reset();
    })
  }

}
