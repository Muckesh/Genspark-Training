import { Component, inject } from '@angular/core';
import { CartService } from '../../../services/cart.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CartItem, CheckoutDto } from '../../../models/order.model';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './checkout.html',
  styleUrls: ['./checkout.css']
})
export class Checkout {
  private cartService = inject(CartService);
  private router = inject(Router);
  
  cartItems$ = this.cartService.cart$;
  isPlacingOrder = false;
  
  checkoutForm: CheckoutDto = {
    orderName: `ORDER_${Math.floor(1000 + Math.random() * 9000)}`,
    paymentType: 'Cash',
    customerName: '',
    customerPhone: '',
    customerEmail: '',
    customerAddress: '',
    items: []
  };

  placeOrder(): void {
    this.isPlacingOrder = true;
    
    // Get current cart items
    const cartItems = this.cartService.getCartItems();
    this.checkoutForm.items = cartItems;
    
    this.cartService.placeOrder(this.checkoutForm).subscribe({
      next: (order) => {
        this.cartService.clearCart();
        alert("Order placed successfully")
        this.router.navigate(['/orders']);
      },
      error: (err) => {
        console.error('Order placement failed', err);
        alert('Order placement failed');
        this.isPlacingOrder = false;
      }
    });
  }

  getCartTotal(): number {
    return this.cartService.getCartTotal();
  }
}