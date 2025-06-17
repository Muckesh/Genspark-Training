import { Component, inject, Input, OnInit } from '@angular/core';
import { ProductModel } from '../models/productModel';
import { ProductService } from '../services/product.service';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-product',
  imports: [CurrencyPipe,CommonModule,RouterModule],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product implements OnInit {
  @Input() product:ProductModel | null = new ProductModel();
  loading=true;
  error:string="";
  private productService = inject(ProductService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.params["id"]);
    console.log(id);
    this.productService.getProductById(id).subscribe({
      next:(data:any)=>{
        console.log(data);
        this.product=data;
        this.loading = false;
      },
      error:()=>{
        this.error="Invalid product Id.";
        this.loading=false;
      }
    });

  }
}
