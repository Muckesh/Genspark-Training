import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class Sidebar {
  user!: User;
  role!: string;
  isCollapsed = false;
  activeSubmenu: string | null = null;
  // Add this inside your Sidebar component class
  @Output() sidebarToggled = new EventEmitter<boolean>(); // Renamed the output



  constructor(private authService: AuthService) {
    this.user = authService.getCurrentUser();
    this.role = this.user.role;
  }

  buyerMenuItems = [
    { path: '/buyer/dashboard', icon: 'bi-speedometer2', title: 'Dashboard' },
    { path: '/buyer/browse-listings', icon: 'bi-house', title: 'Listings' },
    { path: '/buyer/my-inquiries', icon: 'bi-chat-left-text', title: 'My Inquiries' },
    { path: '/buyer/profile', icon: 'bi-person', title: 'Profile' }
  ];

  agentMenuItems = [
    { path: '/agent/dashboard', icon: 'bi-speedometer2', title: 'Dashboard' },
    { path: '/agent/listings', icon: 'bi-house', title: 'My Listings' },
    { path: '/agent/create', icon: 'bi-plus-circle', title: 'Create Listings' },
    { path: '/agent/inquiries', icon: 'bi-chat', title: 'Inquiries' },
    { path: '/agent/browse-listings', icon: 'bi-house', title: 'All Listings' },
    { path: '/agent/profile', icon: 'bi-person', title: 'Profile' }
  ];

  adminMenuItems = [
    { path: '/admin/dashboard', icon: 'bi-speedometer2', title: 'Dashboard' },
    { path: '/admin/users', icon: 'bi-people', title: 'User Management' },
    { path: '/admin/listings', icon: 'bi-house', title: 'Listings Management' },
    { path: '/admin/image-cleanup', icon: 'bi-image', title: 'Image Cleanup' },
    { path: '/admin/browse-listings', icon: 'bi-house', title: 'All Listings' },
    { path: '/admin/profile', icon: 'bi-person', title: 'Profile' }
  ];

  // Modify your toggleCollapse method to emit the event
  toggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
    this.sidebarToggled.emit(this.isCollapsed);
  }

  toggleSubmenu(menu: string) {
    this.activeSubmenu = this.activeSubmenu === menu ? null : menu;
  }

  logout() {
    this.authService.logout();
  }
}