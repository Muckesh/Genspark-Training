import { Agent } from "./agent.model";
import { Buyer } from "./buyer.model";

export interface User {
  id: string;
  name: string;
  email: string;
  passwordHash?: string;
  role: 'Admin' | 'Agent' | 'Buyer';
  createdAt?: Date;
  isDeleted: boolean;
  refreshToken?: string;
  refreshTokenExpiryTime?: Date;
  agentProfile?: Agent;
  buyerProfile?: Buyer;
}

export interface UserResponseDto {
  id: string;
  name: string;
  email: string;
  role: 'Admin' | 'Agent' | 'Buyer';
  createdAt: Date;
  isDeleted: boolean;
  refreshToken?: string;
  refreshTokenExpiryTime?: Date;
  agentProfile?: Agent;
  buyerProfile?: Buyer;
}

export interface StatCard {
  label: string;
  value: number;
  icon: string;
  color: string;
}

export interface UserQueryParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  name?:string;
  email?:string;
  role?:string;
}