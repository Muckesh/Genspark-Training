import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
// import { Footer } from '../../footer/footer';
import { Navbar } from '../../shared/navbar/navbar';
import { PropertyListingService } from '../../../services/property-listing.service';
import { InquiryService } from '../../../services/inquiry.service';
import { StatCard } from '../../../models/user.model';
import { PropertyListing } from '../../../models/property-listing.model';
import { forkJoin } from 'rxjs';
import { PropertyCard } from '../../shared/property-card/property-card';

@Component({
  selector: 'app-agent-dashboard',
  imports: [CommonModule,RouterLink,PropertyCard],
  templateUrl: './agent-dashboard.html',
  styleUrl: './agent-dashboard.css'
})
export class AgentDashboard implements OnInit {

  listingService = inject(PropertyListingService);
  inquiryService = inject(InquiryService);

  myListings: PropertyListing[] = [];
  // rentListings: PropertyListing[]=[];
  recentListings: PropertyListing[]=[];
    isLoading = false;
    paginationInfo = {
      currentPage: 1,
      pageSize: 12,
      totalCount: 0,
      totalPages: 1
    };
  
    math=Math;
    isStatsLoading = true;

    error: string | null = null;
  
    stats: StatCard[] = [
    { label: 'My Listings', value: 0, icon: 'bi-house-fill', color: 'primary' },
    { label: 'My Inquiries', value: 0, icon: 'bi-chat-dots-fill', color: 'success' },
    { label: 'Available Properties', value: 0, icon: 'bi-buildings-fill', color: 'info' },
    { label: 'Active Listings', value: 0, icon: 'bi-check-circle-fill', color: 'warning' }
  ];
  

  ngOnInit() {
    // this.listingService.getMyListings().subscribe(listings => {
    //   this.stats[0].value = listings.items.length;
    //   this.stats[2].value = listings.items.filter(l => !l.isDeleted).length;
    // });

    this.loadDashboardData();
    // this.loadProperties("Rent");
    this.loadProperties();
    // this.inquiryService.getAgentInquiries()
    //   .subscribe(inquiries => this.stats[1].value = inquiries.items.length);
  }

    loadDashboardData(): void {
    this.isStatsLoading = true;
    this.error = null;
    
    // Create observables for all the data we need
    const myListings$ = this.listingService.getMyListings();
    const myInquiries$ = this.inquiryService.getAgentInquiries();
    const allProperties$ = this.listingService.getListings({ pageSize: 1 }); // Just to get total count

    // Execute all requests in parallel
    forkJoin({
      myListings: myListings$,
      myInquiries: myInquiries$,
      allProperties: allProperties$
    }).subscribe({
      next: (results) => {
        this.updateStats(results);
        this.myListings = results.myListings.items?.slice(0, 6) || []; // Show first 6 listings
        this.isStatsLoading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard data:', err);
        this.isStatsLoading = false;
        this.error = 'Failed to load dashboard data. Please try again.';
      }
    });
  }

  updateStats(data: any): void {
    const myListingsItems = data.myListings.items || [];
    
    this.stats[0].value = myListingsItems.length; // My Listings
    this.stats[1].value = data.myInquiries.totalCount || 0; // My Inquiries
    this.stats[2].value = data.allProperties.totalCount || 0; // Available Properties
    this.stats[3].value = myListingsItems.filter((listing: any) => listing.status==="Available").length; // Active Listings
  }

  refreshDashboard(): void {
    this.loadDashboardData();
  }

  // loadProperties(params:any):void{
  //   this.listingService.getListings({listingType:params}).subscribe((data)=>{
  //     this.rentListings=data.items.slice(0,6)||[];
  //   });
  // }

  loadProperties():void{
    this.listingService.getListings().subscribe((data)=>{
      this.recentListings=data.items.slice(0,6)||[];
    });
  }

  getStatActionLink(statLabel: string): string {
    switch (statLabel) {
      case 'My Listings':
      case 'Active Listings':
        return '/agent/listings';
      case 'My Inquiries':
        return '/agent/inquiries';
      case 'Available Properties':
        return '/agent/browse-listings';
      default:
        return '/agent/listings';
    }
  }

  trackByStat(index: number, stat: StatCard): string {
    return stat.label;
  }

  trackByProperty(index: number, property: any): string {
    return property.id;
  }

  loadUsers(){
    this.isLoading = true;
    this.error = null;

  }


}
