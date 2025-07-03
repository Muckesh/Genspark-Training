import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { catchError, map, Observable, of } from 'rxjs';
import { Inquiry, InquiryReply, InquiryResponseDto, InquiryWithRepliesDto } from '../models/inquiry.model';
import { AuthService } from './auth.service';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class InquiryService {
  private baseUrl = `${environment.apiUrl}/inquiries`;

  constructor(private http: HttpClient,private authService:AuthService) {}

  createInquiry(data: Inquiry): Observable<Inquiry> {
    return this.http.post<Inquiry>(this.baseUrl, data);
  }

  getInquiriesByListing(listingId: string): Observable<Inquiry[]> {
    return this.http.get<Inquiry[]>(`${this.baseUrl}?listingId=${listingId}`);
  }

  getInquiries(params?: any): Observable<PagedResult<InquiryResponseDto>> {
    const cleanParams: any = {};
    for (const key in params) {
      if (params[key] !== '' && params[key] !== null && params[key] !== undefined) {
        cleanParams[key] = params[key];
      }
    }

    return this.http.get<any>(this.baseUrl, { params:cleanParams }).pipe(
      map(response=>{
        console.log(response.items?.$values[0]);
        const mapped:PagedResult<InquiryResponseDto>={
          pageNumber:response.pageNumber,
          pageSize:response.pageSize,
          totalCount:response.totalCount,
          items:(response.items?.$values || []).map((item:any)=>this.transformInquiry(item))
        };
        console.log(mapped);
        return mapped;
      })
    );

    }

  getMyInquiries(): Observable<PagedResult<InquiryResponseDto>> {
    const currentUser = this.authService.getCurrentUser();
    console.log(currentUser.id);
    // console.log(inquiries);
    return this.getInquiries({ buyerId: currentUser!.id });
  }

  getAgentInquiries():Observable<PagedResult<InquiryResponseDto>>{
    const currentUser = this.authService.getCurrentUser();
    return this.getInquiries({agentId:currentUser!.id});
  }

  getExistingInquiry(listingId:string,buyerId:string):Observable<Inquiry>{
    return this.http.get<Inquiry>(`${this.baseUrl}/existing`,{params:{listingId:listingId,buyerId:buyerId}});
  }

  checkIfInquiryExists(listingId: string, buyerId: string): Observable<boolean> {
  return this.getExistingInquiry(listingId, buyerId).pipe(
    map(() => true),
    catchError(err => {
      if (err.status === 404) return of(false);
      else throw err;
    })
  );
}


  private transformInquiry(raw:any):InquiryResponseDto{
    // console.log(raw.listing)
    return {
      id:raw.id,
      listingId: raw.listingId,
      // listing: raw.listing,
      listingTitle:raw.listingTitle,
      buyerId: raw.buyerId,
      // buyer: raw.buyer,
      buyerName:raw.buyerName,
      agentId:raw.agentId,
      agentName:raw.agentName,
      message: raw.message,
      replies:raw.replies.$values,
      status:raw.status,
      createdAt: new Date(raw.createdAt)
    }
  }

  getInquiryWithReplies(inquiryId: string): Observable<InquiryWithRepliesDto> {
    return this.http.get<InquiryWithRepliesDto>(`${this.baseUrl}/${inquiryId}/with-replies`);
  }

  getReplies(inquiryId: string): Observable<InquiryReply[]> {
    // return this.http.get<InquiryReply[]>(`${this.baseUrl}/${inquiryId}/replies`);
    return this.http.get<any>(`${this.baseUrl}/${inquiryId}/replies`).pipe(
      map(response=>{
        const mapped:InquiryReply[]=response.$values;
        console.log(mapped);
        
        return mapped;
      })
    );
  }

  addReply(inquiryId: string, message: string): Observable<InquiryReply> {
    return this.http.post<InquiryReply>(
      `${this.baseUrl}/${inquiryId}/replies`,
      { message }
    );
  }

}
