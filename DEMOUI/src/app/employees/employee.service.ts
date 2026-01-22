import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

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
}
