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

  getProjectById(id: number): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/${id}`);
  }

  createProject(project: CreateProject): Observable<any> {
    return this.http.post(this.apiUrl, project);
  }

  updateProject(id: number, project: UpdateProject): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, project);
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getProjectsPaged(
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy: string = 'ProjectName',
    sortOrder: 'ASC' | 'DESC' = 'ASC',
    searchTerm: string = '',
    hasEmployeesOnly: boolean = false
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

    return this.http.get<PagedResponse<Project>>(`${this.apiUrl}/paged`, { params });
  }
}