import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeProjectDto, AssignEmployeeDto } from './employee-project.models';
import { PagedResponse } from '../shared/pagination.models';

@Injectable({
  providedIn: 'root'
})
export class EmployeeProjectService {
  private apiUrl = 'http://localhost:5127/api/EmployeeProjects';

  constructor(private http: HttpClient) {}

  getByProject(projectId: number): Observable<EmployeeProjectDto[]> {
    return this.http.get<EmployeeProjectDto[]>(`${this.apiUrl}/project/${projectId}`);
  }

  getEmployeeProjectsPaged(
    projectId: number,
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy: string = 'AssignedDate',
    sortOrder: 'ASC' | 'DESC' = 'DESC',
    searchTerm: string = ''
  ): Observable<PagedResponse<EmployeeProjectDto>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('sortBy', sortBy)
      .set('sortOrder', sortOrder);

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<PagedResponse<EmployeeProjectDto>>(`${this.apiUrl}/project/${projectId}/paged`, { params });
  }

  assign(dto: AssignEmployeeDto): Observable<number> {
    return this.http.post<number>(this.apiUrl, dto);
  }

  remove(employeeId: number, projectId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${employeeId}/${projectId}`);
  }
}
