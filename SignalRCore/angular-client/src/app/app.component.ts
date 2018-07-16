import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  private _hubConnection: HubConnection;
  user = '';
  message = '';
  messages: string[] = [];

  ngOnInit() {
    this.initHub();
  }

  private initHub() {
    const url = 'http://localhost:54173/chathub';

    this._hubConnection = new HubConnectionBuilder()
      .withUrl(`${url}`)
      .configureLogging(LogLevel.Information)
      .build();

    this._hubConnection.start().catch(err => console.error(err.toString()));

    this._hubConnection.on('ReceiveMessage', (message: string) => {
      console.log('ReceiveMessage received');
      console.log(message);
    });
  }

  public sendMessage(): void {
    this._hubConnection
      .invoke('SendMessage', { user: this.user, message: this.message })
      .catch(err => console.error(err));
  }
}
