import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { AdminDashboard } from './admin-dashboard';
import { UserService } from '../../../services/user.service';
import { PropertyListingService } from '../../../services/property-listing.service';
import { InquiryService } from '../../../services/inquiry.service';
import { PagedResult } from '../../../models/paged-result.model';
import { User } from '../../../models/user.model';
import { PropertyListingResponseDto } from '../../../models/property-listing.model';
import { InquiryResponseDto } from '../../../models/inquiry.model';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

const mockUsers = {
  items: [
    { id: '1', role: 'Agent', createdAt: new Date(), name: 'John', email: 'john@example.com', isDeleted: false },
    { id: '2', role: 'Buyer', createdAt: new Date(), name: 'Jane', email: 'jane@example.com', isDeleted: false }
  ],
  pageNumber: 1,
  pageSize: 10,
  totalCount: 2
};

const mockProperties = {
  items: [
    { id: '101', status: 'Sold', isDeleted: false, price: 200000, createdAt: new Date() }
  ],
  pageNumber: 1,
  pageSize: 10,
  totalCount: 1
};

const mockInquiries = {
  items: [
    { id: '201', status: 'Pending', createdAt: new Date() }
  ],
  pageNumber: 1,
  pageSize: 10,
  totalCount: 1
};

describe('AdminDashboard', () => {
  let component: AdminDashboard;
  let fixture: ComponentFixture<AdminDashboard>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let propertyListingServiceSpy: jasmine.SpyObj<PropertyListingService>;
  let inquiryServiceSpy: jasmine.SpyObj<InquiryService>;
  

  beforeEach(waitForAsync(() => {
    userServiceSpy = jasmine.createSpyObj('UserService', ['getAllUsers', 'deleteUser']);
    propertyListingServiceSpy = jasmine.createSpyObj('PropertyListingService', ['getListings']);
    inquiryServiceSpy = jasmine.createSpyObj('InquiryService', ['getInquiries']);
    const mockActivatedRoute = {
    snapshot:{
      params:{}
    }
  };

    TestBed.configureTestingModule({
      imports: [AdminDashboard,CommonModule,FormsModule,DatePipe,RouterLink], 
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: PropertyListingService, useValue: propertyListingServiceSpy },
        { provide: InquiryService, useValue: inquiryServiceSpy },
        {provide: ActivatedRoute,useValue:mockActivatedRoute}
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminDashboard);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load dashboard data and process it successfully', () => {
    userServiceSpy.getAllUsers.and.returnValue(of(mockUsers as PagedResult<User>));
    propertyListingServiceSpy.getListings.and.returnValue(of(mockProperties as PagedResult<PropertyListingResponseDto>));
    inquiryServiceSpy.getInquiries.and.returnValue(of(mockInquiries as PagedResult<InquiryResponseDto>));

    component.loadDashboardData();

    expect(component.stats[0].value).toBe(2); // Total users
    expect(component.stats[1].value).toBe(1); // Total properties
    expect(component.stats[2].value).toBe(1); // Total inquiries
  });

  it('should handle error when loading dashboard data', () => {
    userServiceSpy.getAllUsers.and.returnValue(throwError(() => new Error('Load failed')));
    propertyListingServiceSpy.getListings.and.returnValue(of(mockProperties as PagedResult<PropertyListingResponseDto>));
    inquiryServiceSpy.getInquiries.and.returnValue(of(mockInquiries as PagedResult<InquiryResponseDto>));

    component.loadDashboardData();

    expect(component.error).toContain('Failed to load dashboard data');
    expect(component.isStatsLoading).toBeFalse();
  });

  it('should delete user and update stats', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    userServiceSpy.deleteUser.and.returnValue(of({}));
    component.users = [...mockUsers.items as User[]];
    component.stats[0].value = 2;

    component.deleteUser('1');

    expect(component.users.length).toBe(1);
    expect(userServiceSpy.deleteUser).toHaveBeenCalledWith('1');
  });

  it('should return correct role badge class', () => {
    expect(component.getRoleBadgeClass('Admin')).toBe('bg-danger');
    expect(component.getRoleBadgeClass('Agent')).toBe('bg-success');
    expect(component.getRoleBadgeClass('Buyer')).toBe('bg-primary');
    expect(component.getRoleBadgeClass('Unknown')).toBe('bg-secondary');
  });
});
