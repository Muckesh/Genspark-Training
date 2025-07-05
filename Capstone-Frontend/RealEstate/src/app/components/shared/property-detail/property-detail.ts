import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PropertyListingService } from '../../../services/property-listing.service';
import { PropertyListing, PropertyListingResponseDto } from '../../../models/property-listing.model';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';
import { ModalService } from '../../../services/modal.service';
import { PropertyCard } from '../property-card/property-card';
import { InquiryService } from '../../../services/inquiry.service';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-property-detail',
  imports: [CommonModule, PropertyCard, RouterLink],
  templateUrl: './property-detail.html',
  styleUrls: ['./property-detail.css']
})
export class PropertyDetail implements OnInit {
  property!: PropertyListingResponseDto;
  isLoading = true;
  currentSlideIndex = 0;
  relatedProperties: PropertyListing[] = [];
  isBuyer=false;
  user!:User;
  constructor(
    private route: ActivatedRoute,
    private propertyService: PropertyListingService,
    private inquiryService:InquiryService,
    private modalService: ModalService,
    private router:Router,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isBuyer=this.authService.hasRole("Buyer");
    this.user = this.authService.getCurrentUser();
    
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isLoading = true;
        this.propertyService.getListingById(id).subscribe({
          next: (property) => {
            this.property = property.items[0];
            this.isLoading = false;
            this.loadRelatedProperties(this.property.location, this.property.id);
          },
          error: () => {
            this.isLoading = false;
          }
        });
      } else {
        this.isLoading = false;
      }
    });
  }

  goToSlide(index: number): void {
    this.currentSlideIndex = index;
    const carousel = document.getElementById('propertyCarousel');
    if (carousel && (window as any).bootstrap) {
      const bootstrapCarousel = new (window as any).bootstrap.Carousel(carousel);
      bootstrapCarousel.to(index);
    }
  }

  // openInquiryModal(): void {
  //   this.modalService.openInquiryModal(this.property.id);
  // }

  openInquiryModal(){
    this.inquiryService.checkIfInquiryExists(this.property.id, this.user.id).subscribe(exists => {
    if (exists) {
      this.router.navigate(['/buyer/my-inquiries']);
    } else {
      this.modalService.openInquiryModal(this.property.id);
    }
    });

      
  }


  shareProperty(): void {
    if (navigator.share) {
      navigator.share({
        title: this.property?.title,
        text: `Check out this property: ${this.property?.title}`,
        url: window.location.href
      }).catch(console.error);
    } else {
      // Fallback for browsers that don't support Web Share API
      const shareUrl = `mailto:?subject=${encodeURIComponent(this.property?.title || 'Property')}&body=${encodeURIComponent(window.location.href)}`;
      window.open(shareUrl, '_blank');
    }
  }

  copyToClipboard(): void {
    navigator.clipboard.writeText(window.location.href).then(() => {
      // You could add a toast notification here
      console.log('URL copied to clipboard');
    });
  }

  loadRelatedProperties(location: string, currentId: string): void {
    this.propertyService.getListings({ location }).subscribe((data) => {
      this.relatedProperties = (data.items || []).filter((p: PropertyListing) => p.id !== currentId).slice(0, 4);
    });
  }

  formatPrice(price: number): string {
    if (price >= 10000000) {
      return `₹${(price / 10000000).toFixed(1)} Cr`;
    } else if (price >= 100000) {
      return `₹${(price / 100000).toFixed(1)} L`;
    } else {
      return `₹${price.toLocaleString('en-IN')}`;
    }
  }
}