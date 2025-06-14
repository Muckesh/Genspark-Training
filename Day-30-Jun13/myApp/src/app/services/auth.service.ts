import { User } from "../models/user";

export class AuthService {
    private dummyUsers:User[]=[
        {username:"admin",password:"admin123"},
        {username:"bob",password:"bob123"}
    ];
    
    validateUser(user:User):boolean{
        return this.dummyUsers.some(u=>u.username==user.username&&u.password==user.password);
    }

    saveUser(user:User,useSession:boolean=false):void{
        const storage = useSession ? sessionStorage:localStorage;
        storage.setItem("loggedInUser",JSON.stringify(user));
    }

    getUser(useSession:boolean=false):User|null{
        const storage = useSession?sessionStorage:localStorage;
        const data = storage.getItem("loggedInUser");
        return data?JSON.parse(data):null;
    }

    clearUser(useSession:boolean=false):void{
        const storage = useSession?sessionStorage:localStorage;
        storage.removeItem("loggedInUser");
    }
}