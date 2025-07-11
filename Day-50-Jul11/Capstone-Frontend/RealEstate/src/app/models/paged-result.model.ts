export interface PagedResult<T> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  items:T[];
}