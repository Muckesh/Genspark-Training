import {
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
  HttpErrorResponse,
  HttpResponse
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import {
  BehaviorSubject,
  Observable,
  catchError,
  filter,
  switchMap,
  take,
  throwError
} from 'rxjs';

// Shared token refresh state
let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Don't add token to login/register/refresh requests
  if (
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') 
    // req.url.includes('/auth/refresh')
  ) {
    return next(req);
  }

  const token = authService.getAccessToken();

  // If access token exists, add it to the request
  const authReq = token ? addTokenHeader(req, token) : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (
        error.status === 401 &&
        !req.url.includes('/auth/refresh') &&
        token
      ) {
        return handle401Error(authReq, next, authService, router);
      }

      return throwError(() => error) as Observable<HttpEvent<any>>;
    })
  );
};

function addTokenHeader(
  request: HttpRequest<any>,
  token: string
): HttpRequest<any> {
  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });
}

function handle401Error(
  request: HttpRequest<any>,
  next: HttpHandlerFn,
  authService: AuthService,
  router: Router
): Observable<HttpEvent<any>> {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    const refreshToken = authService.getRefreshToken();

    if (!refreshToken) {
      authService.logout();
      return throwError(() => new Error('No refresh token')) as Observable<HttpEvent<any>>;
    }

    return authService.refreshToken(refreshToken).pipe(
      switchMap((response) => {
        isRefreshing = false;
        authService.storeTokens(response);
        refreshTokenSubject.next(response.token);

        const updatedRequest = addTokenHeader(request, response.token);
        return next(updatedRequest);
      }),
      catchError((err) => {
        isRefreshing = false;
        authService.logout();
        router.navigate(['/login']);
        return throwError(() => err) as Observable<HttpEvent<any>>;
      })
    );
  } else {
    // If refreshing, wait until it's done
    return refreshTokenSubject.pipe(
      filter((token): token is string => token !== null),
      take(1),
      switchMap((newToken) => {
        const updatedRequest = addTokenHeader(request, newToken);
        return next(updatedRequest);
      })
    );
  }
}

