import { HttpClient, HttpHeaders } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { UserLogin } from "../models/userLoginModel";
import { Router } from "@angular/router";

@Injectable()
export class UserService{
    private http = inject(HttpClient);
    // private router = inject(Router);
    private apiUrl = "https://dummyjson.com/auth";
    private usernameSubject = new BehaviorSubject<string|null>(null);
    username$:Observable<string|null>  = this.usernameSubject.asObservable();

    validateUserLogin(user:UserLogin)
    {
        if(user.username.length<3){
            this.usernameSubject.next(null);
        }
        else{
            this.callLoginApi(user).subscribe(
                {
                    next:(data:any)=>{
                        this.usernameSubject.next(user.username),
                        localStorage.setItem("token",data.accessToken);
                    }
                }
            );
            // this.router.navigate(["products"]);

        }
    }

    callGetProfile(){
        var token = localStorage.getItem("token");
        const httpHeader = new HttpHeaders({
            'Authorizatiom': `Bearer ${token}`
        });
        return this.http.get(`${this.apiUrl}/me`,{headers:httpHeader});
    }

    callLoginApi(user:UserLogin){
        return this.http.post(`${this.apiUrl}/login`,user);
    }

}