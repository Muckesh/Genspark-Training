// contact.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContactRequest, ContactResponse, CaptchaResponse } from '../models/contact.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private apiUrl = `${environment.apiUrl}/contactus`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<ContactResponse[]> {
    return this.http.get<ContactResponse[]>(`${this.apiUrl}/get-all`);
  }

  getById(id: number): Observable<ContactResponse> {
    return this.http.get<ContactResponse>(`${this.apiUrl}/get/${id}`);
  }

  create(contact: ContactRequest): Observable<ContactResponse> {
    return this.http.post<ContactResponse>(`${this.apiUrl}/create`, contact);
  }

  update(id: number, contact: ContactRequest): Observable<ContactResponse> {
    return this.http.put<ContactResponse>(`${this.apiUrl}/update/${id}`, contact);
  }

  delete(id: number): Observable<ContactResponse> {
    return this.http.delete<ContactResponse>(`${this.apiUrl}/delete/${id}`);
  }
}