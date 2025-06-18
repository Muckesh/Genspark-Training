import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable()
export class UserService{

    private apiUrl = "https://dummyjson.com/users";

    constructor(private http:HttpClient){}

    addUser(user:any){
        return this.http.post(`${this.apiUrl}/add`,user);
    }

    getUsers(){
        return this.http.get<any>(this.apiUrl);
    }

}