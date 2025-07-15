import { Component, Input } from '@angular/core';
import { PropertyListingResponseDto } from '../../../models/property-listing.model';
import { NgbActiveModal, NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PurchaseService } from '../../../services/purchase.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkout-modal',
  imports: [NgbModule,FormsModule,CommonModule],
  templateUrl: './checkout-modal.html',
  styleUrl: './checkout-modal.css'
})
export class CheckoutModal {
  @Input() property!: PropertyListingResponseDto;
  isLoading = false;
  paymentMethod = 'bank_transfer';
  agreedToTerms = false;

  constructor(
    public activeModal: NgbActiveModal,
    private purchaseService: PurchaseService,
    private router: Router
  ) {}

  confirmPurchase(): void {
    if (!this.agreedToTerms) {
      alert('Please agree to the terms and conditions');
      return;
    }

    this.isLoading = true;
    this.purchaseService.buyProperty(this.property.id).subscribe({
      next: () => {
        this.activeModal.close('success');
        this.router.navigate(['/buyer/my-purchases']);
      },
      error: (err) => {
        this.isLoading = false;
        alert(err.error?.error || 'Failed to complete purchase');
      }
    });
  }

  formatPrice(price: number): string {
    if (price >= 10000000) {
      return `₹${(price / 10000000).toFixed(1)} Cr`;
    } else if (price >= 100000) {
      return `₹${(price / 100000).toFixed(1)} L`;
    }
    return `₹${price.toLocaleString('en-IN')}`;
  }
}
