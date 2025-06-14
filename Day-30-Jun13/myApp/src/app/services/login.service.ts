import { BehaviorSubject, Observable } from "rxjs";
import { LoginModel } from "../models/login";

export class LoginService {
    private userNameSubject = new BehaviorSubject<string|null>(null);
    username$:Observable<string|null> = this.userNameSubject.asObservable();

    validateUserLogin(login:LoginModel){
        if(login.username.length<3){
            this.userNameSubject.next(null);
            this.userNameSubject.error("Too short for username.");
        }
        else{
            this.userNameSubject.next(login.username);
        }
    }

    logout(){
        this.userNameSubject.next(null);
    }
}