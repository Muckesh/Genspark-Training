import { Injectable } from "@angular/core";

@Injectable()
export class UserService{
    private dummyUsers = [
        {id:1,name:"Alice",email:"alice@gmail.com"},
        {id:2,name:"Bob",email:"bob@gmail.com"},
        {id:3,name:"Charlie",email:"charlie@gamil.com"}
    ];

    getUsersCallback(callback: (users: any[])=>void):void{
        setTimeout(()=>{
            callback(this.dummyUsers);
        },1000);
    }

    getUsersPromise(): Promise<any[]>{
        return new Promise(resolve => {
            setTimeout(()=>{
                resolve(this.dummyUsers);
            },2000);
        });
    }

    async getUsersAsync():Promise<any[]>{
        return new Promise(resolve=>{
            setTimeout(()=>{
                resolve(this.dummyUsers)
            },3000);
        });
    }
}