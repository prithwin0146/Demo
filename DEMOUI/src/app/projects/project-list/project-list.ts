import { Component, OnInit, ChangeDetectorRef, ViewChild, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../project.service';
import { Project } from '../project.models';
import { AuthService } from '../../login/auth.service';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-project-list',
  standalone: true,
  templateUrl: './project-list.html',
  styleUrls: ['./project-list.css'],
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ]
})
export class ProjectListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  projects: Project[] = [];
  displayedColumns: string[] = ['projectName', 'status', 'startDate', 'endDate', 'actions'];
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
    private authService: AuthService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.canEdit = this.authService.isHROrAdmin();
    console.log('ProjectListComponent - User role:', this.authService.getUserRole());
    console.log('ProjectListComponent - canEdit:', this.canEdit);
  }

  ngOnInit(): void {
    // Only load data in the browser, not during SSR
    if (!isPlatformBrowser(this.platformId)) {
      this.loading = false;
      return;
    }

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

  // Material Paginator event handler
  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadProjects();
  }

  // Material Sort event handler
  onSortChange(sort: Sort): void {
    if (sort.direction) {
      this.sortBy = sort.active.charAt(0).toUpperCase() + sort.active.slice(1);
      this.sortOrder = sort.direction === 'asc' ? 'ASC' : 'DESC';
    } else {
      this.sortBy = 'ProjectName';
      this.sortOrder = 'ASC';
    }
    this.pageNumber = 1;
    this.loadProjects();
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.searchTerm = filterValue;
    this.pageNumber = 1;
    this.loadProjects();
  }
}
