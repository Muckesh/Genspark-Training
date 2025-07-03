import { Agent } from './agent.model';
import { PropertyImage } from './property-image.model';
import { Inquiry } from './inquiry.model';

export interface PropertyListing {
  id: string;
  title: string;
  description: string;
  price: number;
  location: string;
  bedrooms: number;
  bathrooms: number;
  squareFeet: number;
  propertyType:string;
  listingType:string;
  isPetsAllowed:boolean;
  status:string;
  hasParking:boolean;
  agentId: string;
  imageUrls: string[];
  agent?: Agent;
  createdAt: Date;
  isDeleted: boolean;
  images?: PropertyImage[];
  inquiries?: Inquiry[];
}

export interface PropertyListingResponseDto {
  id: string;
  title: string;
  description: string;
  price: number;
  location: string;
  bedrooms: number;
  bathrooms: number;
  squareFeet: number;
  propertyType:string;
  listingType:string;
  isPetsAllowed:boolean;
  status:string;
  hasParking:boolean;
  agentId: string;
  imageUrls: string[];
  name:string;
  email:string;
  licenseNumber:string;
  agencyName:string;
  phone:string;
  // agent?: Agent;
  createdAt: Date;
  isDeleted: boolean;
  // images?: PropertyImage[];
  // inquiries?: Inquiry[];
}

export interface CreateListingDto{
  title:string;
  description:string;
  price: number;
  location: string;
  bedrooms: number;
  bathrooms: number;
  squareFeet: number;
  agentId: string;
  propertyType:string;
  listingType:string;
  isPetsAllowed:boolean;
  status:string;
  hasParking:boolean;

}

export interface UpdateListingDto{
  title:string;
  description:string;
  price: number;
  location: string;
  bedrooms: number;
  bathrooms: number;
  squareFeet: number;
  propertyType:string;
  listingType:string;
  isPetsAllowed:boolean;
  status:string;
  hasParking:boolean;
}

export interface PropertyListingDto {
  id: string;
  title: string;
  description: string;
  price: number;
  location: string;
  bedrooms: number;
  bathrooms: number;
  squareFeet: number;
  agentId: string;
  createdAt: Date;
  isDeleted: boolean;
  imageUrls: string[];
  inquiries?: Inquiry[];
}