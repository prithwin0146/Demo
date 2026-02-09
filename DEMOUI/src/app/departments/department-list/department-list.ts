import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { DepartmentService } from '../department.service';
import { Department } from '../department.models';
import { AuthService } from '../../login/auth.service';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-department-list',
  standalone: true,
  templateUrl: './department-list.html',
  styleUrls: ['./department-list.css'],
  imports: [
    CommonModule,
    RouterLink,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ]
})
export class DepartmentListComponent implements OnInit, OnDestroy {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  departments: Department[] = [];
  displayedColumns: string[] = ['departmentName', 'description', 'managerName', 'employeeCount', 'actions'];
  loading = true;
  error: string | null = null;
  isAdmin = false;
  isHR = false;
  isManager = false;

  // Pagination properties
  pageNumber = 1;
  pageSize = 10;
  totalRecords = 0;
  totalPages = 0;
  sortBy = 'DepartmentName';
  sortOrder: 'ASC' | 'DESC' = 'ASC';
  searchTerm = '';

  private destroy$ = new Subject<void>();
  private searchSubject$ = new Subject<string>();
  Math = Math;

  constructor(
    private departmentService: DepartmentService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    const token = this.authService.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = (payload.role || payload.Role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '').toLowerCase();
        this.isAdmin = role === 'admin';
        this.isHR = role === 'hr';
        this.isManager = role === 'manager';
      } catch (e) {
        console.error('Error parsing token', e);
      }
    }
  }

  ngOnInit(): void {
    // Only load data in the browser, not during SSR
    if (!isPlatformBrowser(this.platformId)) {
      this.loading = false;
      return;
    }

    // Setup debounced search
    this.searchSubject$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(term => {
      this.searchTerm = term;
      this.pageNumber = 1;
      this.loadDepartments();
    });

    this.loadDepartments();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDepartments(): void {
    this.loading = true;
    this.error = null;

    this.departmentService.getDepartmentsPaged(this.pageNumber, this.pageSize, this.sortBy, this.sortOrder, this.searchTerm)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.departments = response.data;
          this.totalRecords = response.totalRecords;
          this.totalPages = response.totalPages;
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error loading departments:', err);
          this.error = 'Failed to load departments. Please try again later.';
          this.loading = false;
          this.cdr.detectChanges();
        }
      });
  }

  viewDepartment(id: number): void {
    this.router.navigate(['/departments', id]);
  }

  editDepartment(id: number): void {
    this.router.navigate(['/departments/edit', id]);
  }

  dismissError(): void {
    this.error = null;
  }

  // Material Paginator event handler
  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadDepartments();
  }

  // Computed properties for MatSort binding
  get matSortActive(): string {
    return this.sortBy.charAt(0).toLowerCase() + this.sortBy.slice(1);
  }

  get matSortDir(): 'asc' | 'desc' | '' {
    return this.sortOrder === 'DESC' ? 'desc' : 'asc';
  }

  // Material Sort event handler
  onSortChange(sort: Sort): void {
    if (sort.direction) {
      this.sortBy = sort.active.charAt(0).toUpperCase() + sort.active.slice(1);
      this.sortOrder = sort.direction === 'asc' ? 'ASC' : 'DESC';
    } else {
      this.sortBy = 'DepartmentName';
      this.sortOrder = 'ASC';
    }
    this.pageNumber = 1;
    this.loadDepartments();
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.searchSubject$.next(filterValue);
  }
}
