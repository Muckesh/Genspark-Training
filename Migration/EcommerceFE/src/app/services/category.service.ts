import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { CategoryRequest, CategoryResponse } from "../models/category.model";

@Injectable()
export class CategoryService {
    private readonly baseUrl = `${environment.apiUrl}/categories`;
    private http = inject(HttpClient);

    getAll():Observable<CategoryResponse[]>{
        return this.http.get<CategoryResponse[]>(`${this.baseUrl}/get-all`);
    }

    getById(id:number):Observable<CategoryResponse>{
        return this.http.get<CategoryResponse>(`${this.baseUrl}/get/${id}`);
    }

    update(id:number,update:CategoryRequest):Observable<CategoryResponse>{
        return this.http.put<CategoryResponse>(`${this.baseUrl}/update/${id}`,update);
    }

    delete(id:number):Observable<CategoryResponse>{
        return this.http.delete<CategoryResponse>(`${this.baseUrl}/delete/${id}`);
    }

    create(category:CategoryRequest):Observable<CategoryResponse>{
        return this.http.post<CategoryResponse>(`${this.baseUrl}/create`,category);
    }
}