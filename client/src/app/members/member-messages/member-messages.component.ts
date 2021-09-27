import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  // access the form
  @ViewChild('messageForm') messageForm: NgForm
  @Input() username: string;
  messageContent: string;


  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
  }

  // send messages
  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    })
  }

}
