/**
 * DEPARTMENT GRPC ERROR HANDLING GUIDE
 * ====================================
 * 
 * This document provides implementation examples for client-side error handling
 * when consuming the DepartmentGrpcService from the backend API.
 * 
 * Root Cause: Department with ID 6 does not exist in the database
 * Error Type: RpcException with StatusCode.NotFound
 * 
 * Solutions:
 * 1. Server-side: Added logging to track requests for non-existent departments ?
 * 2. Client-side: Implement error handling and cache invalidation
 * 
 * ====================================
 */

// ============================================
// EXAMPLE 1: REST API ERROR HANDLING
// (For Angular HttpClient calling REST endpoints)
// ============================================

/*
 * File: DEMOUI/src/app/departments/department.service.ts
 * 
 * Implement error handling in the DepartmentService:
 */

import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry, tap } from 'rxjs/operators';
import { Department, CreateDepartment, UpdateDepartment } from './department.models';
import { PagedResponse } from '../shared/pagination.models';

interface CacheEntry<T> {
  data: T;
  timestamp: number;
}

export class DepartmentServiceWithErrorHandling {
  private apiUrl = 'http://localhost:5127/api/Departments';
  private cache = new Map<string, CacheEntry<any>>();
  private cacheTTL = 5 * 60 * 1000; // 5 minutes

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  private getHeaders(): HttpHeaders {
    let token = '';
    if (isPlatformBrowser(this.platformId)) {
      token = localStorage.getItem('token') || '';
    }
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  // ============================================
  // ERROR HANDLING METHOD
  // ============================================
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 404:
          errorMessage = 'Department not found. It may have been deleted.';
          break;
        case 401:
          errorMessage = 'Unauthorized. Please log in again.';
          // Trigger logout/redirect
          break;
        case 403:
          errorMessage = 'You do not have permission to access this resource.';
          break;
        case 500:
        case 502:
        case 503:
        case 504:
          errorMessage = 'Server error. Please try again later.';
          break;
        default:
          errorMessage = error.error?.message || `HTTP Error: ${error.status}`;
      }
    }
    
    console.error('Department Service Error:', errorMessage, error);
    return throwError(() => new Error(errorMessage));
  }

  // ============================================
  // CACHING METHODS
  // ============================================
  private getCachedData<T>(key: string): T | null {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }

    const entry = this.cache.get(key);
    if (entry) {
      const now = Date.now();
      if (now - entry.timestamp < this.cacheTTL) {
        return entry.data;
      } else {
        this.cache.delete(key);
      }
    }
    return null;
  }

  private setCachedData<T>(key: string, data: T): void {
    if (isPlatformBrowser(this.platformId)) {
      this.cache.set(key, { data, timestamp: Date.now() });
    }
  }

  // ============================================
  // CACHE INVALIDATION METHODS
  // ============================================
  invalidateCache(): void {
    this.cache.clear();
  }

  invalidateDepartmentCache(encryptedId?: string): void {
    if (encryptedId) {
      this.cache.delete(`department_${encryptedId}`);
    } else {
      this.cache.clear();
    }
  }

  // ============================================
  // API METHODS WITH ERROR HANDLING
  // ============================================

  getDepartmentById(encryptedId: string): Observable<Department> {
    const cacheKey = `department_${encryptedId}`;
    const cached = this.getCachedData<Department>(cacheKey);
    
    if (cached) {
      return new Observable(observer => {
        observer.next(cached);
        observer.complete();
      });
    }

    return this.http.get<Department>(`${this.apiUrl}/${encryptedId}`, {
      headers: this.getHeaders(),
    }).pipe(
      // Retry once on transient failures (not on 404)
      retry({
        count: 1,
        delay: (error) => {
          if (error instanceof HttpErrorResponse && error.status === 404) {
            throw error; // Don't retry 404 errors
          }
          return throwError(() => error);
        }
      }),
      tap(data => this.setCachedData(cacheKey, data)),
      catchError(error => {
        // Clear cache on 404 to prevent serving stale data
        if (error instanceof HttpErrorResponse && error.status === 404) {
          this.invalidateDepartmentCache(encryptedId);
        }
        return this.handleError(error);
      })
    );
  }

  createDepartment(department: CreateDepartment): Observable<any> {
    return this.http.post(this.apiUrl, department, {
      headers: this.getHeaders(),
    }).pipe(
      tap(() => this.invalidateCache()), // Clear all caches after create
      catchError(error => this.handleError(error))
    );
  }

  updateDepartment(encryptedId: string, department: UpdateDepartment): Observable<any> {
    return this.http.put(`${this.apiUrl}/${encryptedId}`, department, {
      headers: this.getHeaders(),
    }).pipe(
      tap(() => {
        this.invalidateDepartmentCache(encryptedId);
        this.invalidateCache();
      }),
      catchError(error => this.handleError(error))
    );
  }

  deleteDepartment(encryptedId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${encryptedId}`, {
      headers: this.getHeaders(),
    }).pipe(
      tap(() => this.invalidateCache()),
      catchError(error => this.handleError(error))
    );
  }

  getDepartmentsPaged(
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy: string = 'DepartmentName',
    sortOrder: 'ASC' | 'DESC' = 'ASC',
    searchTerm: string = ''
  ): Observable<PagedResponse<Department>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('sortBy', sortBy)
      .set('sortOrder', sortOrder);

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<PagedResponse<Department>>(`${this.apiUrl}/paged`, {
      headers: this.getHeaders(),
      params,
    }).pipe(
      catchError(error => this.handleError(error))
    );
  }
}


// ============================================
// EXAMPLE 2: COMPONENT ERROR HANDLING
// (For department-list component)
// ============================================

/*
 * File: DEMOUI/src/app/departments/department-list/department-list.ts
 * 
 * Implement error handling in component:
 */

export class DepartmentListComponentWithErrorHandling {
  error: string | null = null;

  constructor(private departmentService: DepartmentServiceWithErrorHandling) {}

  loadDepartments(): void {
    this.error = null; // Clear previous errors

    this.departmentService.getDepartmentsPaged(this.pageNumber, this.pageSize, this.sortBy, this.sortOrder, this.searchTerm)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.departments = response.data;
          this.totalRecords = response.totalRecords;
        },
        error: (err) => {
          const errorMsg = err instanceof Error ? err.message : 'Failed to load departments';
          this.error = errorMsg;
          this.cdr.detectChanges();
        }
      });
  }

  viewDepartment(encryptedId: string): void {
    this.departmentService.getDepartmentById(encryptedId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (department) => {
          this.router.navigate(['/departments', encryptedId]);
        },
        error: (err) => {
          if (err instanceof Error && err.message.includes('not found')) {
            // Department was deleted - refresh list
            this.error = 'This department no longer exists. Refreshing the list...';
            this.departmentService.invalidateCache();
            setTimeout(() => this.loadDepartments(), 1500);
          } else {
            this.error = err instanceof Error ? err.message : 'Failed to load department';
          }
        }
      });
  }

  dismissError(): void {
    this.error = null;
  }
}


// ============================================
// EXAMPLE 3: GLOBAL HTTP ERROR INTERCEPTOR
// (For application-wide error handling)
// ============================================

/*
 * File: DEMOUI/src/app/http-error.interceptor.ts
 * 
 * Create a global error interceptor:
 */

import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(
    private notificationService: NotificationService, // Your custom notification service
    private authService: AuthService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An error occurred';

        if (error.error instanceof ErrorEvent) {
          errorMessage = `Error: ${error.error.message}`;
        } else {
          switch (error.status) {
            case 404:
              errorMessage = 'Resource not found. It may have been deleted.';
              break;
            case 401:
              errorMessage = 'Your session has expired. Please log in again.';
              this.authService.logout();
              break;
            case 403:
              errorMessage = 'You do not have permission to perform this action.';
              break;
            case 500:
            case 502:
            case 503:
              errorMessage = 'Server error. Please try again later.';
              break;
            default:
              errorMessage = `HTTP Error: ${error.status} - ${error.statusText}`;
          }
        }

        // Show notification to user
        this.notificationService.showError(errorMessage);

        return throwError(() => new Error(errorMessage));
      })
    );
  }
}

/*
 * Register interceptor in app.config.ts:
 */

export const appConfig: ApplicationConfig = {
  providers: [
    // ... other providers
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    }
  ]
};


// ============================================
// SUMMARY OF FIXES
// ============================================

/*
 * 1. SERVER-SIDE (? ALREADY IMPLEMENTED):
 *    - Added ILogger<DepartmentGrpcService> to DepartmentGrpcService
 *    - Logs warning when department not found: "Department with ID {Id} not found. Client: {Peer}"
 *    - Helps identify client IPs requesting invalid departments
 * 
 * 2. CLIENT-SIDE (EXAMPLES PROVIDED):
 *    - Implement error handling for HTTP 404 responses
 *    - Implement cache invalidation when NotFound errors occur
 *    - Show user-friendly error messages
 *    - Auto-refresh department list when department is deleted
 *    - Use retry logic with exponential backoff for transient errors
 *    - Implement global HTTP error interceptor
 * 
 * 3. ROOT CAUSE:
 *    - Client is requesting department ID 6 which doesn't exist in database
 *    - Database has departments: 2, 4, 5, 11, 13
 *    - Either:
 *      a) Department 6 was deleted (but client still has cached/stale reference)
 *      b) Client has hardcoded department ID 6
 *      c) Race condition between client fetch and deletion
 * 
 * 4. NEXT STEPS:
 *    a) Check server logs to see which client IP is requesting ID 6
 *    b) Review client code for hardcoded department IDs
 *    c) Clear browser cache/local storage
 *    d) Implement cache invalidation strategy
 */
