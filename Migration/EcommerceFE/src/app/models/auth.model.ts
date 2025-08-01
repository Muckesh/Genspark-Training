
export interface AuthResponse {
    token: string;
    refreshToken: string;
    username: string;
    role: string;
}

export interface AuthLoginRequest {
    username: string;
    password: string;
}

export interface AuthLogoutRequest {
    refreshToken: string;
}

export interface AuthRefreshTokenRequest {
    refreshToken: string;
}