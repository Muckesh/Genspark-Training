import { User } from './user.model';
import { Inquiry } from './inquiry.model';

export interface Buyer {
  id: string;
  phone: string;
  preferredLocation: string;
  budget: number;
  user?: User;
  inquiries?: Inquiry[];
}

export interface UpdateBuyerDto{
  phone: string;
  preferredLocation:string;
  budget:string;
}