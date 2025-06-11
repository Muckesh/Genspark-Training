import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

interface Product {
  id: number;
  name: string;
  price: number;
  imageUrl: string;
}

@Component({
  selector: 'app-products',
  imports: [CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products {

  cartCount = 0;

  products: Product[] = [
    {
      id: 1,
      name: 'Smartphone',
      price: 699,
      imageUrl: 'https://images.unsplash.com/photo-1726066012645-959fc63f61b4?w=1600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDF8MHxzZWFyY2h8OHx8c21hcnRwaG9uZXxlbnwwfHwwfHx8MA%3D%3D'
    },
    {
      id: 2,
      name: 'Headphones',
      price: 199,
      imageUrl: 'https://plus.unsplash.com/premium_photo-1679513691474-73102089c117?w=1600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8aGVhZHBob25lc3xlbnwwfHwwfHx8MA%3D%3D'
    },
    {
      id: 3,
      name: 'Smartwatch',
      price: 299,
      imageUrl: 'https://images.unsplash.com/photo-1617625802912-cde586faf331?w=1600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8M3x8c21hcnR3YXRjaHxlbnwwfHwwfHx8MA%3D%3D'
    }
  ];

  addToCart() {
    this.cartCount++;
  }


}
