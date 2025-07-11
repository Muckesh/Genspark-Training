import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { NotificationService, Notification } from '../../../services/notification.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule],
  templateUrl: './notifications.html',
  styleUrls: ['./notifications.css']
})
export class Notifications implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  notificationService = inject(NotificationService);
  authService = inject(AuthService);
  router = inject(Router);

  notifications$ = this.notificationService.notifications$;
  unreadCount$ = this.notificationService.unreadCount$;
  connectionStatus$ = this.notificationService.connectionStatus$;

  filteredNotifications: Notification[] = [];
  selectedFilter: 'all' | 'unread' | 'property' | 'inquiry' | 'system' = 'all';
  searchTerm: string = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  ngOnInit(): void {
    this.notifications$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(notifications => {
      this.applyFilters(notifications);
    });

    // Request browser notification permission
    this.notificationService.requestNotificationPermission();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  applyFilters(notifications: Notification[]): void {
    let filtered = notifications;

    // Apply type filter
    if (this.selectedFilter !== 'all') {
      if (this.selectedFilter === 'unread') {
        filtered = filtered.filter(n => !n.isRead);
      } else {
        filtered = filtered.filter(n => n.type === this.selectedFilter);
      }
    }

    // Apply search filter
    if (this.searchTerm.trim()) {
      const searchLower = this.searchTerm.toLowerCase();
      filtered = filtered.filter(n => 
        n.title.toLowerCase().includes(searchLower) ||
        n.message.toLowerCase().includes(searchLower)
      );
    }

    // Apply pagination
    this.totalPages = Math.ceil(filtered.length / this.pageSize);
    const startIndex = (this.currentPage - 1) * this.pageSize;
    this.filteredNotifications = filtered.slice(startIndex, startIndex + this.pageSize);
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.notifications$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(notifications => {
      this.applyFilters(notifications);
    });
  }

  onSearchChange(): void {
    this.currentPage = 1;
    this.notifications$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(notifications => {
      this.applyFilters(notifications);
    });
  }

  markAsRead(notificationId: string): void {
    this.notificationService.markAsRead(notificationId);
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead();
  }

  clearNotification(notificationId: string): void {
    this.notificationService.clearNotification(notificationId);
  }

  clearAllNotifications(): void {
    if (confirm('Are you sure you want to clear all notifications?')) {
      this.notificationService.clearAllNotifications();
    }
  }

  sendTestNotification(): void {
    this.notificationService.sendTestNotification();
  }

  viewProperty(listingId: string): void {
    this.router.navigate(['/buyer/property', listingId]);
  }

  viewInquiry(inquiryId: string): void {
    this.router.navigate(['/inquiries', inquiryId]);
  }

  onNotificationClick(notification: Notification): void {
    // Mark as read when clicked
    if (!notification.isRead) {
      this.markAsRead(notification.id);
    }

    // Navigate based on notification type and data
    if (notification.type === 'property' && notification.data?.listingId) {
      this.router.navigate(['/buyer/property', notification.data.listingId]);
    } else if (notification.type === 'inquiry' && notification.data?.inquiryId) {
      this.router.navigate(['/inquiries', notification.data.inquiryId]);
    } else if (notification.type === 'system') {
      // Handle system notifications - maybe navigate to settings or dashboard
      console.log('System notification clicked:', notification);
    }
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

  getNotificationClass(type: string): string {
    switch (type) {
      case 'property':
        return 'text-primary';
      case 'inquiry':
        return 'text-info';
      case 'system':
        return 'text-warning';
      default:
        return 'text-secondary';
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.notifications$.pipe(
        takeUntil(this.destroy$)
      ).subscribe(notifications => {
        this.applyFilters(notifications);
      });
    }
  }

  // Add trackBy function for better performance
  trackByNotificationId(index: number, notification: any): string {
    return notification.id;
  }

  getPaginationArray(): number[] {
    const pages = [];
    const maxVisible = 5;
    
    if (this.totalPages <= maxVisible) {
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      const start = Math.max(1, this.currentPage - 2);
      const end = Math.min(this.totalPages, start + maxVisible - 1);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
    }
    
    return pages;
  }
}