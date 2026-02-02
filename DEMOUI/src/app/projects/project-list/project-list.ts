import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../project.service';
import { Project } from '../project.models';
import { AuthService } from '../../login/auth.service';

@Component({
  selector: 'app-project-list',
  standalone: true,
  templateUrl: './project-list.html',
  styleUrls: ['./project-list.css'],
  imports: [CommonModule, RouterLink]
})
export class ProjectListComponent implements OnInit {
  projects: Project[] = [];
  loading = true;
  error: string | null = null;
  canEdit: boolean = false;

  constructor(
    private projectService: ProjectService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) {
    this.canEdit = this.authService.isHROrAdmin();
    console.log('ProjectListComponent - User role:', this.authService.getUserRole());
    console.log('ProjectListComponent - canEdit:', this.canEdit);
  }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.error = null;
    console.log('Loading projects...');

    this.projectService.getAllProjects().subscribe({
      next: (data) => {
        console.log('Projects loaded:', data);
        this.projects = data;
        this.loading = false;
        console.log('Loading set to false, projects count:', this.projects.length);
        this.cdr.detectChanges();
        console.log('Change detection triggered');
      },
      error: (err) => {
        console.error('Error loading projects:', err);
        this.error = 'Failed to load projects. Please try again later.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
