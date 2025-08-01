import { Component, inject } from '@angular/core';
import { CartService } from '../../../services/cart.service';
import { CommonModule } from '@angular/common';
import { CartItem } from '../../../models/order.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [CommonModule,RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class Cart {
  private cartService = inject(CartService);
  
  cartItems$ = this.cartService.cart$;

  updateQuantity(item: CartItem, quantity: number): void {
    if (quantity > 0) {
      this.cartService.updateQuantity(item.product.productId, quantity);
    } else {
      this.removeItem(item.product.productId);
    }
  }

  removeItem(productId: number): void {
    this.cartService.removeFromCart(productId);
  }


  getCartTotal(): number {
    return this.cartService.getCartTotal();
  }

  clearCart(): void {
    this.cartService.clearCart();
  }
}
