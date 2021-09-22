import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService) { }

  // method to start a SignalR Presence Hub connection
  createHubConnection(user: User) {
    // Creating a SignalR Hub connection.
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    // Starting the hub connection
    this.hubConnection
      .start()
      .catch(error => console.log(error));

    // Listening for server events - UserIsOnline and UserIsOffline methods.
    this.hubConnection.on("UserIsOnline", username => {
      this.toastr.info(username + ' has connected');
    })

    this.hubConnection.on("UserIsOffline", username => {
      this.toastr.warning(username + ' has disconnected');
    })

    // add onlineUsers to the observable onlineUsersSource.
    this.hubConnection.on("GetOnlineUsers", (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })
  }

  // method to stop a SignalR Presence Hub connection
  stopHubConnection() {
    this.hubConnection
      .stop()
      .catch(error => console.log(error));
  }
}
