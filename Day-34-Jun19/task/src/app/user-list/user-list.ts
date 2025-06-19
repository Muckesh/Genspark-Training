import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { BehaviorSubject, combineLatest, debounceTime, map } from 'rxjs';
import { User } from '../models/user.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserList implements OnInit {
  private userService = inject(UserService);

  searchSubject = new BehaviorSubject<string>('');
  roleSubject = new BehaviorSubject<string>('All');

  roles = ['All', 'Admin', 'User', 'Guest'];
  filteredUsers: User[] = [];

  searchTerm = '';
  selectedRole = 'All';

  ngOnInit(): void {
    combineLatest([
      this.searchSubject.pipe(debounceTime(400)),
      this.roleSubject,
      this.userService.users$
    ])
    .pipe(
      map(([search, role, users]) =>
        users.filter(user =>
          user.username.toLowerCase().includes(search.toLowerCase()) &&
          (role === 'All' || user.role === role)
        )
      )
    )
    .subscribe(filtered => this.filteredUsers = filtered);
  }

  onSearchChange() {
    this.searchSubject.next(this.searchTerm);
  }

  onRoleChange() {
    this.roleSubject.next(this.selectedRole);
  }
}
