import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project, CreateProject, UpdateProject } from './project.models';
import { PagedResponse } from '../shared/pagination.models';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private apiUrl = 'http://localhost:5127/api/Projects';

  constructor(private http: HttpClient) {}

  getAllProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.apiUrl);
  }

  getProjectById(encryptedId: string): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/${encryptedId}`);
  }

  createProject(project: CreateProject): Observable<any> {
    return this.http.post(this.apiUrl, project);
  }

  updateProject(encryptedId: string, project: UpdateProject): Observable<any> {
    return this.http.put(`${this.apiUrl}/${encryptedId}`, project);
  }

  deleteProject(encryptedId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${encryptedId}`);
  }

  getProjectsPaged(
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy: string = 'ProjectName',
    sortOrder: 'ASC' | 'DESC' = 'ASC',
    searchTerm: string = '',
    hasEmployeesOnly: boolean = false,
    status: string = ''
  ): Observable<PagedResponse<Project>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('sortBy', sortBy)
      .set('sortOrder', sortOrder)
      .set('hasEmployeesOnly', hasEmployeesOnly.toString());

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    if (status) {
      params = params.set('status', status);
    }

    return this.http.get<PagedResponse<Project>>(`${this.apiUrl}/paged`, { params });
  }
}
