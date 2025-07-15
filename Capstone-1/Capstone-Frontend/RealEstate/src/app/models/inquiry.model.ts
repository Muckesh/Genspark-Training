import { PropertyListing } from './property-listing.model';
import { Buyer } from './buyer.model';

export interface Inquiry {
  id?: string;
  listingId: string;
  listing?: PropertyListing;
  buyerId: string;
  buyer?: Buyer;
  message: string;
  createdAt?: Date;
  status?:string;
  replies:InquiryReply[];
}

export interface InquiryResponseDto{
  id:string;
  listingId:string;
  listingTitle:string;
  buyerId:string;
  buyerName:string;
  agentId:string;
  agentName:string;
  status:string;
  message:string;
  createdAt:Date;
  replies:InquiryReply[];
}

export interface InquiryReply {
  id?: string;
  inquiryId: string;
  inquiry?: Inquiry;
  authorId: string;
  authorType?: string;
  message: string;
  createdAt?: Date;
  
}

export interface InquiryWithRepliesDto{
  id?:string;
  message?:string;
  createdAt:Date;
  status:string;
  listing?:PropertyListing;
  replies:InquiryReply[];
}