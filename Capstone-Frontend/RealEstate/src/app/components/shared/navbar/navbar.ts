import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';
import { Notifications } from '../notifications/notifications';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule,Notifications],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar implements OnInit, OnDestroy {
  authService = inject(AuthService);
  notificationService = inject(NotificationService);
  router = inject(Router);
  
  private destroy$ = new Subject<void>();

  notifications$ = this.notificationService.notifications$;
  unreadCount$ = this.notificationService.unreadCount$;
  connectionStatus$ = this.notificationService.connectionStatus$;

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      // Add a small delay to ensure other services are initialized
      setTimeout(() => {
        this.notificationService.startConnection()
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              console.log('Notification service connected');
              // Request notification permission
              this.notificationService.requestNotificationPermission();
            },
            error: (error) => console.error('Failed to connect notification service:', error)
          });
      }, 1000);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    
    this.notificationService.stopConnection()
      .pipe(takeUntil(this.destroy$))
      .subscribe();
  }

  markAsRead(notificationId: string): void {
    this.notificationService.markAsRead(notificationId);
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead();
  }

  viewProperty(listingId: string): void {
    this.router.navigate(['/buyer/property', listingId]);
  }

  viewInquiry(inquiryId: string): void {
    this.router.navigate(['/inquiries', inquiryId]);
  }

  viewAllNotifications(): void {
    // The modal will open automatically due to data-bs-target
  }

  sendTestNotification(): void {
    this.notificationService.sendTestNotification();
  }

  getRecentNotifications() {
    const notifications = this.notificationService.notificationsSubject?.value || [];
    return notifications.slice(0, 5); // Show only the 5 most recent
  }

  getNotificationIcon(type: string): string {
    switch (type) {
      case 'property':
        return 'bi bi-house-door';
      case 'inquiry':
        return 'bi bi-chat-dots';
      case 'system':
        return 'bi bi-info-circle';
      default:
        return 'bi bi-bell';
    }
  }

  logout(): void {
    this.authService.logout();
  }

  // Add trackBy function for better performance
  trackByNotificationId(index: number, notification: any): string {
    return notification.id;
  }
}