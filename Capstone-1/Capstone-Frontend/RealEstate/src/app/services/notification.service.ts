import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject, from, throwError } from 'rxjs';
import { catchError, takeUntil, tap, switchMap } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: 'property' | 'inquiry' | 'system';
  timestamp: Date;
  isRead: boolean;
  data?: any;
}

export interface PropertyListingNotification {
  title: string;
  listingId: string;
  location: string;
  price: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService implements OnDestroy {
  private hubConnection: signalR.HubConnection | null = null;
  private destroy$ = new Subject<void>();
  
  public notificationsSubject = new BehaviorSubject<Notification[]>([]);
  private unreadCountSubject = new BehaviorSubject<number>(0);
  private connectionStatusSubject = new BehaviorSubject<boolean>(false);

  public notifications$ = this.notificationsSubject.asObservable();
  public unreadCount$ = this.unreadCountSubject.asObservable();
  public connectionStatus$ = this.connectionStatusSubject.asObservable();

  constructor(private authService: AuthService) {
    this.loadNotificationsFromStorage();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.stopConnection().subscribe();
  }

  startConnection(): Observable<void> {
    if (this.hubConnection && 
        (this.hubConnection.state === signalR.HubConnectionState.Connected || 
         this.hubConnection.state === signalR.HubConnectionState.Connecting)) {
      return from(Promise.resolve());
    }

    // If connection exists but is in a different state, stop it first
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      return from(this.hubConnection.stop()).pipe(
        catchError(() => from(Promise.resolve())), // Ignore stop errors
        switchMap(() => this.createNewConnection())
      );
    }

    return this.createNewConnection();
  }

  private createNewConnection(): Observable<void> {
    // Update this URL to match your backend
    const hubUrl = 'http://localhost:5011/hubs/notifications';
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => {
          const token = this.authService.getAccessToken();
          return token || '';
        }
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          if (retryContext.elapsedMilliseconds < 60000) {
            return Math.random() * 10000;
          } else {
            return null;
          }
        }
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.setupEventListeners();

