import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';
import { User } from '../models/user.model';
import { ChangePasswordDto } from '../models/change-password-dto';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl = `${environment.apiUrl}/user`;

  constructor(private http: HttpClient) {}

  getAllUsers(params?: any): Observable<PagedResult<User>> {
     // Convert empty strings to undefined
    const cleanParams: any = {};
    for (const key in params) {
      if (params[key] !== '' && params[key] !== null && params[key] !== undefined) {
        cleanParams[key] = params[key];
      }
    }

    return this.http.get<any>(this.baseUrl, { params }).pipe(
      map(response=>{
        console.log(response.items.$values[0]);
          const mapped:PagedResult<User>={
            pageNumber:response.pageNumber,
            pageSize:response.pageSize,
            totalCount:response.totalCount,
            items:(response.items?.$values || []).map((item:any)=> this.transformUser(item))
          };
          console.log(mapped);
          return mapped;
      })
    );
  }
  private transformUser(raw: any):User {
    return {
      id:raw.id,
      name:raw.name,
      email:raw.email,
      role:raw.role,
      isDeleted:raw.isDeleted,
      createdAt:raw.createdAt,
      agentProfile:raw.agentProfile,
      buyerProfile:raw.buyerProfile
    };
  }

  updateUser(id: string, userData: any): Observable<User> {
    return this.http.put<User>(`${this.baseUrl}/${id}`, userData);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  changePassword(id: string, dto: ChangePasswordDto): Observable<any> {
    return this.http.put(`${this.baseUrl}/change-password/${id}`, dto);
  }

  getStats():Observable<any>{
    return this.http.get(`${this.baseUrl}/stats`);
  }
}
