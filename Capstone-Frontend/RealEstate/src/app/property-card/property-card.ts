import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { PropertyListing } from '../models/property-listing.model';
import { AsyncPipe, CommonModule, NgOptimizedImage } from '@angular/common';
import { PropertyImageService } from '../services/property-image.service';
import { Observable } from 'rxjs';
import { Router, RouterLink } from '@angular/router';
import { TruncatePipe } from '../pipes/truncate.pipe';
import { InquiryService } from '../services/inquiry.service';
import { AuthService } from '../services/auth.service';
import { ModalService } from '../services/modal.service';
import { User } from '../models/user.model';
import { PropertyListingService } from '../services/property-listing.service';

@Component({
  selector: 'app-property-card',
  imports: [CommonModule,RouterLink,TruncatePipe],
  templateUrl: './property-card.html',
  styleUrl: './property-card.css'
})
export class PropertyCard {
  @Input() listing!: PropertyListing;
  @Input() showManageOptions = false;
  @Input() showInquireButton = false;

  @Output() listingDeleted = new EventEmitter<string>();
  user!:User;
  // errorMessage:string|null=null;
  // isInquiring = false;
  // imageUrls$!: Observable<string[]>;

  // constructor(private propertyImageService: PropertyImageService) {}

  // ngOnChanges(changes: SimpleChanges): void {
  //   if (changes['listing'] && this.listing?.id) {
  //     this.imageUrls$ = this.propertyImageService.getImageUrls(this.listing.id) ?? [];
  //   }
  // }

  constructor(private modalService:ModalService,private router:Router,private authService:AuthService,private listingService:PropertyListingService,private inquiryService:InquiryService){
    // this.user=authService.getCurrentUser();
    this.user = authService.getCurrentUser();
  }

  get formattedPrice(): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      maximumFractionDigits: 0
    }).format(this.listing.price);
  }

  //   get formattedPrice(): string {
  //   if (this.listing.price >= 10000000) {
  //     return `₹${(this.listing.price / 10000000).toFixed(1)} Cr`;
  //   } else if (this.listing.price >= 100000) {
  //     return `₹${(this.listing.price / 100000).toFixed(1)} L`;
  //   } else {
  //     return `₹${this.listing.price.toLocaleString('en-IN')}`;
  //   }
  // }

  openInquiryModal(){
    this.inquiryService.checkIfInquiryExists(this.listing.id, this.user.id).subscribe(exists => {
    if (exists) {
      this.router.navigate(['/buyer/my-inquiries']);
    } else {
      this.modalService.openInquiryModal(this.listing.id);
    }
    });

      
  }

  onDelete() {
    if (confirm('Are you sure you want to delete this listing?')) {
      this.listingDeleted.emit(this.listing.id);
    }
  }

  isAgent():boolean{
    return this.authService.hasRole('Agent');
  }

  isBuyer():boolean{
    return this.authService.hasRole('Buyer');
  }

  //  deleteListing(id: string) {
  //   if (confirm('Are you sure you want to delete this listing?')) {
  //     this.listingService.deleteListing(id).subscribe({
  //       next: () => {
  //         // this.listings = this.listings.filter(listing => listing.id !== id);
  //       },
  //       error: (error) => {
  //         this.errorMessage = error.message || 'Failed to delete listing';
  //       }
  //     });
  //   }
  // }

}
