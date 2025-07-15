import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { InquiryService } from './inquiry.service';
import { Inquiry, InquiryReply, InquiryResponseDto } from '../models/inquiry.model';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

describe('InquiryService', () => {
  let service: InquiryService;
  let httpMock: HttpTestingController;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  const baseUrl = `${environment.apiUrl}/inquiries`;

  const mockInquiry: Inquiry = {
    listingId: 'listing123',
    buyerId: 'buyer456',
    message: 'Is this available?',
    createdAt: new Date(),
    status: 'Pending',
    replies: []
  };

  const inquiryDtoFromApi: any = {
    id: 'i1',
    listingId: 'listing123',
    listingTitle: '2BHK in Mumbai',
    buyerId: 'buyer456',
    buyerName: 'John',
    agentId: 'agent789',
    agentName: 'Agent Smith',
    status: 'Pending',
    message: 'Is this available?',
    createdAt: new Date().toISOString(),
    replies: { $values: [] }
  };

  beforeEach(() => {
    const authMock = jasmine.createSpyObj('AuthService', ['getCurrentUser']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        InquiryService,
        { provide: AuthService, useValue: authMock }
      ]
    });

    service = TestBed.inject(InquiryService);
    httpMock = TestBed.inject(HttpTestingController);
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create an inquiry', () => {
    service.createInquiry(mockInquiry).subscribe(res => {
      expect(res).toEqual(mockInquiry);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockInquiry);
    req.flush(mockInquiry);
  });

  it('should get inquiries and map to InquiryResponseDto', () => {
    const mockApiResponse = {
      pageNumber: 1,
      pageSize: 10,
      totalCount: 1,
      items: { $values: [inquiryDtoFromApi] }
    };

    service.getInquiries({ pageNumber: 1 }).subscribe(res => {
      expect(res.totalCount).toBe(1);
      expect(res.items[0].listingTitle).toBe('2BHK in Mumbai');
      expect(res.items[0].replies.length).toBe(0);
    });

    const req = httpMock.expectOne(r =>
      r.method === 'GET' &&
      r.url === baseUrl &&
      r.params.get('pageNumber') === '1'
    );
    req.flush(mockApiResponse);
  });

  it('should return true if inquiry exists', () => {
    const existingInquiry: Inquiry = {
      ...mockInquiry,
      id: 'i1',
      replies: []
    };

    service.checkIfInquiryExists('listing123', 'buyer456').subscribe(exists => {
      expect(exists).toBeTrue();
    });

    const req = httpMock.expectOne(`${baseUrl}/existing?listingId=listing123&buyerId=buyer456`);
    expect(req.request.method).toBe('GET');
    req.flush(existingInquiry);
  });

  it('should return false if inquiry does not exist (404)', () => {
    service.checkIfInquiryExists('listing123', 'buyer456').subscribe(exists => {
      expect(exists).toBeFalse();
    });

    const req = httpMock.expectOne(`${baseUrl}/existing?listingId=listing123&buyerId=buyer456`);
    expect(req.request.method).toBe('GET');
    req.flush({}, { status: 404, statusText: 'Not Found' });
  });
});
