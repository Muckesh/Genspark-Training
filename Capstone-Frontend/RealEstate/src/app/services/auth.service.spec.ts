import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

import {
  LoginDto,
  RegisterAgentDto,
  RegisterBuyerDto,
  AuthResponseDto,
  RefreshTokenRequestDto,
  LogoutRequestDto
} from '../models/auth-dto.model';

import { User } from '../models/user.model';
import { environment } from '../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  const apiUrl = `${environment.apiUrl}`;

  const dummyToken = 'dummy.jwt.token';
  const dummyRefreshToken = 'refresh123';

  const authResponse: AuthResponseDto = {
    email: 'agent@test.com',
    role: 'Agent',
    token: dummyToken,
    refreshToken: dummyRefreshToken
  };

  const mockUser: User = {
    id: 'u1',
    name: 'Agent Smith',
    email: 'agent@test.com',
    role: 'Agent',
    isDeleted: false,
    agentProfile: {
      id: 'a1',
      licenseNumber: 'LIC123',
      agencyName: 'Real Estate Co.',
      phone: '1234567890',
      user: undefined as any
    }
  };

  beforeEach(() => {
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: Router, useValue: routerMock }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login and store tokens and fetch user', () => {
    const loginDto: LoginDto = {
      email: 'agent@test.com',
      password: 'secret'
    };

    service.login(loginDto).subscribe(user => {
      expect(user.email).toBe('agent@test.com');
      expect(localStorage.getItem('accessToken')).toBe(authResponse.token);
      expect(localStorage.getItem('refreshToken')).toBe(authResponse.refreshToken);
    });

    const loginReq = httpMock.expectOne(`${apiUrl}/auth/login`);
    expect(loginReq.request.method).toBe('POST');
    expect(loginReq.request.body).toEqual(loginDto);
    loginReq.flush(authResponse);

    const userReq = httpMock.expectOne(`${apiUrl}/auth/me`);
    expect(userReq.request.method).toBe('GET');
    userReq.flush(mockUser);
  });

  it('should register an agent and store tokens and fetch user', () => {
    const agentDto: RegisterAgentDto = {
      name: 'Agent Smith',
      email: 'agent@test.com',
      password: 'secret',
      licenseNumber: 'LIC123',
      agencyName: 'Real Estate Co.',
      phone: '1234567890'
    };

    service.registerAgent(agentDto).subscribe(user => {
      expect(user.email).toBe('agent@test.com');
    });

    const registerReq = httpMock.expectOne(`${apiUrl}/agents/register`);
    expect(registerReq.request.method).toBe('POST');
    expect(registerReq.request.body).toEqual(agentDto);
    registerReq.flush(authResponse);

    const userReq = httpMock.expectOne(`${apiUrl}/auth/me`);
    userReq.flush(mockUser);
  });

  it('should register a buyer and store tokens and fetch user', () => {
    const buyerDto: RegisterBuyerDto = {
      name: 'Buyer One',
      email: 'buyer@test.com',
      password: 'buyerpass',
      phone: '9876543210',
      preferredLocation: 'Chennai',
      budget: 5000000
    };

    const mockBuyerUser: User = {
      id: 'u2',
      name: 'Buyer One',
      email: 'buyer@test.com',
      role: 'Buyer',
      isDeleted: false,
      buyerProfile: {
        id: 'b1',
        phone: '9876543210',
        preferredLocation: 'Chennai',
        budget: 5000000,
        user: undefined as any
      }
    };

    service.registerBuyer(buyerDto).subscribe(user => {
      expect(user.email).toBe('buyer@test.com');
    });

    const registerReq = httpMock.expectOne(`${apiUrl}/buyers/register`);
    expect(registerReq.request.method).toBe('POST');
    expect(registerReq.request.body).toEqual(buyerDto);
    registerReq.flush(authResponse);

    const userReq = httpMock.expectOne(`${apiUrl}/auth/me`);
    userReq.flush(mockBuyerUser);
  });

  it('should refresh token and store new tokens', () => {
    service.refreshToken(dummyRefreshToken).subscribe(response => {
      expect(response.token).toBe(dummyToken);
      expect(localStorage.getItem('accessToken')).toBe(dummyToken);
    });

    const refreshReq = httpMock.expectOne(`${apiUrl}/auth/refresh`);
    expect(refreshReq.request.body).toEqual({ refreshToken: dummyRefreshToken } as RefreshTokenRequestDto);
    refreshReq.flush(authResponse);
  });

  it('should logout, clear tokens and navigate to login', () => {
    localStorage.setItem('accessToken', dummyToken);
    localStorage.setItem('refreshToken', dummyRefreshToken);
    localStorage.setItem('currentUser', JSON.stringify(mockUser));

    service.logout();

    const logoutReq = httpMock.expectOne(`${apiUrl}/auth/logout`);
    expect(logoutReq.request.body).toEqual({ refreshToken: dummyRefreshToken } as LogoutRequestDto);

    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('currentUser')).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['login']);
  });

  it('should return the current user', () => {
    service.setCurrentUser(mockUser);
    expect(service.getCurrentUser()).toEqual(mockUser);
  });

//   it('should check authentication using valid token', () => {
//     const jwtHelper = new JwtHelperService();
//     spyOn(jwtHelper, 'isTokenExpired').and.returnValue(false);
//     (service as any).jwtHelper = jwtHelper;

//     localStorage.setItem('accessToken', dummyToken);
//     expect(service.isAuthenticated()).toBeTrue();
//   });

  it('should return false when no token is found', () => {
    expect(service.isAuthenticated()).toBeFalse();
  });

  it('should check if user has a role', () => {
    service.setCurrentUser(mockUser);
    expect(service.hasRole('Agent')).toBeTrue();
    expect(service.hasRole('Admin')).toBeFalse();
  });

  it('should decode token', () => {
    const jwtHelper = new JwtHelperService();
    spyOn(jwtHelper, 'decodeToken').and.returnValue({ sub: 'u1', role: 'Agent' });
    (service as any).jwtHelper = jwtHelper;

    const decoded = service.decodeToken(dummyToken);
    expect(decoded.role).toBe('Agent');
  });
});
