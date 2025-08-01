export interface UserResponse 
{
    userId: number;
    username: string;
    role: string;
} 

export interface UserRequest 
{
    username: string;
    password: string;
    role: string;
} 