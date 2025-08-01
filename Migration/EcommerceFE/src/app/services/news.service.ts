import { inject, Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { NewsRequest, NewsResponse } from "../models/news.model";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";

@Injectable()
export class NewsService {
    protected endpoint = "news";

    protected baseUrl = `${environment.apiUrl}`;
    protected http = inject(HttpClient);

    getAll():Observable<NewsResponse[]>{
        return this.http.get<NewsResponse[]>(`${this.baseUrl}/${this.endpoint}/get-all`);
    }

    getById(id:number):Observable<NewsResponse>{
        return this.http.get<NewsResponse>(`${this.baseUrl}/${this.endpoint}/get/${id}`);
    }

    update(id: number, update: NewsRequest): Observable<NewsResponse> {
        const formData = this.createFormData(update);
        return this.http.put<NewsResponse>(`${this.baseUrl}/${this.endpoint}/update/${id}`, formData);
    }

    delete(id:number):Observable<NewsResponse>{
        return this.http.delete<NewsResponse>(`${this.baseUrl}/${this.endpoint}/delete/${id}`);
    }

    create(item: NewsRequest): Observable<NewsResponse> {
        const formData = this.createFormData(item);
        return this.http.post<NewsResponse>(`${this.baseUrl}/${this.endpoint}/create`, formData);
    }

    exportToCsv():Observable<Blob>{
        return this.http.get(`${this.baseUrl}/${this.endpoint}/export/csv`,{
            responseType:'blob'
        });
    }

    exportToExcel():Observable<Blob>{
        return this.http.get(`${this.baseUrl}/${this.endpoint}/export/excel`,{
            responseType:'blob'
        });
    }


    // // Override update method to handle FormData conversion
    // override update(id: number, update: NewsRequest): Observable<NewsResponse> {
    //     const formData = this.createFormData(update);
    //     return this.http.put<NewsResponse>(`${this.baseUrl}/${this.endpoint}/update/${id}`, formData);
    // }

    // Override create method to handle FormData conversion
    // override create(item: NewsRequest): Observable<NewsResponse> {
    //     const formData = this.createFormData(item);
    //     return this.http.post<NewsResponse>(`${this.baseUrl}/${this.endpoint}/create`, formData);
    // }

    private createFormData(news: NewsRequest): FormData {
        const formData = new FormData();
        formData.append('title', news.title);
        formData.append('shortDescription', news.shortDescription);
        formData.append('content', news.content);
        if(news.status)
            formData.append('status', news.status.toString());
        if(news.userId)
            formData.append('userId', news.userId.toString());
        
        if (news.image) {
            formData.append('image', news.image);
        }

        if (news.createdDate) {
            formData.append('createdDate', news.createdDate.toString());
        }

        return formData;
    }

}