import { CurrencyPipe } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ProductModel } from '../models/product';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product',
  imports: [CurrencyPipe],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product {
@Input() product:ProductModel | null = new ProductModel();
@Output() addToCart:EventEmitter<Number> = new EventEmitter<Number>();
private productService = inject(ProductService);

handleBuyClick(pid:Number|undefined){
  if(pid){
    this.addToCart.emit(pid);
  }
}

constructor() {
  // this.productService.getProduct(1).subscribe(
  //   {
  //     next:(data)=>{
    
  //       this.product = data as ProductModel;
  //       console.log(this.product)
  //     },
  //     error:(err)=>{
  //       console.log(err)
  //     },
  //     complete:()=>{
  //       console.log("All done");
  //     }
  //   })

}

}


/*

<button type="button" class="btn btn-primary">
  Notifications <span class="badge badge-light">{{cartCount}}</span>
</button>
  <div>
        @if (cartCount>0) {
            @for (item of cartItems; track item.Id) {
            <li>{{item.Id}} -- {{item.Count}}</li>
            }
        }
  </div>


@if (products) {
    <div>
        @for (item of products; track item.id) {
            <app-product (addToCart)="handleAddToCart($event)" [product]="item"></app-product>
        }
    </div>
}
@else {
    <div>
        <div class="spinner-border text-success" role="status">
            <span class="sr-only">Loading...</span>
        </div>
    </div>
}
--------------
import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { ProductModel } from '../models/product';
import { Product } from "../product/product";
import { CartItem } from '../models/cartItem';



@Component({
  selector: 'app-products',
  imports: [Product],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products:ProductModel[]|undefined=undefined;
  cartItems:CartItem[] =[];
  cartCount:number =0;
  constructor(private productService:ProductService)

  }
  handleAddToCart(event:Number)
  {
    console.log("Handling add to cart - "+event)
    let flag = false;
    for(let i=0;i<this.cartItems.length;i++)
    {
      if(this.cartItems[i].Id==event)
      {
         this.cartItems[i].Count++;
         flag=true;
      }
    }
    if(!flag)
      this.cartItems.push(new CartItem(event,1));
    this.cartCount++;
  }
  ngOnInit(): void {
    this.productService.getAllProducts().subscribe(
      {
        next:(data:any)=>{
         this.products = data.products as ProductModel[];
        },
        error:(err)=>{},
        complete:()=>{}
      }
    )
  }

}
---------------------
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ProductService } from '../services/product.service';
import { ProductModel } from '../models/product';
import { CurrencyPipe } from '@angular/common';




@Component({
  selector: 'app-product',
  imports: [CurrencyPipe],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product {
@Input() product:ProductModel|null = new ProductModel();
@Output() addToCart:EventEmitter<Number> = new EventEmitter<Number>();
private productService = inject(ProductService);

handleBuyClick(pid:Number|undefined){
  if(pid)
  {
      this.addToCart.emit(pid);
  }
}
constructor(){
    // this.productService.getProduct(1).subscribe(
    //   {
    //     next:(data)=>{
     
    //       this.product = data as ProductModel;
    //       console.log(this.product)
    //     },
    //     error:(err)=>{
    //       console.log(err)
    //     },
    //     complete:()=>{
    //       console.log("All done");
    //     }
    //   })
}

}
--------------------------
<div class="card" style="width: 18rem;">
  <img class="card-img-top" [src]="product?.thumbnail" alt="Card image cap">
  <div class="card-body">
    <h5 class="card-title">{{product?.title}}</h5>
    <p class="card-text">{{product?.description}}</p>
    <button (click)="handleBuyClick(product?.id)" class="btn btn-primary">Buy for {{product?.price | currency:'INR'}}</button>
  </div>
</div>

*/