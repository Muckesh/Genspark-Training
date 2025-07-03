import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';
import { CreateListingDto, UpdateListingDto, PropertyListing, PropertyListingResponseDto } from '../models/property-listing.model';
import { AuthService } from './auth.service';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class PropertyListingService {
  private baseUrl = `${environment.apiUrl}/propertylistings`;

  constructor(private http: HttpClient,private authService:AuthService) {}

  getListings(params?: any): Observable<PagedResult<PropertyListingResponseDto>> {
    // Convert empty strings to undefined
    const cleanParams: any = {};
    for (const key in params) {
      if (params[key] !== '' && params[key] !== null && params[key] !== undefined) {
        cleanParams[key] = params[key];
      }
    }

    console.log(params);

    return this.http.get<any>(this.baseUrl, { params:cleanParams }).pipe(
    
        map(response=>{
          console.log(response.items.$values[0]);
            const mapped: PagedResult<PropertyListingResponseDto>={
                pageNumber:response.pageNumber,
                pageSize:response.pageSize,
                totalCount:response.totalCount,
                items:(response.items?.$values || []).map((item:any)=>this.transformListing(item))
            };
            console.log(mapped);
            return mapped;
        })
    );
  }

  // getListingById(id: string): Observable<PropertyListing> {
  //   return this.http.get<PropertyListing>(`${this.baseUrl}/${id}`);
  // }
  getListingById(id: string): Observable<PagedResult<PropertyListingResponseDto>> {
    return this.getListings({listingId:id});
  }

  createListing(data: CreateListingDto): Observable<PropertyListing> {
    return this.http.post<PropertyListing>(this.baseUrl, data);
  }

  updateListing(id: string, data: UpdateListingDto): Observable<PropertyListing> {
    return this.http.put<PropertyListing>(`${this.baseUrl}/${id}`, data);
  }

  deleteListing(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }


    getMyListings(): Observable<PagedResult<PropertyListingResponseDto>> {
        const currentUser = this.authService.getCurrentUser();
        return this.getListings({ agentId: currentUser!.id });
    }


    private transformListing(raw: any): PropertyListingResponseDto {
    return {
        id: raw.id,
        title: raw.title,
        description: raw.description,
        price: raw.price,
        location: raw.location,
        bedrooms: raw.bedrooms,
        bathrooms: raw.bathrooms,
        squareFeet: raw.squareFeet,
        propertyType:raw.propertyType,
        listingType:raw.listingType,
        isPetsAllowed:raw.isPetsAllowed,
        status:raw.status,
        hasParking:raw.hasParking,
        agentId: raw.agentId,
        name:raw.name,
        email:raw.email,
        licenseNumber:raw.licenseNumber,
        agencyName:raw.agencyName,
        phone:raw.phone,
        imageUrls:raw.imageUrls?.$values || [],
        // agent: raw.agent, // Optional: you may normalize this too
        createdAt: new Date(raw.createdAt),
        isDeleted: raw.isDeleted,
        // images: raw.images?.$values || [],
        // inquiries: raw.inquiries?.$values || []
    };
    }


}
