import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AdminListingsManagement } from './admin-listings-management';
import { PropertyListingService } from '../../../services/property-listing.service';
import { PagedResult } from '../../../models/paged-result.model';
import { PropertyListingResponseDto } from '../../../models/property-listing.model';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';

describe('AdminListingsManagement', () => {
  let component: AdminListingsManagement;
  let fixture: ComponentFixture<AdminListingsManagement>;
  let listingServiceSpy: jasmine.SpyObj<PropertyListingService>;

  const mockListings: PropertyListingResponseDto[] = [
    { id: '1', title: 'Listing 1', price: 100000 } as PropertyListingResponseDto,
    { id: '2', title: 'Listing 2', price: 200000 } as PropertyListingResponseDto
  ];

  const mockPagedResult: PagedResult<PropertyListingResponseDto> = {
    items: mockListings,
    pageNumber: 1,
    pageSize: 10,
    totalCount: 2,
  };

  const mockActivatedRoute = {
    snapshot:{
      params:{}
    }
  }

  beforeEach(async () => {
    listingServiceSpy = jasmine.createSpyObj('PropertyListingService', [
      'getListings',
      'deleteListing'
    ]);

    listingServiceSpy.getListings.and.returnValue(of(mockPagedResult));
    listingServiceSpy.deleteListing.and.returnValue(of({}));

    await TestBed.configureTestingModule({
      imports: [AdminListingsManagement, RouterLink, CommonModule, FormsModule],
      providers: [
        { provide: PropertyListingService, useValue: listingServiceSpy },
        { provide: ActivatedRoute, useValue:mockActivatedRoute}
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminListingsManagement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load listings on init', fakeAsync(() => {
    // Trigger ngOnInit
    component.ngOnInit();
    tick(300); // Wait for debounceTime

    expect(listingServiceSpy.getListings).toHaveBeenCalledWith({
      pageNumber: 1,
      pageSize: 10
    });
    expect(component.listings).toEqual(mockListings);
    expect(component.paginationInfo.totalCount).toBe(2);
    expect(component.isLoading).toBeFalse();
  }));

  it('should handle error when loading listings fails', fakeAsync(() => {
    const errorResponse = new Error('Failed to load');
    listingServiceSpy.getListings.and.returnValue(throwError(() => errorResponse));

    component.ngOnInit();
    tick(300); // Wait for debounceTime

    expect(component.error).toBe('Failed to load listings. Please try again later.');
    expect(component.isLoading).toBeFalse();
  }));

});