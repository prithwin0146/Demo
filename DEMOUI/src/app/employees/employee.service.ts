import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { PagedResponse } from '../shared/pagination.models';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {

  apiUrl = 'http://localhost:5127/api/Employees';
  private employeesCache: any[] | null = null;
  private cacheTimestamp: number | null = null;
  private cacheDuration = 5 * 60 * 1000; // 5 minutes

  constructor(private http: HttpClient) {}

  getEmployees(): Observable<any[]> {
    // Check if cache is still valid
    if (this.employeesCache && this.cacheTimestamp && (Date.now() - this.cacheTimestamp) < this.cacheDuration) {
      console.log('Returning cached employees');
      return new Observable(observer => {
        observer.next(this.employeesCache!);
        observer.complete();
      });
    }

    // Fetch fresh data and cache it
    console.log('Fetching fresh employee data from API');
    this.cacheTimestamp = Date.now();
    return this.http.get<any[]>(this.apiUrl).pipe(
      tap((data) => {
        console.log('Employees cached');
        this.employeesCache = data;
      })
    );
  }

  invalidateCache() {
    console.log('Cache invalidated');
    this.employeesCache = null;
    this.cacheTimestamp = null;
  }

  createEmployee(employee: any) {
    return this.http.post(this.apiUrl, employee).pipe(
      tap(() => this.invalidateCache())
    );
  }

  getEmployee(id: number) {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  updateEmployee(id: number, employee: any) {
    return this.http.put(`${this.apiUrl}/${id}`, employee).pipe(
      tap(() => this.invalidateCache())
    );
  }

  deleteEmployee(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.invalidateCache())
    );
  }

  // Paginated employees using stored procedure
  getEmployeesPaged(
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy: string = 'Id',
    sortOrder: 'ASC' | 'DESC' = 'ASC',
    searchTerm: string = '',
    departmentId?: number,
    jobRole?: string,
    systemRole?: string,
    projectId?: number
  ): Observable<PagedResponse<any>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('sortBy', sortBy)
      .set('sortOrder', sortOrder);

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    if (departmentId !== undefined && departmentId !== null) {
      params = params.set('departmentId', departmentId.toString());
    }

    if (jobRole) {
      params = params.set('jobRole', jobRole);
    }

    if (systemRole) {
      params = params.set('systemRole', systemRole);
    }

    if (projectId !== undefined && projectId !== null) {
      params = params.set('projectId', projectId.toString());
    }

    const url = `${this.apiUrl}/paged?${params.toString()}`;
    console.log('📡 HTTP GET REQUEST STARTING:', url);
    return this.http.get<PagedResponse<any>>(`${this.apiUrl}/paged`, { params }).pipe(
      tap(response => console.log('✅ HTTP RESPONSE RECEIVED:', response))
    );
  }
}
