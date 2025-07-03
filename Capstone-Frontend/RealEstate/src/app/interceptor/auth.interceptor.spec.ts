// import { TestBed } from '@angular/core/testing';
// import { HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
// import { of, throwError } from 'rxjs';
// import { authInterceptor } from './auth.interceptor';
// import { AuthService } from '../services/auth.service';
// import { Router } from '@angular/router';
// import { AuthResponseDto } from "../models/auth-dto.model";

// describe('AuthInterceptor', () => {
//   let mockAuthService: jasmine.SpyObj<AuthService>;
//   let mockRouter: jasmine.SpyObj<Router>;
//   const mockNextHandler = jasmine.createSpy().and.callFake((req: HttpRequest<any>) => of(req));

//   beforeEach(() => {
//     mockAuthService = jasmine.createSpyObj('AuthService', [
//       'getAccessToken',
//       'getRefreshToken',
//       'refreshToken',
//       'logout'
//     ]);
//     mockRouter = jasmine.createSpyObj('Router', ['navigate']);

//     TestBed.configureTestingModule({
//       providers: [
//         { provide: AuthService, useValue: mockAuthService },
//         { provide: Router, useValue: mockRouter }
//       ]
//     });
//   });

//   it('should add authorization header when token exists', () => {
//     mockAuthService.getAccessToken.and.returnValue('valid-token');
//     const testRequest = new HttpRequest('GET', '/api/data');
    
//     // Wrap the interceptor call in runInInjectionContext
//     TestBed.runInInjectionContext(() => {
//       authInterceptor(testRequest, mockNextHandler).subscribe();
//     });
    
//     const interceptedRequest = mockNextHandler.calls.mostRecent().args[0];
//     expect(interceptedRequest.headers.get('Authorization')).toBe('Bearer valid-token');
//   });

//   it('should attempt token refresh on 401 error', () => {
//     // Initial request with expired token
//     mockAuthService.getAccessToken.and.returnValue('expired-token');
//     mockAuthService.getRefreshToken.and.returnValue('refresh-token');
    
//     // First request fails with 401
//     const error401 = new HttpErrorResponse({ status: 401 });
//     const mockFailingHandler = jasmine.createSpy().and.returnValue(throwError(() => error401));
    
//     // Create a properly typed mock response
//     const mockAuthResponse: AuthResponseDto = {
//       token: 'new-token',
//       refreshToken: 'new-refresh-token',
//       email: 'test@example.com',
//       role: 'User'
//     };
    
//     // Mock successful refresh with proper type
//     mockAuthService.refreshToken.and.returnValue(of(mockAuthResponse));
    
//     const testRequest = new HttpRequest('GET', '/api/protected');
    
//     // Wrap the interceptor call in runInInjectionContext
//     TestBed.runInInjectionContext(() => {
//       authInterceptor(testRequest, mockFailingHandler).subscribe();
//     });
    
//     expect(mockAuthService.refreshToken).toHaveBeenCalledWith('refresh-token');
//   });
// });