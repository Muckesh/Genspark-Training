import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Footer } from '../../footer/footer';
import { Navbar } from '../../navbar/navbar';
import { PropertyListingService } from '../../services/property-listing.service';
import { InquiryService } from '../../services/inquiry.service';
import { StatCard } from '../../models/user.model';
import { PropertyListing } from '../../models/property-listing.model';

@Component({
  selector: 'app-agent-dashboard',
  imports: [CommonModule],
  templateUrl: './agent-dashboard.html',
  styleUrl: './agent-dashboard.css'
})
export class AgentDashboard implements OnInit {

  listingService = inject(PropertyListingService);
  inquiryService = inject(InquiryService);

  listings: PropertyListing[] = [];
    isLoading = false;
    paginationInfo = {
      currentPage: 1,
      pageSize: 12,
      totalCount: 0,
      totalPages: 1
    };
  
    math=Math;
    error: string | null = null;
  
    stats: StatCard[] = [
        { label: 'Total Users', value: 0, icon: 'bi-people-fill', color: 'primary' },
        { label: 'Agents', value: 0, icon: 'bi-person-badge-fill', color: 'success' },
        { label: 'Buyers', value: 0, icon: 'bi-house-heart-fill', color: 'info' },
        { label: 'Admins', value: 0, icon: 'bi-shield-fill', color: 'warning' }
      ];
  

  ngOnInit() {
    this.listingService.getMyListings().subscribe(listings => {
      this.stats[0].value = listings.items.length;
      this.stats[2].value = listings.items.filter(l => !l.isDeleted).length;
    });

    this.inquiryService.getAgentInquiries()
      .subscribe(inquiries => this.stats[1].value = inquiries.items.length);
  }

  loadUsers(){
    this.isLoading = true;
    this.error = null;

    // this.userService.getAllUsers().subscribe({
    //   next:(users)=>{
    //     this.users=users.items;
    //     console.log(this.users);
    //     this.filteredUsers=[...users.items];
    //     this.updateStats(users.items);
    //     this.isLoading=false;
    //   },
    //   error:(err)=>{
    //     this.error = 'Failed to load users. Please try again later.';
    //     this.isLoading = false;
    //     console.error('Error loading users:', err);
    //   }
    // });
  }


}
