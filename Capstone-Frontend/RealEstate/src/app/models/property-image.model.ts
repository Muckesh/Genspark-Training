import { PropertyListing } from './property-listing.model';

export interface PropertyImage {
  id: string;
  fileName: string;
  fileUrl: string;
  isDeleted: boolean;
  deletedAt?: Date;
  propertyListingId: string;
  listing?: PropertyListing;
}