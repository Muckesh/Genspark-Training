import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Buyer, UpdateBuyerDto } from '../models/buyer.model';

@Injectable({ providedIn: 'root' })
export class BuyerService {
  private baseUrl = `${environment.apiUrl}/buyers`;

  constructor(private http: HttpClient) {}

  getBuyers(query: any): Observable<any> {
    return this.http.get<any>(this.baseUrl, { params: query });
  }

  updateBuyer(id: string, data: UpdateBuyerDto): Observable<Buyer> {
    return this.http.put<Buyer>(`${this.baseUrl}/${id}`, data);
  }
}
