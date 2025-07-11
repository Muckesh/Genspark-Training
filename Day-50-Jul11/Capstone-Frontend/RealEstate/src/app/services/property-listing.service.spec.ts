import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PropertyListingService } from './property-listing.service';
import { AuthService } from './auth.service';
import { PropertyListingResponseDto, CreateListingDto, UpdateListingDto } from '../models/property-listing.model';
import { environment } from '../../environments/environment';

describe('PropertyListingService', () => {
  let service: PropertyListingService;
  let httpMock: HttpTestingController;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  const baseUrl = `${environment.apiUrl}/propertylistings`;

  const mockUser = { id: 'agent123' };

  const mockListingRaw = {
    id: '1',
    title: 'Luxury Villa',
    description: 'Beautiful villa',
    price: 1000000,
    location: 'Chennai',
    bedrooms: 4,
    bathrooms: 3,
    squareFeet: 3000,
    propertyType: 'Villa',
    listingType: 'Sale',
    isPetsAllowed: true,
    hasParking: true,
    status: 'Available',
    agentId: 'agent123',
    name: 'John Agent',
    email: 'john@agent.com',
    licenseNumber: 'LIC123',
    agencyName: 'Dream Realty',
    phone: '1234567890',
    imageUrls: { $values: ['img1.jpg', 'img2.jpg'] },
    createdAt: new Date().toISOString(),
    isDeleted: false
  };

  const expectedListing: PropertyListingResponseDto = {
    ...mockListingRaw,
    imageUrls: ['img1.jpg', 'img2.jpg'],
    createdAt: new Date(mockListingRaw.createdAt)
  };

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['getCurrentUser']);
    authServiceSpy.getCurrentUser.and.returnValue(mockUser);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        PropertyListingService,
        { provide: AuthService, useValue: authServiceSpy }
      ]
    });

    service = TestBed.inject(PropertyListingService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch and transform listings', () => {
    const mockResponse = {
      pageNumber: 1,
      pageSize: 10,
      totalCount: 1,
      items: { $values: [mockListingRaw] }
    };

    service.getListings({ location: 'Chennai' }).subscribe(result => {
      expect(result.totalCount).toBe(1);
      expect(result.items[0]).toEqual(expectedListing);
    });

    const req = httpMock.expectOne((r) =>
      r.url === baseUrl && r.params.get('location') === 'Chennai'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should create a listing', () => {
    const createDto: CreateListingDto = {
      title: 'New Home',
      description: 'Nice house',
      price: 500000,
      location: 'Coimbatore',
      bedrooms: 3,
      bathrooms: 2,
      squareFeet: 1500,
      agentId: 'agent123',
      propertyType: 'Apartment',
      listingType: 'Rent',
      isPetsAllowed: false,
      status: 'Available',
      hasParking: true
    };

    service.createListing(createDto).subscribe(res => {
      expect(res.title).toBe('New Home');
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createDto);
    req.flush({ ...createDto, id: '10', createdAt: new Date(), isDeleted: false });
  });

  it('should update a listing', () => {
    const updateDto: UpdateListingDto = {
      title: 'Updated Home',
      description: 'Updated description',
      price: 550000,
      location: 'Salem',
      bedrooms: 3,
      bathrooms: 2,
      squareFeet: 1600,
      propertyType: 'Apartment',
      listingType: 'Sale',
      isPetsAllowed: true,
      status: 'Sold',
      hasParking: false
    };

    service.updateListing('123', updateDto).subscribe(res => {
      expect(res.title).toBe('Updated Home');
    });

    const req = httpMock.expectOne(`${baseUrl}/123`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updateDto);
    req.flush({ ...updateDto, id: '123', createdAt: new Date(), isDeleted: false });
  });

  it('should delete a listing', () => {
    service.deleteListing('456').subscribe(res => {
      expect(res.message).toBe('Deleted');
    });

    const req = httpMock.expectOne(`${baseUrl}/456`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ message: 'Deleted' });
  });

  it('should get my listings using current user ID', () => {
    const mockResponse = {
      pageNumber: 1,
      pageSize: 10,
      totalCount: 1,
      items: { $values: [mockListingRaw] }
    };

    service.getMyListings().subscribe(result => {
      expect(result.items.length).toBe(1);
      expect(result.items[0].agentId).toBe(mockUser.id);
    });

    const req = httpMock.expectOne((r) =>
      r.url === baseUrl && r.params.get('agentId') === mockUser.id
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
