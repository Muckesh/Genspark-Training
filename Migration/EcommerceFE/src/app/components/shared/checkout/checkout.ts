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
    
    if(this.checkoutForm.paymentType==="Cash"){
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
    if(this.checkoutForm.paymentType==="PayPal"){
      
      const carts = this.cartService.getCartItems().map(item=>({
        productId:item.product.productId,
        quantity:item.quantity
      }));
      this.cartService.checkoutWithPaypal(carts).subscribe({
        next:(res:any)=>{
          this.cartService.clearCart();
          const approveLink = res.links?.find((link:any)=> link.rel === "approve")?.href;
          if(approveLink)
            window.location.href=approveLink;
          else
            alert('Approval link not found.');
        },
        error:(err)=>{
          console.error(err);
          alert('Failed to initiate Paypal checkout.');
          this.isPlacingOrder=false;
        }
      });

    }
  }

  getCartTotal(): number {
    return this.cartService.getCartTotal();
  }
}