import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
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

  constructor(private toastr: ToastrService, private router: Router) { }

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
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames, username]);
      })
    })

    this.hubConnection.on("UserIsOffline", username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames.filter(x => x !== username)]);
      })
    })

    // add onlineUsers to the observable onlineUsersSource.
    this.hubConnection.on("GetOnlineUsers", (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })

    this.hubConnection.on("NewMessageRecieved", ({ userName, knownAs }) => {
      this.toastr.info(knownAs + ' has send you a new message!')
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + userName + '?tab=3'));
    })
  }

  // method to stop a SignalR Presence Hub connection
  stopHubConnection() {
    this.hubConnection
      .stop()
      .catch(error => console.log(error));
  }
}
