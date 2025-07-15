import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PropertyImageService {
  private baseUrl = `${environment.apiUrl}/propertyimages`;

  constructor(private http: HttpClient) {}

  getImageUrls(listingId: string): Observable<string[]> {
    return this.http.get<any>(`${this.baseUrl}/urls/${listingId}`).pipe(
      map(res=>res.$values ?? [])
    );
  }

  uploadImage(listingId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('ImageFile', file);
    formData.append('PropertyListingId', listingId);

    return this.http.post(`${environment.apiUrl}/propertylistings/images/upload`, formData);
  }

  deleteImage(imageId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${imageId}`);
  }
}
