import { Component, OnInit } from '@angular/core';
import { ProductResponse } from '../../../models/product.model';
import { AuthService } from '../../../services/auth.service';
import { CartService } from '../../../services/cart.service';
import { ProductService } from '../../../services/product.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css'
})
export class ProductList implements OnInit {
  constructor(private authService:AuthService, private productService:ProductService,private cartService:CartService,private route: ActivatedRoute){}
  
    products:ProductResponse[]=[];
    isLoading=false;
    currentCategory: string | null = null;

    ngOnInit(): void {
      this.route.queryParams.subscribe(params => {
      this.currentCategory = params['categoryName'] || null;
      if(this.currentCategory!=null)
        this.loadProducts({'categoryName':`${this.currentCategory}`});
      else
        this.loadProducts();
    });
    }
  
    loadProducts(params?:any){
      this.isLoading=true;
      this.productService.getAll(params).subscribe({
        next:(data)=>{
          this.products=data;
          this.isLoading=false;
        },
        error:(err)=>{
          console.error(err);
        }
      });
    }
  
    addToCart(product:ProductResponse){
      alert("Product added to cart.");
      this.cartService.addToCart(product);
    }
}
