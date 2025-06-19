import { User } from "../models/User"

export interface UserState{
    users: User[];
    loading:boolean;
    error:string | null;
}

export const initialUserState: UserState={
    users:[new User(101,"Jane Doe","janedoe@gmail.com","User"),new User(102,"Tim Cook","tc@gmail.com","Admin")],
    loading:false,
    error:null
}