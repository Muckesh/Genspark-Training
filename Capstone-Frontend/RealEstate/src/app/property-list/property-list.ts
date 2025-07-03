import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PropertyListing } from '../models/property-listing.model';
import { PropertyCard } from '../property-card/property-card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-list',
  imports: [PropertyCard,CommonModule],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css'
})
export class PropertyList {
  @Input() listings: PropertyListing[] = [];
  @Input() showManageOptions = false;
  @Input() showInquireButton = false;
  @Output() listingDeleted = new EventEmitter<string>();



  // In PropertyList component
  trackById(index: number, listing: PropertyListing): string {
    return listing.id;
  }

  onListingDeleted(listingId: string) {
    this.listingDeleted.emit(listingId);
  }

}
