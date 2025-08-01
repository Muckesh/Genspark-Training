import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { ProductService } from '../../../services/product.service';
import { ProductResponse } from '../../../models/product.model';
import { CommonModule } from '@angular/common';
import { CartService } from '../../../services/cart.service';
import { ProductList } from '../product-list/product-list';

@Component({
  selector: 'app-home',
  imports: [CommonModule,ProductList],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home  {
  
  
}
