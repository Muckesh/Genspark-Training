import { User } from './user.model';
import { PropertyListing } from './property-listing.model';

export interface Agent {
  id: string;
  licenseNumber: string;
  agencyName: string;
  phone: string;
  user?: User;
  listings?: PropertyListing[];
}

export interface UpdateAgentDto{
  agencyName:string;
  phone:string;
  licenseNumber:string;
}