import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department, CreateDepartment, UpdateDepartment } from './department.models';
import { PagedResponse } from '../shared/pagination.models';

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private apiUrl = 'http://localhost:5127/api/Departments';

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

  getAllDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(this.apiUrl, {
      headers: this.getHeaders(),
    });
  }

  getDepartmentById(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders(),
    });
  }

  createDepartment(department: CreateDepartment): Observable<any> {
    return this.http.post(this.apiUrl, department, {
      headers: this.getHeaders(),
    });
  }

  updateDepartment(id: number, department: UpdateDepartment): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, department, {
      headers: this.getHeaders(),
    });
  }

  deleteDepartment(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders(),
    });
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
    });
  }
}
