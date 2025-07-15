import { TestBed } from '@angular/core/testing';
import { TokenService } from './token.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';

describe('TokenService', () => {
  let service: TokenService;
  let httpMock: HttpTestingController;
  let jwtHelperSpy: jasmine.SpyObj<JwtHelperService>;

  const mockResponse = {
    token: 'mock-access-token',
    refreshToken: 'mock-refresh-token'
  };

  beforeEach(() => {
    const jwtSpy = jasmine.createSpyObj('JwtHelperService', ['isTokenExpired']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        TokenService,
        { provide: JwtHelperService, useValue: jwtSpy }
      ]
    });

    service = TestBed.inject(TokenService);
    httpMock = TestBed.inject(HttpTestingController);
    jwtHelperSpy = TestBed.inject(JwtHelperService) as jasmine.SpyObj<JwtHelperService>;
  });

  afterEach(() => {
    localStorage.clear();
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should store and retrieve tokens', () => {
    service.storeTokens(mockResponse);
    expect(service.getAccessToken()).toBe('mock-access-token');
    expect(service.getRefreshToken()).toBe('mock-refresh-token');
  });

  it('should clear tokens', () => {
    service.storeTokens(mockResponse);
    service.clearTokens();
    expect(service.getAccessToken()).toBeNull();
    expect(service.getRefreshToken()).toBeNull();
  });


  it('should refresh token and store new tokens', () => {
    service.refreshToken('old-refresh-token').subscribe((res) => {
      expect(res).toEqual(mockResponse);
      expect(localStorage.getItem('accessToken')).toBe(mockResponse.token);
      expect(localStorage.getItem('refreshToken')).toBe(mockResponse.refreshToken);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/refresh`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'old-refresh-token' });

    req.flush(mockResponse);
  });
});