    return from(this.hubConnection.start()).pipe(
      tap(() => {
        this.connectionStatusSubject.next(true);
        console.log('Notification SignalR Connected');
      }),
      catchError(error => {
        console.error('SignalR connection error:', error);
        this.connectionStatusSubject.next(false);
        return throwError(() => error);
      })
    );
  }

  stopConnection(): Observable<void> {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      return from(this.hubConnection.stop()).pipe(
        tap(() => {
          this.connectionStatusSubject.next(false);
          console.log('Notification SignalR Disconnected');
        }),
        catchError(error => {
          console.error('Error stopping SignalR connection:', error);
          return throwError(() => error);
        })
      );
    }
    return from(Promise.resolve());
  }

  private setupEventListeners(): void {
    if (!this.hubConnection) return;

    this.hubConnection.onclose(() => {
      this.connectionStatusSubject.next(false);
      console.log('SignalR connection closed');
    });

    this.hubConnection.onreconnecting(() => {
      this.connectionStatusSubject.next(false);
      console.log('SignalR reconnecting...');
    });

    this.hubConnection.onreconnected(() => {
      this.connectionStatusSubject.next(true);
      console.log('Notification SignalR reconnected');
    });

    // Listen for property listing notifications
    this.hubConnection.on('ReceiveListingNotification', (data: PropertyListingNotification) => {
      const notification: Notification = {
        id: this.generateId(),
        title: 'New Property Listed!',
        message: `${data.title} in ${data.location} - ₹${data.price.toLocaleString()}`,
        type: 'property',
        timestamp: new Date(),
        isRead: false,
        data: { 
          listingId: data.listingId,
          title: data.title,
          location: data.location,
          price: data.price
        }
      };

      this.addNotification(notification);
      this.showBrowserNotification(notification);
    });

    // Listen for general notifications
    this.hubConnection.on('ReceiveNotification', (data: any) => {
      const notification: Notification = {
        id: this.generateId(),
        title: data.title || 'New Notification',
        message: data.message || '',
        type: data.type || 'system',
        timestamp: new Date(),
        isRead: false,
        data: {
          ...data,
          // Ensure we preserve any IDs for routing
          listingId: data.listingId,
          inquiryId: data.inquiryId
        }
      };

      this.addNotification(notification);
      this.showBrowserNotification(notification);
    });

    this.hubConnection.start().then(() => {
      this.connectionStatusSubject.next(true);
      console.log('Notification SignalR Connected');
    }).catch(err => {
      this.connectionStatusSubject.next(false);
      console.error('Notification SignalR Connection Error:', err);
    });
  }

  private addNotification(notification: Notification): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = [notification, ...currentNotifications];
    
    // Keep only last 50 notifications
    if (updatedNotifications.length > 50) {
      updatedNotifications.splice(50);
    }

    this.notificationsSubject.next(updatedNotifications);
    this.updateUnreadCount();
    this.saveNotificationsToStorage();
  }

  private showBrowserNotification(notification: Notification): void {
    if ('Notification' in window && Notification.permission === 'granted') {
      new Notification(notification.title, {
        body: notification.message,
        icon: '/assets/favicon.ico'
      });
    }
  }

  requestNotificationPermission(): void {
    if ('Notification' in window && Notification.permission === 'default') {
      Notification.requestPermission();
    }
  }

  markAsRead(notificationId: string): void {
    const notifications = this.notificationsSubject.value;
    const updated = notifications.map(n => 
      n.id === notificationId ? { ...n, isRead: true } : n
    );
    
    this.notificationsSubject.next(updated);
    this.updateUnreadCount();
    this.saveNotificationsToStorage();
  }

  markAllAsRead(): void {
    const notifications = this.notificationsSubject.value;
    const updated = notifications.map(n => ({ ...n, isRead: true }));
    
    this.notificationsSubject.next(updated);
    this.updateUnreadCount();
    this.saveNotificationsToStorage();
  }

  clearNotification(notificationId: string): void {
    const notifications = this.notificationsSubject.value;
    const updated = notifications.filter(n => n.id !== notificationId);
    
    this.notificationsSubject.next(updated);
    this.updateUnreadCount();
    this.saveNotificationsToStorage();
  }

  clearAllNotifications(): void {
    this.notificationsSubject.next([]);
    this.unreadCountSubject.next(0);
    this.saveNotificationsToStorage();
  }

  private updateUnreadCount(): void {
    const unreadCount = this.notificationsSubject.value.filter(n => !n.isRead).length;
    this.unreadCountSubject.next(unreadCount);
  }

  private generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }

  private saveNotificationsToStorage(): void {
    try {
      const notifications = this.notificationsSubject.value;
      localStorage.setItem('notifications', JSON.stringify(notifications));
    } catch (error) {
      console.error('Failed to save notifications to storage:', error);
    }
  }

  private loadNotificationsFromStorage(): void {
    try {
      const stored = localStorage.getItem('notifications');
      if (stored) {
        const notifications: Notification[] = JSON.parse(stored);
        // Convert timestamp strings back to Date objects
        const convertedNotifications = notifications.map(n => ({
          ...n,
          timestamp: new Date(n.timestamp)
        }));
        
        this.notificationsSubject.next(convertedNotifications);
        this.updateUnreadCount();
      }
    } catch (error) {
      console.error('Failed to load notifications from storage:', error);
    }
  }

  // Test method to simulate notifications
  sendTestNotification(): void {
    const testNotification: Notification = {
      id: this.generateId(),
      title: 'Test Notification',
      message: 'This is a test notification to verify the system works',
      type: 'system',
      timestamp: new Date(),
      isRead: false,
      data: {}
    };

    this.addNotification(testNotification);
  }
}