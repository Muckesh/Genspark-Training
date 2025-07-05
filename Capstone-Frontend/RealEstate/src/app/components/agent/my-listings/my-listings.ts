import { Component, OnInit,CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { PropertyListingService } from '../../../services/property-listing.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Alert } from '../../shared/alert/alert';
import { PropertyListing } from '../../../models/property-listing.model';
import { PropertyList } from '../../shared/property-list/property-list';

@Component({
  selector: 'app-my-listings',
  imports: [CommonModule,PropertyList,RouterLink,Alert],
  templateUrl: './my-listings.html',
  styleUrl: './my-listings.css'
})
export class MyListings implements OnInit {
  listings:PropertyListing[]=[];
  isLoading = true;
  errorMessage: string | null = null;

  constructor(private listingService:PropertyListingService){}

  ngOnInit(): void {
    // this.listingService.getMyListings().subscribe({
    //   next:(res)=>{
    //     this.listings=res.items;
    //     console.log(this.listings);
    //   }
    // });
    this.loadListings();
  }

  loadListings() {
    this.isLoading = true;
    this.errorMessage = null;
    
    this.listingService.getMyListings().subscribe({
      next: (res) => {
        this.listings = res.items;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.message || 'Failed to load listings';
        this.isLoading = false;
      }
    });
  }

  //   deleteListing(id: string) {
  //   if (confirm('Are you sure you want to delete this listing?')) {
  //     this.listingService.deleteListing(id).subscribe({
  //       next: () => {
  //         this.listings = this.listings.filter(listing => listing.id !== id);
  //       },
  //       error: (error) => {
  //         this.errorMessage = error.message || 'Failed to delete listing';
  //       }
  //     });
  //   }
  // }

  handleListingDeleted(listingId: string) {
    this.listingService.deleteListing(listingId).subscribe({
      next: () => {
        this.listings = this.listings.filter(listing => listing.id !== listingId);
      },
      error: (error) => {
        this.errorMessage = error.message || 'Failed to delete listing';
      }
    });
  }


}
