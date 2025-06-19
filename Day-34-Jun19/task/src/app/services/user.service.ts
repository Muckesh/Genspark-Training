import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { User } from "../models/user.model";

@Injectable()
export class UserService{

    private initialUsers: User[] = [
        new User('john_doe', 'john@example.com', 'Password@123', 'Admin'),
        new User('jane_smith', 'jane@example.com', 'Secure#456', 'User'),
        new User('guest_user', 'guest@example.com', 'Guest#789', 'Guest'),
    ];

    private usersSubject = new BehaviorSubject<User[]>(this.initialUsers);
    users$=this.usersSubject.asObservable();

    
    get users() : User[] {
        return this.usersSubject.getValue();
    }
    

    addUser(user:User){
        const updated = [...this.users,user];
        this.usersSubject.next(updated);
    }

    filterUsers(query:string):User[]{
        query=query.toLowerCase();
        return this.users.filter(u=>
            u.username.toLowerCase().includes(query) ||
            u.role.toLowerCase().includes(query)
        );
    }

}