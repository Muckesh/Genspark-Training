export interface PropertyListingQueryParametersDto {
  pageNumber: number;
  pageSize?: number;
  keyword?: string;
  location?: string;
  minPrice?: number;
  maxPrice?: number;
  minBedrooms?: number;
  minBathrooms?: number;
  agentId?: string;
  sortBy?: string;          // 'price', 'bedrooms', 'bathrooms', 'createdAt'
  isDescending?: boolean;   // true for descending, false for ascending
  includeDeleted?: boolean; // Optional: to include deleted listings
}