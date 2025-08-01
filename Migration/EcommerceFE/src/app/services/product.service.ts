import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { ProductRequest, ProductResponse } from "../models/product.model";

@Injectable()
export class ProductService {
    private readonly baseUrl = `${environment.apiUrl}/products`;
    private http = inject(HttpClient);

    getAll(params?:any): Observable<ProductResponse[]> {
        return this.http.get<ProductResponse[]>(`${this.baseUrl}/get-all`,{params:params});
    }

    getById(id: number): Observable<ProductResponse> {
        return this.http.get<ProductResponse>(`${this.baseUrl}/get/${id}`);
    }

    update(id: number, update: ProductRequest): Observable<ProductResponse> {
        const formData = this.createFormData(update);
        return this.http.put<ProductResponse>(`${this.baseUrl}/update/${id}`, formData);
    }

    delete(id: number): Observable<ProductResponse> {
        return this.http.delete<ProductResponse>(`${this.baseUrl}/delete/${id}`);
    }

    create(product: ProductRequest): Observable<ProductResponse> {
        const formData = this.createFormData(product);
        return this.http.post<ProductResponse>(`${this.baseUrl}/create`, formData);
    }

    private createFormData(product: ProductRequest): FormData {
        const formData = new FormData();
        formData.append('ProductName', product.productName);
        if(product.image) formData.append('Image', product.image);
        formData.append('Price', product.price.toString());
        if (product.userId) formData.append('UserId', product.userId.toString());
        if (product.categoryId) formData.append('CategoryId', product.categoryId.toString());
        if (product.colorId) formData.append('ColorId', product.colorId.toString());
        if (product.modelId) formData.append('ModelId', product.modelId.toString());
        // if (product.sellStartDate) formData.append('SellStartDate', product.sellStartDate.toISOString());
        // if (product.sellEndDate) formData.append('SellEndDate', product.sellEndDate.toISOString());
        // if (product.isNew) formData.append('IsNew', product.isNew.toString());
        return formData;
    }
}