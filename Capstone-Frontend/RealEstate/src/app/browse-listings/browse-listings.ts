import { Component, inject } from '@angular/core';
import { PropertyListing } from '../models/property-listing.model';
import { BehaviorSubject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { PropertyListingQueryParametersDto } from '../models/property-listing-query-parameters.dto';
import { PropertyListingService } from '../services/property-listing.service';
import { PagedResult } from '../models/paged-result.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarFilter } from '../sidebar-filter/sidebar-filter';
import { PropertyList } from '../property-list/property-list';

@Component({
  selector: 'app-browse-listings',
  imports: [CommonModule,FormsModule,SidebarFilter,PropertyList],
  templateUrl: './browse-listings.html',
  styleUrl: './browse-listings.css'
})
export class BrowseListings {
  listings: PropertyListing[] = [];
  isLoading = false;
  paginationInfo = {
    currentPage: 1,
    pageSize: 12,
    totalCount: 0,
    totalPages: 1
  };

  math=Math;

  private listingService = inject(PropertyListingService);

  private filtersSubject = new BehaviorSubject<PropertyListingQueryParametersDto>({
    pageNumber:1,
    pageSize:10
  });

  
  ngOnInit() {
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

    // this.listingService.getListings({pageNumber:1,pageSize:20}).subscribe({
    //   next: (res) => {
    //     console.log('Listings response:',res.items);
    //     // console.log(res.items?.$values[1].images.$values[0])
    //     this.listings=res.items??[];
    //   }
    // });
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

  // loadMore(){
  //   const current = this.filtersSubject.value;
  //   this.filtersSubject.next({
  //     ...current,
  //     pageNumber: current.pageNumber + 1
  //   });
  // }

}
