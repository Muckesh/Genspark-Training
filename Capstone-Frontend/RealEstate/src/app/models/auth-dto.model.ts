export interface LoginDto{
    email:string;
    password:string;
}

export interface RegisterBuyerDto{
    name:string;
    email:string;
    password:string;
    phone:string;
    preferredLocation?:string;
    budget?:number;
}

export interface RegisterAgentDto{
    name:string;
    email:string;
    password:string;
    licenseNumber:string;
    agencyName:string;
    phone:string;
}

export interface AuthResponseDto{
    email:string;
    role:string;
    token:string;
    refreshToken:string;
}

export interface RefreshTokenRequestDto{
    refreshToken:string;
}

export interface LogoutRequestDto{
    refreshToken:string;
}