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






// import { HttpErrorResponse, HttpHandler, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
// import { inject } from "@angular/core";
// import { AuthService } from "../services/auth.service";
// import { Router } from "@angular/router";
// import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from "rxjs";

// export const authInterceptor: HttpInterceptorFn=(req,next)=>{
//     const authService = inject(AuthService);
//     const router = inject(Router);
//     const isRefreshing = false;
//     const refreshTokenSubject = new BehaviorSubject<any>(null);

//     // skip adding token for auth requests
//     if( !req.url.includes('/auth/me')&& req.url.includes('/auth/'))
//         return next(req);

//     const accessToken = authService.getAccessToken();

//     // If no token but trying to access protected route, redirect to login
//     if (!accessToken && !req.url.includes('/auth/me')) {
//         router.navigate(['/login']);
//         return throwError(() => new Error('No access token'));
//     }

//     // only add token if available
//     if(accessToken)
//         req=addTokenHeader(req,accessToken);

//     return next(req).pipe(
//         catchError((error)=>{
//             if(error instanceof HttpErrorResponse && error.status === 401 && !req.url.includes('/auth/refresh') && accessToken){
//                 return handle401Error(req,next,authService,router,isRefreshing,refreshTokenSubject);
//             }
//             return throwError(()=>error);
//         })
//     );
// };

// function addTokenHeader(request:HttpRequest<any>,token:string){
//     return request.clone({
//         setHeaders:{
//             Authorization: `Bearer ${token}`
//         }
//     });
// }

// function handle401Error(request:HttpRequest<any>, next:HttpHandlerFn,authService:AuthService,router:Router,isRefreshing:boolean,refreshTokenSubject:BehaviorSubject<any>){
//     if(!isRefreshing){
//         isRefreshing=true;
//         refreshTokenSubject.next(null);

//         const refreshToken = authService.getRefreshToken();

//         if(refreshToken){
//             return authService.refreshToken(refreshToken).pipe(
//                 switchMap((response:any)=>{
//                     isRefreshing=false;
//                     refreshTokenSubject.next(response.token);
//                     return next(addTokenHeader(request,response.token));
//                 }),
//                 catchError((err)=>{
//                     isRefreshing=false;
//                     authService.logout();
//                     router.navigate(['login']);
//                     return throwError(()=>err);
//                 })
//             );
//         }
//     }

//     return refreshTokenSubject.pipe(
//         filter(token=>token!==null),
//         take(1),
//         switchMap((token)=> next(addTokenHeader(request,token)))
//     );
// }

// import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
// import { inject } from "@angular/core";
// import { AuthService } from "../services/auth.service";
// import { Router } from "@angular/router";
// import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from "rxjs";

// // Shared state for token refresh
// let isRefreshing = false;
// const refreshTokenSubject = new BehaviorSubject<string | null>(null);

// export const authInterceptor: HttpInterceptorFn = (req, next) => {
//     const authService = inject(AuthService);
//     const router = inject(Router);

//     // Skip token addition for auth endpoints (except auth/me)
//     if (req.url.includes('/auth/') && !req.url.includes('/auth/me')) {
//         return next(req);
//     }

//     const accessToken = authService.getAccessToken();

//     // If no token but accessing protected route, redirect to login
//     if (!accessToken && isProtectedRoute(req.url)) {
//         authService.logout();
//         return throwError(() => new Error('No access token'));
//     }

//     // Add token to request if available
//     const authReq = accessToken ? addTokenHeader(req, accessToken) : req;

//     return next(authReq).pipe(
//         catchError((error) => {
//             if (shouldHandle401Error(error, authReq, accessToken)) {
//                 return handle401Error(authReq, next, authService, router);
//             }
//             return throwError(() => error);
//         })
//     );
// };

// function addTokenHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
//     return request.clone({
//         setHeaders: {
//             Authorization: `Bearer ${token}`
//         }
//     });
// }

// function shouldHandle401Error(
//     error: any,
//     request: HttpRequest<any>,
//     accessToken: string | null
// ): error is HttpErrorResponse {
//     return error instanceof HttpErrorResponse &&
//         error.status === 401 &&
//         !request.url.includes('/auth/refresh') &&
//         !!accessToken;
// }

// function isProtectedRoute(url: string): boolean {
//     const unprotectedRoutes = ['/auth/login', '/auth/register'];
//     return !unprotectedRoutes.some(route => url.includes(route));
// }

// function handle401Error(
//     request: HttpRequest<any>,
//     next: HttpHandlerFn,
//     authService: AuthService,
//     router: Router
// ) {
//     if (!isRefreshing) {
//         isRefreshing = true;
//         refreshTokenSubject.next(null);

//         const refreshToken = authService.getRefreshToken();

//         if (!refreshToken) {
//             authService.logout();
//             return throwError(() => new Error('No refresh token available'));
//         }

//         return authService.refreshToken(refreshToken).pipe(
//             switchMap((response: any) => {
//                 isRefreshing = false;
//                 authService.storeTokens(response);
//                 refreshTokenSubject.next(response.token);
                
//                 // Retry the original request with new token
//                 return next(addTokenHeader(request, response.token));
//             }),
//             catchError((err) => {
//                 isRefreshing = false;
//                 authService.logout();
//                 router.navigate(['/login']);
//                 return throwError(() => err);
//             })
//         );
//     }

//     // If already refreshing, wait for the new token
//     return refreshTokenSubject.pipe(
//         filter(token => token !== null),
//         take(1),
//         switchMap((token) => next(addTokenHeader(request, token!)))
//     );
// }