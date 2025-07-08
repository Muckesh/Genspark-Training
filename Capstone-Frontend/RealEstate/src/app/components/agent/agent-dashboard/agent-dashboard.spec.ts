import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AgentDashboard } from './agent-dashboard';
import { PropertyListingService } from '../../../services/property-listing.service';
import { InquiryService } from '../../../services/inquiry.service';
import { AuthService } from '../../../services/auth.service';
import { RouterTestingModule } from '@angular/router/testing';
import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { forkJoin, of, throwError } from 'rxjs';
import { PagedResult } from '../../../models/paged-result.model';
import { PropertyListingResponseDto } from '../../../models/property-listing.model';
import { InquiryResponseDto } from '../../../models/inquiry.model';

describe('AgentDashboard', () => {
  let component: AgentDashboard;
  let fixture: ComponentFixture<AgentDashboard>;
  let listingServiceSpy: jasmine.SpyObj<PropertyListingService>;
  let inquiryServiceSpy: jasmine.SpyObj<InquiryService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  const mockListings: PropertyListingResponseDto[] = [
    {
      id: '1',
      title: 'Luxury Villa',
      description: 'Beautiful villa with pool',
      price: 500000,
      location: 'Beverly Hills',
      bedrooms: 4,
      bathrooms: 3,
      squareFeet: 3500,
      propertyType: 'House',
      listingType: 'Sale',
      isPetsAllowed: true,
      status: 'Available',
      hasParking: true,
      agentId: 'agent1',
      imageUrls: ['img1.jpg', 'img2.jpg'],
      name: 'John Doe',
      email: 'john@example.com',
      licenseNumber: 'LIC123',
      agencyName: 'Elite Properties',
      phone: '123-456-7890',
      createdAt: new Date(),
      isDeleted: false
    }
  ];

  const mockPagedResult: PagedResult<PropertyListingResponseDto> = {
    items: mockListings,
    pageNumber: 1,
    pageSize: 10,
    totalCount: 1,
  };

  const mockInquiryResult = {
    items: [{ id: '1', message: 'Test inquiry' }],
    totalCount: 1
  };

  const mockUser = {
    id: 'agent1',
    name: 'John Doe',
    email: 'john@example.com',
    role: 'Agent'
  };

  beforeEach(async () => {
    listingServiceSpy = jasmine.createSpyObj('PropertyListingService', [
      'getMyListings',
      'getListings'
    ]);

    inquiryServiceSpy = jasmine.createSpyObj('InquiryService', [
      'getAgentInquiries'
    ]);

    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'getCurrentUser',
      'hasRole',
      'isAuthenticated'
    ]);

    // Mock AuthService methods
    authServiceSpy.getCurrentUser.and.returnValue(mockUser);
    authServiceSpy.hasRole.and.callFake((role: string) => role === 'Agent');
    authServiceSpy.isAuthenticated.and.returnValue(true);

    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        RouterTestingModule,
        HttpClientTestingModule,
        AgentDashboard
      ],
      providers: [
        { provide: PropertyListingService, useValue: listingServiceSpy },
        { provide: InquiryService, useValue: inquiryServiceSpy },
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AgentDashboard);
    component = fixture.componentInstance;

    // Setup mock responses
    listingServiceSpy.getMyListings.and.returnValue(of(mockPagedResult));
    listingServiceSpy.getListings.and.returnValue(of(mockPagedResult));
    inquiryServiceSpy.getAgentInquiries.and.returnValue(of(mockInquiryResult as PagedResult<InquiryResponseDto>));

    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
    expect(authServiceSpy.getCurrentUser).toHaveBeenCalled();
  });

  it('should load dashboard data and update stats correctly', fakeAsync(() => {
    component.ngOnInit();
    tick();

    expect(authServiceSpy.hasRole).toHaveBeenCalled();
    expect(listingServiceSpy.getMyListings).toHaveBeenCalled();
    expect(inquiryServiceSpy.getAgentInquiries).toHaveBeenCalled();
    expect(listingServiceSpy.getListings).toHaveBeenCalled();

    expect(component.stats[0].value).toBe(1); // My Listings
    expect(component.stats[1].value).toBe(1); // My Inquiries
    expect(component.stats[2].value).toBe(1); // Available Properties
    expect(component.stats[3].value).toBe(1); // Active Listings

    expect(component.myListings.length).toBe(1);
    expect(component.recentListings.length).toBe(1);
    expect(component.isStatsLoading).toBeFalse();
  }));

  it('should handle errors when loading dashboard data', fakeAsync(() => {
    const errorResponse = new Error('Failed to load');
    listingServiceSpy.getMyListings.and.returnValue(throwError(() => errorResponse));

    component.loadDashboardData();
    tick();

    expect(component.error).toBe('Failed to load dashboard data. Please try again.');
    expect(component.isStatsLoading).toBeFalse();
  }));

  it('should return correct action links for stats', () => {
    expect(component.getStatActionLink('My Listings')).toBe('/agent/listings');
    expect(component.getStatActionLink('My Inquiries')).toBe('/agent/inquiries');
    expect(component.getStatActionLink('Available Properties')).toBe('/agent/browse-listings');
    expect(component.getStatActionLink('Unknown Stat')).toBe('/agent/listings');
  });

  it('should properly track items in lists', () => {
    const stat = { label: 'Test Stat', value: 1, icon: '', color: '' };
    const property = mockListings[0];

    expect(component.trackByStat(0, stat)).toBe('Test Stat');
    expect(component.trackByProperty(0, property)).toBe('1');
  });
});