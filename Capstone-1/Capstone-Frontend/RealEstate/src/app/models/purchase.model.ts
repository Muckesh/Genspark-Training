import { PropertyListing } from "./property-listing.model";
import { User } from "./user.model";

export interface Purchase1{
    id: string;
    buyerId: string;
    listingId: string;
    priceAtPurchase: number;
    purchasedAt: Date;
    status: string;
    buyer?:User;
    listing?:PropertyListing;
}

export interface Purchase {
  id: string;
  buyerId: string;
  listingId: string;
  priceAtPurchase: number;
  purchasedAt: Date;
  status: string;
  buyer?: {
    id: string;
    name: string;
    email: string;
  };
  listing?: {
    id: string;
    title: string;
    description: string;
    price: number;
    location: string;
    bedrooms: number;
    bathrooms: number;
    squareFeet: number;
    propertyType: string;
    listingType: string;
    status: string;
    agent?: {
      id: string;
      agencyName: string;
      licenseNumber: string;
      phone: string;
    };
  };
}

export interface PurchaseResponseDto{
    id: string;
    buyerId: string;
    listingId: string;
    priceAtPurchase: number;
    purchasedAt: Date;
    status: string;
    buyer?:User;
    listing?:PropertyListing;
}