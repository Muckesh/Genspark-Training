import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';
import { Purchase } from "../models/purchase.model";

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
  private apiUrl = `${environment.apiUrl}/purchases`;

  constructor(private http: HttpClient) { }

  buyProperty(listingId: string): Observable<Purchase> {
    return this.http.post<Purchase>(`${this.apiUrl}`, { listingId });
  }

  getMyPurchases(): Observable<Purchase[]> {
    return this.http.get<any>(`${this.apiUrl}/my`).pipe(
      map(response => {
        // Handle both the array response and the $values format
        const purchases = response.$values || response;
        return purchases.map((purchase: any) => ({
          id: purchase.id,
          buyerId: purchase.buyerId,
          listingId: purchase.listingId,
          priceAtPurchase: purchase.priceAtPurchase,
          purchasedAt: new Date(purchase.purchasedAt),
          status: purchase.status,
          buyer: purchase.buyer,
          listing: purchase.listing
        }));
      })
    );
  }
}