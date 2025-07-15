import { Component } from '@angular/core';
import { Purchase } from '../../../models/purchase.model';
import { PurchaseService } from '../../../services/purchase.service';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-my-purchases',
  imports: [DatePipe,CommonModule,FormsModule,RouterLink],
  templateUrl: './my-purchases.html',
  styleUrl: './my-purchases.css'
})
export class MyPurchases {
  purchases: Purchase[] = [];
  isLoading = true;

  constructor(private purchaseService: PurchaseService) {}

  ngOnInit(): void {
    this.loadPurchases();
  }

  loadPurchases(): void {
    this.isLoading = true;
    this.purchaseService.getMyPurchases().subscribe({
      next: (purchases) => {
        this.purchases = purchases;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
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

  getPropertyTypeIcon(propertyType: string): string {
    switch (propertyType?.toLowerCase()) {
      case 'apartment':
        return 'bi-building';
      case 'villa':
        return 'bi-house';
      case 'plot':
        return 'bi-pin-map';
      case 'commercial':
        return 'bi-shop';
      default:
        return 'bi-house';
    }
  }

}
