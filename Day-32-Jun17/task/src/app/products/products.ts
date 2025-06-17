import { Component, OnInit } from '@angular/core';
import { Product } from '../product/product';
import { CommonModule } from '@angular/common';
import { ProductModel } from '../models/productModel';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { ProductService } from '../services/product.service';
import { FormsModule } from '@angular/forms';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-products',
  imports: [Product,CommonModule,FormsModule,RouterOutlet],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: ProductModel[]=[];
  searchString:string="";
  searchSubject = new Subject<string>();
  loading:boolean=false;
  limit:number=10;
  skip:number=0;
  total:number=0;
  displayScrollToTop:boolean=false;

  constructor(private productService:ProductService,private router:Router){}

  handleSearchProducts(){
    this.searchSubject.next(this.searchString);
  }

  goToProductDetails(id:number){
    this.router.navigate([`products/${id}`])
  }

  ngOnInit(): void {
    this.productService.getAllProducts().subscribe({
      next:(data:any)=>{
        this.products=data.products;
        this.loading=false;
      }
    });

    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      tap(()=>this.loading=true),
      switchMap(q=>this.productService.searchProducts(q,this.limit,this.skip)),
      tap(()=>this.loading=false)
    ).subscribe({
      next:(data:any)=>{
        this.products=data.products as ProductModel[];
        this.total=data.total;
      }
    });
  }
}
