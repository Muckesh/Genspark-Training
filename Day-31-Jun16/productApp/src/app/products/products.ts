import { Component, HostListener, inject, OnInit } from '@angular/core';
import { Product } from '../product/product';
import { FormsModule } from '@angular/forms';
import { ProductModel } from '../models/product';
import { debounce, debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { ProductService } from '../services/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-products',
  imports: [Product,FormsModule,CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: ProductModel[]=[];
  searchString:string="";
  searchSubject = new Subject<string>();
  loading:boolean=false;
  limit=10;
  skip=0;
  total=0;
  displayScrollToTop = false;

  constructor(private productService:ProductService) {}
  handleSearchProducts(){
    this.searchSubject.next(this.searchString);
  }

  ngOnInit(): void {

    this.productService.getAllProducts().subscribe({
      next:(data:any)=> {
        this.products=data.products;
        this.loading=false;
      }
    });

    this.searchSubject.pipe(
      debounceTime(4000),
      distinctUntilChanged(),
      tap(()=>this.loading=true),
      switchMap(query=> this.productService.getProductSearchResult(query,this.limit,this.skip)),
      tap(()=>this.loading=false)
    ).subscribe({
      next:(data:any)=>{
        this.products = data.products as ProductModel[];
        this.total=data.total;
        console.log(this.total);
      }
    });

  }
  @HostListener("window:scroll",[])
  onScroll():void{
    this.displayScrollToTop = window.scrollY > 500;
    const scrollPosition = window.innerHeight + window.scrollY;
    const threshold = document.body.offsetHeight - 100;
    if (scrollPosition>=threshold && this.products?.length<this.total) {
      this.loadMore();
    }
  }

  scrollToTop() {
  window.scrollTo({ top: 0, behavior: 'smooth' });
}


  loadMore(){
    this.loading = true;
    this.skip += this.limit;
    this.productService.getProductSearchResult(this.searchString,this.limit,this.skip)
      .subscribe({
        next:(data:any)=>{
          this.products=[...this.products,...data.products]
          this.loading=false;
        }
      })
  }


}
