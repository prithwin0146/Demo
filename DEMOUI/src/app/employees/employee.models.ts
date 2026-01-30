export interface Employee {
  id: number;
  name: string;
  email: string;
  jobRole: string;       // Business role: Developer, Manager, etc.
  userId?: number;       // FK to Users table
  userRole?: string;     // System role: Admin | HR | Employee
}

export interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  ascending?: boolean;
  searchTerm?: string;
}

export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
