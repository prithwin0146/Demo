import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../project.service';
import { Project } from '../project.models';
import { AuthService } from '../../login/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-project-list',
  standalone: true,
  templateUrl: './project-list.html',
  styleUrls: ['./project-list.css'],
  imports: [CommonModule, RouterLink, FormsModule]
})
export class ProjectListComponent implements OnInit {
  projects: Project[] = [];
  loading = true;
  error: string | null = null;
  canEdit: boolean = false;

  // Pagination properties
  pageNumber = 1;
  pageSize = 10;
  totalRecords = 0;
  totalPages = 0;
  sortBy = 'ProjectName';
  sortOrder: 'ASC' | 'DESC' = 'ASC';
  searchTerm = '';

  Math = Math;

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

    this.projectService.getProjectsPaged(this.pageNumber, this.pageSize, this.sortBy, this.sortOrder, this.searchTerm).subscribe({
      next: (response) => {
        console.log('Projects loaded:', response);
        this.projects = response.data;
        this.totalRecords = response.totalRecords;
        this.totalPages = response.totalPages;
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

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.pageNumber = 1;
    this.loadProjects();
  }

  changePageSize(newSize: number): void {
    this.pageSize = newSize;
    this.pageNumber = 1;
    this.loadProjects();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.loadProjects();
    }
  }

  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.loadProjects();
    }
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadProjects();
    }
  }

  firstPage(): void {
    this.pageNumber = 1;
    this.loadProjects();
  }

  lastPage(): void {
    this.pageNumber = this.totalPages;
    this.loadProjects();
  }

  sort(column: string): void {
    if (this.sortBy === column) {
      this.sortOrder = this.sortOrder === 'ASC' ? 'DESC' : 'ASC';
    } else {
      this.sortBy = column;
      this.sortOrder = 'ASC';
    }
    this.loadProjects();
  }
}
