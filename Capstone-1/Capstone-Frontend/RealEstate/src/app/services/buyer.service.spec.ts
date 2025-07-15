import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { BuyerService } from './buyer.service';
import { environment } from '../../environments/environment';
import { Buyer, UpdateBuyerDto } from '../models/buyer.model';

describe('BuyerService', () => {
  let service: BuyerService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/buyers`;

  const mockBuyer: Buyer = {
    id: 'b1',
    phone: '9876543210',
    preferredLocation: 'Chennai',
    budget: 7500000,
    user: {
      id: 'u1',
      name: 'Buyer One',
      email: 'buyer@example.com',
      role: 'Buyer',
      isDeleted: false
    },
    inquiries: []
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BuyerService]
    });

    service = TestBed.inject(BuyerService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get buyers with query params', () => {
    const query = { pageNumber: 1, pageSize: 10, searchTerm: 'chennai' };
    const mockResponse = {
      pageNumber: 1,
      pageSize: 10,
      totalCount: 1,
      items: [mockBuyer]
    };

    service.getBuyers(query).subscribe(response => {
      expect(response.totalCount).toBe(1);
      expect(response.items[0].id).toBe('b1');
      expect(response.items[0].preferredLocation).toBe('Chennai');
    });

    const req = httpMock.expectOne(r =>
      r.method === 'GET' &&
      r.url === apiUrl &&
      r.params.get('pageNumber') === '1' &&
      r.params.get('pageSize') === '10' &&
      r.params.get('searchTerm') === 'chennai'
    );

    req.flush(mockResponse);
  });

  it('should update a buyer', () => {
    const updatePayload: UpdateBuyerDto = {
      phone: '9999999999',
      preferredLocation: 'Coimbatore',
      budget: '6000000' // string as per UpdateBuyerDto
    };

    const updatedBuyer: Buyer = {
      ...mockBuyer,
      phone: updatePayload.phone,
      preferredLocation: updatePayload.preferredLocation,
      budget: parseInt(updatePayload.budget, 10)
    };

    service.updateBuyer('b1', updatePayload).subscribe(buyer => {
      expect(buyer.phone).toBe('9999999999');
      expect(buyer.preferredLocation).toBe('Coimbatore');
      expect(buyer.budget).toBe(6000000);
    });

    const req = httpMock.expectOne(`${apiUrl}/b1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatePayload); // string budget is okay
    req.flush(updatedBuyer);
  });
});
