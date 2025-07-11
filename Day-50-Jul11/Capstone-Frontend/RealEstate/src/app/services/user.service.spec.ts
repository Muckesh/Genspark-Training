import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user.service';
import { environment } from '../../environments/environment';
import { User } from '../models/user.model';
import { ChangePasswordDto } from '../models/change-password-dto';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  const baseUrl = `${environment.apiUrl}/user`;

  const mockUser: User = {
    id: '123',
    name: 'John Doe',
    email: 'john@example.com',
    role: 'Admin',
    isDeleted: false,
    createdAt: new Date(),
    agentProfile: undefined,
    buyerProfile: undefined
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all users with params', () => {
    const queryParams = { pageNumber: 1, pageSize: 10 };
    const mockResponse = {
      pageNumber: 1,
      pageSize: 10,
      totalCount: 1,
      items: { $values: [mockUser] }
    };

    service.getAllUsers(queryParams).subscribe((res) => {
      expect(res.items.length).toBe(1);
      expect(res.items[0]).toEqual(mockUser);
    });

    const req = httpMock.expectOne((r) => r.url === baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should update a user', () => {
    const updatePayload = { name: 'Jane Doe' };

    service.updateUser('123', updatePayload).subscribe((res) => {
      expect(res.name).toBe('Jane Doe');
    });

    const req = httpMock.expectOne(`${baseUrl}/123`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatePayload);
    req.flush({ ...mockUser, ...updatePayload });
  });

  it('should delete a user', () => {
    service.deleteUser('123').subscribe((res) => {
      expect(res).toBeTruthy();
    });

    const req = httpMock.expectOne(`${baseUrl}/123`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ success: true });
  });

  it('should change user password', () => {
    const dto: ChangePasswordDto = {
      oldPassword: 'oldpass',
      newPassword: 'newpass'
    };

    service.changePassword('123', dto).subscribe((res) => {
      expect(res).toEqual({ message: 'Password changed successfully' });
    });

    const req = httpMock.expectOne(`${baseUrl}/change-password/123`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush({ message: 'Password changed successfully' });
  });
});
