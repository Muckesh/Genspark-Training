import { inject, Injectable } from '@angular/core';
import { ProductResponse } from '../models/product.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartItem, CheckoutBEDto, CheckoutDto, OrderResponseDto } from '../models/order.model';



@Injectable({
  providedIn: 'root'
})
export class CartService {
    private readonly baseUrl = `${environment.apiUrl}/shoppingcart`;
    private readonly paypalUrl = `${environment.apiUrl}/paypal`;
    private http = inject(HttpClient);
  private cartItems: CartItem[] = [];
  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  cart$ = this.cartSubject.asObservable();

  constructor() {
    this.loadCartFromSession();
  }

  private loadCartFromSession(): void {
    const savedCart = sessionStorage.getItem('cart');
    if (savedCart) {
      this.cartItems = JSON.parse(savedCart);
      this.cartSubject.next(this.cartItems);
    }
  }

  placeOrder(checkout:CheckoutDto):Observable<OrderResponseDto>{
    const checkoutDto=this.createCheckoutDto(checkout);
    console.log(checkoutDto);
    return this.http.post<OrderResponseDto>(`${this.baseUrl}/place-order`,checkoutDto);
  }

  createPaypalOrder(checkout:CheckoutDto):Observable<OrderResponseDto>{
    const checkoutDto = this.createCheckoutDto(checkout);
    return this.http.post<OrderResponseDto>(`${this.paypalUrl}/create-order`,checkoutDto);
  }

  capturePayPalOrder(orderId: string): Observable<OrderResponseDto> {
        return this.http.post<OrderResponseDto>(`${this.paypalUrl}/complete-order`, null, {
            params: { orderId }
        });
    }

    private createCheckoutDto(checkout: CheckoutDto): CheckoutBEDto {
          const checkoutDto: CheckoutBEDto = {} as CheckoutBEDto;
          checkoutDto.customerAddress = checkout.customerAddress;
          checkoutDto.customerEmail = checkout.customerEmail;
          checkoutDto.customerName = checkout.customerName;
          checkoutDto.customerPhone = checkout.customerPhone;
          checkoutDto.paymentType = checkout.paymentType;
          checkoutDto.items = checkout.items.map(item => ({
            productId: item.product.productId,
            quantity: item.quantity
          }));
          return checkoutDto;
      }

  private saveCartToSession(): void {
    sessionStorage.setItem('cart', JSON.stringify(this.cartItems));
    this.cartSubject.next(this.cartItems);
  }

  addToCart(product: ProductResponse, quantity: number = 1): void {
    const existingItem = this.cartItems.find(item => item.product.productId === product.productId);
    
    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      this.cartItems.push({ product, quantity });
    }
    
    this.saveCartToSession();
  }

  removeFromCart(productId: number): void {
    this.cartItems = this.cartItems.filter(item => item.product.productId !== productId);
    this.saveCartToSession();
  }

  updateQuantity(productId: number, quantity: number): void {
    const item = this.cartItems.find(item => item.product.productId === productId);
    if (item) {
      item.quantity = quantity;
      this.saveCartToSession();
    }
  }

  getCartItems(): CartItem[] {
    return [...this.cartItems];
  }

  getCartTotal(): number {
    return this.cartItems.reduce((total, item) => 
      total + (item.product.price * item.quantity), 0);
  }

  clearCart(): void {
    this.cartItems = [];
    sessionStorage.removeItem('cart');
    this.cartSubject.next(this.cartItems);
  }
}