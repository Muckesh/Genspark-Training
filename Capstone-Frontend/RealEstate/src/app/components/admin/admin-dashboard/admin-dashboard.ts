import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { FormsModule } from '@angular/forms';
import { StatCard, User, UserQueryParams } from '../../../models/user.model';
import { BehaviorSubject, debounceTime, distinctUntilChanged, forkJoin, switchMap } from 'rxjs';
import { PagedResult } from '../../../models/paged-result.model';
import { InquiryService } from '../../../services/inquiry.service';
import { PropertyListingService } from '../../../services/property-listing.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule,FormsModule,DatePipe,RouterLink],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {
  private userService = inject(UserService);
  private listingService = inject(PropertyListingService);
  private inquiryService = inject(InquiryService);
  
  // stats:any[]=[];
  users:User[]=[];
  filteredUsers:User[]=[];
  searchTerm: string='';
  isLoading: boolean = false;
  isStatsLoading = true;
  date=Date;

  error: string | null = null;

  paginationInfo = {
    currentPage: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 1
  };
  math=Math;

  // stats: StatCard[] = [
  //   { label: 'Total Users', value: 0, icon: 'bi-people-fill', color: 'primary' },
  //   { label: 'Agents', value: 0, icon: 'bi-person-badge-fill', color: 'success' },
  //   { label: 'Buyers', value: 0, icon: 'bi-house-heart-fill', color: 'info' },
  //   { label: 'Admins', value: 0, icon: 'bi-shield-fill', color: 'warning' }
  // ];

  // Main Stats
  stats: StatCard[] = [
    { label: 'Total Users', value: 0, icon: 'bi-people-fill', color: 'primary' },
    { label: 'Total Properties', value: 0, icon: 'bi-house-fill', color: 'success' },
    { label: 'Total Inquiries', value: 0, icon: 'bi-chat-dots-fill', color: 'info' },
    { label: 'Active Agents', value: 0, icon: 'bi-person-badge-fill', color: 'warning' }
  ];

  // User Distribution
  userStats = {
    totalUsers: 0,
    agents: 0,
    buyers: 0,
    admins: 0,
    newUsersThisMonth: 0
  };

  // Property Stats
  propertyStats = {
    totalProperties: 0,
    activeListings: 0,
    soldProperties: 0,
    averagePrice: 0,
    newListingsThisMonth: 0
  };

  // Inquiry Stats
  inquiryStats = {
    totalInquiries: 0,
    pendingInquiries: 0,
    resolvedInquiries: 0,
    newInquiriesToday: 0
  };

  // Recent Activity
  recentUsers: User[] = [];
  recentProperties: any[] = [];
  recentInquiries: any[] = [];


  private usersSubject = new BehaviorSubject<UserQueryParams>({
    pageNumber: 1,
    pageSize: 10
  });

  // constructor(private userService:UserService){}

  ngOnInit(): void {
    this.loadDashboardData();
    // this.loadStats();
    // this.loadUsers();
    // this.usersSubject.pipe(
    //   debounceTime(300),
    //   distinctUntilChanged(),
    //   switchMap(params => {
    //     this.isLoading = true;
    //     return this.userService.getAllUsers(params);
    //   })
    // ).subscribe({
    //   next:(response)=>{
    //     this.users=response.items??[],
    //     this.filteredUsers=[...this.users];
    //     this.updateStats(this.users);
    //     this.updatePaginationInfo(response);
    //     this.isLoading = false;
    //   }
    // });
  }

  loadDashboardData(): void {
      this.isStatsLoading = true;
      this.isLoading=true;
      this.error = null;
  
      // Load all data in parallel
      forkJoin({
        users: this.userService.getAllUsers(), // Get all users for stats
        properties: this.listingService.getListings(), // Get all properties
        inquiries: this.inquiryService.getInquiries(), // Get all inquiries
        recentUsers: this.userService.getAllUsers({ pageSize: 5 }),
        recentProperties: this.listingService.getListings({ pageSize: 5}),
        recentInquiries: this.inquiryService.getInquiries({ pageSize: 5 })
      }).subscribe({
        next: (results) => {
          this.processData(results);
          this.isStatsLoading = false;
          this.isLoading=false;
        },
        error: (err) => {
          console.error('Error loading dashboard data:', err);
          this.error = 'Failed to load dashboard data. Please try again.';
          this.isStatsLoading = false;
          this.isLoading=false;
        }
      });
    }
  
    private processData(data: any): void {
      const users = data.users.items || [];
      const properties = data.properties.items || [];
      const inquiries = data.inquiries.items || [];
  
      // Update main stats
      this.stats[0].value = users.length;
      this.stats[1].value = properties.length;
      this.stats[2].value = inquiries.length;
      this.stats[3].value = users.filter((u: User) => u.role === 'Agent').length;
  
      // Update user stats
      this.userStats = {
        totalUsers: users.length,
        agents: users.filter((u: User) => u.role === 'Agent').length,
        buyers: users.filter((u: User) => u.role === 'Buyer').length,
        admins: users.filter((u: User) => u.role === 'Admin').length,
        newUsersThisMonth: this.getNewItemsThisMonth(users)
      };
  
      // Update property stats
      const activeProp = properties.filter((p: any) => !p.isDeleted);
      const soldProp = properties.filter((p: any) => p.status === 'Sold');
      const avgPrice = properties.length > 0 
        ? properties.reduce((sum: number, p: any) => sum + p.price, 0) / properties.length 
        : 0;
  
      this.propertyStats = {
        totalProperties: properties.length,
        activeListings: activeProp.length,
        soldProperties: soldProp.length,
        averagePrice: avgPrice,
        newListingsThisMonth: this.getNewItemsThisMonth(properties)
      };
  
      // Update inquiry stats
      this.inquiryStats = {
        totalInquiries: inquiries.length,
        pendingInquiries: inquiries.filter((i: any) => i.status === 'Pending').length,
        resolvedInquiries: inquiries.filter((i: any) => i.status === 'Resolved').length,
        newInquiriesToday: this.getNewItemsToday(inquiries)
      };
  
      // Set recent data
      this.recentUsers = data.recentUsers.items || [];
      this.recentProperties = data.recentProperties.items || [];
      this.recentInquiries = data.recentInquiries.items || [];
    }

    private getNewItemsThisMonth(items: any[]): number {
    const now = new Date();
    const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    return items.filter(item => new Date(item.createdAt) >= startOfMonth).length;
  }

  private getNewItemsToday(items: any[]): number {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return items.filter(item => new Date(item.createdAt) >= today).length;
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
