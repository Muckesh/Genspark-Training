import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable()
export class ProductService{
    private http = inject(HttpClient);

    private apiUrl = 'https://dummyjson.com/products';

    getAllProducts(){
        return this.http.get(this.apiUrl);
    }

    getProductSearchResult(searchData:string,limit:number=10,skip:number=10){
        return this.http.get(`${this.apiUrl}/search?q=${searchData}&limit=${limit}&skip=${skip}`);
        // return this.http.get(`https://dummyjson.com/products/search?q=${searchData}&limit=${limit}&skip=${skip}`);

    }
}