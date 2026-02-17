import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeProjectDto, AssignEmployeeDto } from './employee-project.models';

@Injectable({
  providedIn: 'root'
})
export class EmployeeProjectService {
  private apiUrl = 'http://localhost:5127/api/EmployeeProjects';

  constructor(private http: HttpClient) {}

  getByProject(projectId: number): Observable<EmployeeProjectDto[]> {
    return this.http.get<EmployeeProjectDto[]>(`${this.apiUrl}/project/${projectId}`);
  }

  assign(dto: AssignEmployeeDto): Observable<number> {
    return this.http.post<number>(this.apiUrl, dto);
  }

  remove(employeeId: number, projectId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${employeeId}/${projectId}`);
  }
}
