import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@aspnet/signalr';
import { InstantMessage } from './entities';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
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
    const url = 'http://localhost:50136/instantmessageshub';

    this._hubConnection = new HubConnectionBuilder()
      .withUrl(`${url}`)
      .configureLogging(LogLevel.Information)
      .build();

    this._hubConnection.start().catch(err => console.error(err.toString()));

    this._hubConnection.on('ReceiveMessage', (message: InstantMessage) => {
      this.messages.push(JSON.stringify(message));
    });
  }
}
