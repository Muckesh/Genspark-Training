import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { CategoryRequest, CategoryResponse } from "../models/category.model";

@Injectable()
export abstract class BaseService<T,TCreate=T,TUpdate = TCreate> {
    protected baseUrl = `${environment.apiUrl}`;
    protected abstract endpoint:string;
    protected http = inject(HttpClient);

    getAll():Observable<T[]>{
        return this.http.get<T[]>(`${this.baseUrl}/${this.endpoint}/get-all`);
    }

    getById(id:number):Observable<T>{
        return this.http.get<T>(`${this.baseUrl}/${this.endpoint}/get/${id}`);
    }

    update(id:number,update:TUpdate):Observable<TUpdate>{
        return this.http.put<TUpdate>(`${this.baseUrl}/${this.endpoint}/update/${id}`,update);
    }

    delete(id:number):Observable<T>{
        return this.http.delete<T>(`${this.baseUrl}/${this.endpoint}/delete/${id}`);
    }

    create(item:TCreate):Observable<TCreate>{
        return this.http.post<TCreate>(`${this.baseUrl}/${this.endpoint}/create`,item);
    }
}