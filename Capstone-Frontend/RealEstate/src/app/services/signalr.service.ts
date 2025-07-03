// signalr.service.ts
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection!: signalR.HubConnection;
  private connectionEstablished = new BehaviorSubject<boolean>(false);
  private connectionAttempted = false;

  connectionEstablished$ = this.connectionEstablished.asObservable();

  constructor(private authService:AuthService) {
    this.createConnection();
  }

  private createConnection(): void {
    // const token = localStorage.getItem('accessToken');
    const token = this.authService.getAccessToken();
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5011/hubs/inquiries', {
        // accessTokenFactory: () => token || '',
        accessTokenFactory:()=> token||'',
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.elapsedMilliseconds < 60000) {
            return Math.random() * 5000; // Random delay up to 5 seconds
          }
          return null; // Stop trying to reconnect
        }
      })
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.registerHandlers();
  }

  private registerHandlers(): void {
    this.hubConnection.onclose((error) => {
      console.error('SignalR connection closed:', error);
      this.connectionEstablished.next(false);
    });

    this.hubConnection.onreconnecting((error) => {
      console.log('SignalR reconnecting:', error);
      this.connectionEstablished.next(false);
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.log('SignalR reconnected. Connection ID:', connectionId);
      this.connectionEstablished.next(true);
    });
  }

  public startConnection(): Promise<void> {
    if (this.connectionAttempted) {
      return Promise.resolve();
    }

    this.connectionAttempted = true;
    
    return this.hubConnection.start()
      .then(() => {
        console.log('SignalR connected');
        this.connectionEstablished.next(true);
      })
      .catch(err => {
        console.error('Error connecting to SignalR:', err);
        this.connectionEstablished.next(false);
        throw err;
      });
  }

  public stopConnection(): Promise<void> {
    this.connectionAttempted = false;
    return this.hubConnection.stop();
  }

  public joinInquiryGroup(inquiryId: string): Promise<void> {
    return this.hubConnection.invoke('JoinInquiryGroup', inquiryId);
  }

  public leaveInquiryGroup(inquiryId: string): Promise<void> {
    return this.hubConnection.invoke('RemoveFromGroup', inquiryId);
  }

  public onReceiveReply(handler: (reply: any) => void): void {
    this.hubConnection.on('ReceiveReply', handler);
  }

  public sendReply(inquiryId: string, message: string, authorType: string): Promise<void> {
  return this.hubConnection.invoke('SendReply', inquiryId, message, authorType)
    .catch(err => {
      console.error('Error sending reply:', err);
      throw err; // Re-throw to let component handle it
    });
}
}