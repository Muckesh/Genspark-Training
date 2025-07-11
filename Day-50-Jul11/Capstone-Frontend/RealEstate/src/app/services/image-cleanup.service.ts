import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { catchError, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ImageCleanupService {
  private baseUrl = `${environment.apiUrl}/propertyimages/admin/images/cleanup`;

  constructor(private http: HttpClient) {}

  cleanupDeletedImages(): Observable<CleanupResponse> {
    return this.http.delete<CleanupResponse>(this.baseUrl).pipe(
      catchError(error => {
        throw this.handleError(error);
      })
    );
  }

  private handleError(error: any): Error {
    console.error('Image cleanup failed:', error);
    return new Error(
      error.error?.message || 
      error.message || 
      'An unknown error occurred during image cleanup'
    );
  }

}

export interface CleanupResponse {
  message: string;
  imagesCleaned: number;
  timestamp: string;
}