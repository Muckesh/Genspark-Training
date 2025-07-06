import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { BehaviorSubject, Observable, switchMap, tap } from "rxjs";
import { JwtHelperService } from "@auth0/angular-jwt";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Router } from "@angular/router";
import { AuthResponseDto, LoginDto, LogoutRequestDto, RefreshTokenRequestDto, RegisterAgentDto, RegisterBuyerDto } from "../models/auth-dto.model";

@Injectable()
export class AuthService{
    private readonly baseUrl = environment.apiUrl;
    public currentUserSubject = new BehaviorSubject<any>(null);
    public currentUser$ = this.currentUserSubject.asObservable();
    private jwtHelper = new JwtHelperService();

    constructor(private http:HttpClient,private router:Router){
        this.initializeAuthState();
    }

    private initializeAuthState():void{
        const token = this.getAccessToken();
        if(token&&!this.isTokenExpired(token)){
            // current user from local storage
            const storedUser = localStorage.getItem('currentUser');
            if(storedUser){
                try{
                    const user = JSON.parse(storedUser);
                    this.currentUserSubject.next(user);
                }catch(e){
                    this.fetchAndStoreUser();
                }
            }else{
                this.fetchAndStoreUser();
            }

            // const user = this.decodeToken(token);
        }
    }

    private fetchAndStoreUser():void{
        this.http.get<any>(`${this.baseUrl}/auth/me`).subscribe({
            next:(user)=>{
                this.storeUser(user);
                this.currentUserSubject.next(user);
            },
            error:()=> this.clearAuthState()
        });
    }

    private storeUser(user:any):void{
        localStorage.setItem('currentUser',JSON.stringify(user));
    }

    private clearAuthState():void{
        this.clearTokens();
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }

    login(credentials:LoginDto):Observable<AuthResponseDto>{
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/auth/login`,credentials).pipe(
            tap(response=>{
                this.storeTokens(response);
            }),
            switchMap(() => this.http.get<any>(`${this.baseUrl}/auth/me`)),
            tap(user=>{
                this.storeUser(user);
                this.currentUserSubject.next(user);
            })
        );
    }

    registerBuyer(registerData:RegisterBuyerDto):Observable<AuthResponseDto>{
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/buyers/register`,registerData)
            .pipe(
                tap(response=>{
                    this.storeTokens(response);
                   
                }),
                switchMap(() => this.http.get<any>(`${this.baseUrl}/auth/me`)),
                tap(user=>{
                    this.storeUser(user);
                    this.currentUserSubject.next(user);
                })

            );
    }

    registerAgent(registerData:RegisterAgentDto):Observable<AuthResponseDto>{
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/agents/register`,registerData)
            .pipe(
                tap(response=>{
                    this.storeTokens(response);
                   
                }),
                switchMap(() => this.http.get<any>(`${this.baseUrl}/auth/me`)),
                tap(user=>{
                    this.storeUser(user);
                    this.currentUserSubject.next(user);
                })
            );
    }

    refreshToken(refreshToken:string):Observable<AuthResponseDto>{
        const request:RefreshTokenRequestDto = {refreshToken};
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/auth/refresh`,request)
            .pipe(
                tap(response=>{this.storeTokens(response)})
            );
    }

    logout():void{
        const refreshToken = this.getRefreshToken();
        if(refreshToken){
            const request:LogoutRequestDto = {refreshToken};
            this.http.post(`${this.baseUrl}/auth/logout`,request).subscribe({
                error:err=>{console.log(err);
                }
            });
        }
        // this.clearTokens();
        // this.currentUserSubject.next(null);
        this.clearAuthState();
        this.router.navigate(["login"]);
    }

    getCurrentUser():any{
        return this.currentUserSubject.value;
    }

    setCurrentUser(user: any): void {
        this.currentUserSubject.next(user);
        localStorage.setItem('currentUser', JSON.stringify(user));
    }


    isAuthenticated():boolean{
        const token = this.getAccessToken();
        return !!token && !this.isTokenExpired(token);
    }

    hasRole(role:string):boolean{
        const user = this.getCurrentUser();
        return user?.role===role;
    }

    storeTokens(response:AuthResponseDto):void{
        localStorage.setItem('accessToken',response.token);
        localStorage.setItem('refreshToken',response.refreshToken);
    }

    private clearTokens():void{
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
    }

    getAccessToken():string|null{
        return localStorage.getItem('accessToken');
    }

    getRefreshToken():string|null{
        return localStorage.getItem('refreshToken');
    }

    private isTokenExpired(token:string):boolean{
        return this.jwtHelper.isTokenExpired(token);
    }

    decodeToken(token:string):any{
        return this.jwtHelper.decodeToken(token);
    }
}