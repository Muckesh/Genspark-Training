import { Component, inject, OnInit } from '@angular/core';
// import { Footer } from '../footer/footer';
import { PropertyListing } from '../../../models/property-listing.model';
import { PropertyListingService } from '../../../services/property-listing.service';
import { BehaviorSubject, debounceTime, distinctUntilChanged, forkJoin, switchMap } from 'rxjs';
import { PropertyListingQueryParametersDto } from '../../../models/property-listing-query-parameters.dto';
import { CommonModule } from '@angular/common';
import { PagedResult } from '../../../models/paged-result.model';
import { StatCard, User } from '../../../models/user.model';
import { AuthService } from '../../../services/auth.service';
import { RouterLink } from '@angular/router';
import { InquiryService } from '../../../services/inquiry.service';
import { InquiryResponseDto } from '../../../models/inquiry.model';
import { PropertyCard } from '../../shared/property-card/property-card';

@Component({
  selector: 'app-buyer-dashboard',
  imports: [CommonModule,PropertyCard,RouterLink],
  templateUrl: './buyer-dashboard.html',
  styleUrl: './buyer-dashboard.css'
})
export class BuyerDashboard implements OnInit {
  user!:User;
  listings: PropertyListing[] = [];
  inquiries:InquiryResponseDto[]=[];
  relatedProperties: PropertyListing[] = [];
  propertiesWithinBudget:PropertyListing[]=[];
  isLoading = false;
  isStatsLoading = true;
  paginationInfo = {
    currentPage: 1,
    pageSize: 12,
    totalCount: 0,
    totalPages: 1
  };

  math=Math;
  error: string | null = null;

  stats: StatCard[] = [
    { label: 'My Inquiries', value: 0, icon: 'bi-chat-dots-fill', color: 'primary' },
    { label: 'Available Properties', value: 0, icon: 'bi-house-fill', color: 'success' },
    { label: 'In My Location', value: 0, icon: 'bi-geo-alt-fill', color: 'info' },
    { label: 'Within Budget', value: 0, icon: 'bi-currency-rupee', color: 'warning' }
  ];

  private listingService = inject(PropertyListingService);
  private authService = inject(AuthService);
  private inquiryService = inject(InquiryService);

  private filtersSubject = new BehaviorSubject<PropertyListingQueryParametersDto>({
    pageNumber:1,
    pageSize:10
  });

  
  ngOnInit() {
    this.user=this.authService.getCurrentUser();
    
    this.loadDashboardData();

      this.loadRelatedProperties(this.user.buyerProfile?.preferredLocation!);
      this.loadPropertiesWithinBudget(this.user.buyerProfile?.budget!);

      // this.updateStats(this.propertiesWithinBudget,this.inquiries,this.relatedProperties,this.listings);
      // Subscribe to filter changes with debounce
      this.filtersSubject.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(filters=>{
          this.isLoading=true;
          return this.listingService.getListings(filters);
        })
      ).subscribe({
        next:(res)=>{
          this.listings=res.items??[];
          this.updatePaginationInfo(res);
          this.isLoading=false;
        },
        error:()=>this.isLoading=false
      });

  }

  loadDashboardData(): void {
    this.isStatsLoading = true;
    
    // Create observables for all the data we need
    const myInquiries$ = this.inquiryService.getMyInquiries();
    const allProperties$ = this.listingService.getListings({ pageSize: 1 }); // Just to get total count
    const locationProperties$ = this.user.buyerProfile?.preferredLocation 
      ? this.listingService.getListings({ location: this.user.buyerProfile.preferredLocation, pageSize: 1 })
      : this.listingService.getListings({ pageSize: 0 });
    const budgetProperties$ = this.user.buyerProfile?.budget 
      ? this.listingService.getListings({ maxPrice: this.user.buyerProfile.budget, pageSize: 1 })
      : this.listingService.getListings({ pageSize: 0 });

    // Execute all requests in parallel
    forkJoin({
      inquiries: myInquiries$,
      allProperties: allProperties$,
      locationProperties: locationProperties$,
      budgetProperties: budgetProperties$
    }).subscribe({
      next: (results) => {
        this.updateStats(results);
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
    this.stats[0].value = data.inquiries.totalCount || 0; // My Inquiries
    this.stats[1].value = data.allProperties.totalCount || 0; // Available Properties
    this.stats[2].value = data.locationProperties.totalCount || 0; // In My Location
    this.stats[3].value = data.budgetProperties.totalCount || 0; // Within Budget
  }

  refreshDashboard(): void {
    this.loadDashboardData();
    this.loadRelatedProperties(this.user.buyerProfile?.preferredLocation!);
  }

  getStatActionLink(statLabel: string): string {
    switch (statLabel) {
      case 'My Inquiries':
        return '/buyer/my-inquiries';
      case 'Available Properties':
      case 'In My Location':
      case 'Within Budget':
        return '/buyer/browse-listings';
      default:
        return '/buyer/browse-listings';
    }
  }

  trackByStat(index: number, stat: StatCard): string {
    return stat.label;
  }

  trackByProperty(index: number, property: any): string {
    return property.id;
  }

  loadMyInquiries(){
    this.inquiryService.getMyInquiries().subscribe((data)=>{
      this.inquiries=data.items || [];
    })
  }

  loadPropertiesWithinBudget(budget:number):void{
    this.listingService.getListings({maxPrice:budget}).subscribe((data)=>{
      this.propertiesWithinBudget=data.items||[];
    });
  }

  loadRelatedProperties(location: string): void {
    this.listingService.getListings({ location }).subscribe((data) => {
      this.relatedProperties = (data.items || []);
    });
  }



  private updatePaginationInfo(res: PagedResult<PropertyListing>) {
    this.paginationInfo = {
      currentPage: res.pageNumber,
      pageSize: res.pageSize,
      totalCount: res.totalCount,
      totalPages: Math.ceil(res.totalCount / res.pageSize)
    };
  }
  onFiltersChanged(filters: any) {
    // this.listingService.getListings(filters).subscribe({
    //   next: (res) => this.listings = res.items ?? []
    // });
    // Update the BehaviorSubject with new filters
    this.filtersSubject.next({
      ...this.filtersSubject.value,
      ...filters,
      pageNumber:1 // reset to first page when filters change
    });

  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.paginationInfo.totalPages) {
      this.filtersSubject.next({
        ...this.filtersSubject.value,
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

  loadUsers(){
    this.isLoading = true;
    this.error = null;

    
  }

  
}
