export interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'ASC' | 'DESC';
  searchTerm?: string;
}

export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}
