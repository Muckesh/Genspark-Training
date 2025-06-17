import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable()
export class ProductService{
    private apiUrl = "https://dummyjson.com/products"

    constructor(private http:HttpClient){}

    searchProducts(query:string,limit:number,skip:number){
        return this.http.get(`${this.apiUrl}/search?q=${query}&limit=${limit}&skip=${skip}`);
    }

    getProductById(id:number){
        
        console.log(this.http.get(`${this.apiUrl}/${id}`));
        return this.http.get(`${this.apiUrl}/${id}`);
    }

    getAllProducts(){
        return this.http.get(this.apiUrl);
    }
}