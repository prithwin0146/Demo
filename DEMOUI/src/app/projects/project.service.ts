import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project, CreateProject, UpdateProject } from './project.models';

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
}
