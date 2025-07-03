import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly baseUrl = environment.apiUrl;
  private readonly jwtHelper = new JwtHelperService();


  constructor(private http: HttpClient) {}

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isAccessTokenValid(): boolean {
    const token = this.getAccessToken();
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  isRefreshTokenValid(): boolean {
    const token = this.getRefreshToken();
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  storeTokens(response: { token: string; refreshToken: string }): void {
    localStorage.setItem('accessToken', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
  }

  clearTokens(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  refreshToken(refreshToken: string) {
    return this.http.post<{ token: string; refreshToken: string }>(
      `${this.baseUrl}/auth/refresh`, 
      { refreshToken }
    ).pipe(
      tap(response => this.storeTokens(response))
    );
  }
}