import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { OrderResponseDto } from "../models/order.model";

@Injectable()
export class OrderService {
    private readonly baseUrl = `${environment.apiUrl}/orders`;
    private http = inject(HttpClient);

    getAll(page: number = 1, pageSize: number = 10): Observable<OrderResponseDto[]> {
        return this.http.get<OrderResponseDto[]>(`${this.baseUrl}/get-all?page=${page}&pageSize=${pageSize}`);
    }

    getById(id: number): Observable<OrderResponseDto> {
        return this.http.get<OrderResponseDto>(`${this.baseUrl}/get/${id}`);
    }

    update(id: number, update: any): Observable<OrderResponseDto> {
        return this.http.put<OrderResponseDto>(`${this.baseUrl}/update/${id}`, update);
    }

    delete(id: number): Observable<OrderResponseDto> {
        return this.http.delete<OrderResponseDto>(`${this.baseUrl}/delete/${id}`);
    }

    create(order: any): Observable<OrderResponseDto> {
        return this.http.post<OrderResponseDto>(`${this.baseUrl}/create`, order);
    }

    exportToPdf(): Observable<Blob> {
        return this.http.get(`${this.baseUrl}/export`, { responseType: 'blob' });
    }
}