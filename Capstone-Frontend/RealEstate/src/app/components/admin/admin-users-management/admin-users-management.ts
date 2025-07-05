import { Component } from '@angular/core';
import { PagedResult } from '../../../models/paged-result.model';
import { StatCard, User, UserQueryParams } from '../../../models/user.model';
import { BehaviorSubject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-users-management',
  imports: [CommonModule,FormsModule,RouterLink],
  templateUrl: './admin-users-management.html',
  styleUrl: './admin-users-management.css'
})
export class AdminUsersManagement {
  users:User[]=[];
    filteredUsers:User[]=[];
    searchTerm: string='';
    isLoading: boolean = false;
    error: string | null = null;
  
    paginationInfo = {
      currentPage: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 1
    };
    math=Math;
  
    stats: StatCard[] = [
      { label: 'Total Users', value: 0, icon: 'bi-people-fill', color: 'primary' },
      { label: 'Agents', value: 0, icon: 'bi-person-badge-fill', color: 'success' },
      { label: 'Buyers', value: 0, icon: 'bi-house-heart-fill', color: 'info' },
      { label: 'Admins', value: 0, icon: 'bi-shield-fill', color: 'warning' }
    ];
  
    private usersSubject = new BehaviorSubject<UserQueryParams>({
      pageNumber: 1,
      pageSize: 10
    });
  
    constructor(private userService:UserService){}
  
    ngOnInit(): void {
      // this.loadStats();
      // this.loadUsers();
      this.usersSubject.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(params => {
          this.isLoading = true;
          return this.userService.getAllUsers(params);
        })
      ).subscribe({
        next:(response)=>{
          this.users=response.items??[],
          this.filteredUsers=[...this.users];
          this.updateStats(this.users);
          this.updatePaginationInfo(response);
          this.isLoading = false;
        }
      });
    }
  
    updatePaginationInfo(response: PagedResult<User>): void {
      this.paginationInfo = {
        currentPage: response.pageNumber,
        pageSize: response.pageSize,
        totalCount: response.totalCount,
        totalPages: Math.ceil(response.totalCount / response.pageSize)
      };
    }
  
  
  
  
    // loadStats(){
    //   this.userService.getStats().subscribe(data=>{
    //     this.stats=[
    //       { label: 'Total Users', value: data.totalUsers },
    //       { label: 'Total Agents', value: data.totalAgents },
    //       { label: 'Total Buyers', value: data.totalBuyers },
    //       { label: 'Total Listings', value: data.totalListings },
    //       { label: 'Total Inquiries', value: data.totalInquiries }
    //     ];
    //   });
    // }
  
    // loadUsers(){
    //   this.userService.getAllUsers().subscribe(users=>{
    //     this.users = users.items;
    //     console.log(this.users);
    //     this.filteredUsers = [...users.items];
    //   });
    // }
  
    loadUsers(){
      this.isLoading = true;
      this.error = null;
  
      this.userService.getAllUsers().subscribe({
        next:(users)=>{
          this.users=users.items;
          console.log(this.users);
          this.filteredUsers=[...users.items];
          this.updateStats(users.items);
          this.isLoading=false;
        },
        error:(err)=>{
          this.error = 'Failed to load users. Please try again later.';
          this.isLoading = false;
          console.error('Error loading users:', err);
        }
      });
    }
  
    updateStats(users: User[]): void {
      this.stats[0].value = users.length;
      this.stats[1].value = users.filter(u => u.role === 'Agent').length;
      this.stats[2].value = users.filter(u => u.role === 'Buyer').length;
      this.stats[3].value = users.filter(u => u.role === 'Admin').length;
    }
  
  
    // filterUsers(){
    //   if (!this.searchTerm) {
    //     this.filteredUsers = [...this.users];
    //     return;
    //   }
  
    //   const term = this.searchTerm.toLowerCase();
    //   this.filteredUsers = this.users.filter(u =>
    //     u.name.toLowerCase().includes(term) ||
    //     u.email.toLowerCase().includes(term) ||
    //     u.role.toLowerCase().includes(term)
    //   );
    // }
  
      filterUsers(): void {
      this.usersSubject.next({
        ...this.usersSubject.value,
        searchTerm: this.searchTerm,
        pageNumber: 1 // Reset to first page when filtering
      });
    }
  
  
  
    deleteUser(userId:string){
      if(confirm('Are you sure you want to delete this user?')){
        this.userService.deleteUser(userId).subscribe({
          next:()=>{
            this.users=this.users.filter(user=>user.id!==userId);
            this.filterUsers();
            this.updateStats(this.users);
          },
          error: (err) => {
            this.error = 'Failed to delete user. Please try again.';
            console.error('Error deleting user:', err);
          }
        });
      }
    }
  
    getRoleBadgeClass(role: string): string {
      switch (role) {
        case 'Admin': return 'bg-danger';
        case 'Agent': return 'bg-success';
        case 'Buyer': return 'bg-primary';
        default: return 'bg-secondary';
      }
    }
  
    goToPage(page: number): void {
      if (page >= 1 && page <= this.paginationInfo.totalPages) {
        this.usersSubject.next({
          ...this.usersSubject.value,
          pageNumber: page
        });
      }
    }
  
    get pagesToShow(): number[] {
      const pages = [];
      const maxPagesToShow = 5;
      let startPage = Math.max(1, this.paginationInfo.currentPage - Math.floor(maxPagesToShow / 2));
      const endPage = Math.min(this.paginationInfo.totalPages, startPage + maxPagesToShow - 1);
      
      // Adjust if we're at the end
      if (endPage - startPage + 1 < maxPagesToShow) {
        startPage = Math.max(1, endPage - maxPagesToShow + 1);
      }
      
      for (let i = startPage; i <= endPage; i++) {
        pages.push(i);
      }
      return pages;
    }
  
  
}
