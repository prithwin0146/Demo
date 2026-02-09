import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef, ViewChild, PLATFORM_ID, Inject } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { RouterLink, Router } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { AuthService } from '../../login/auth.service';
import { Subject } from 'rxjs';
import { takeUntil, retryWhen, delay, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { take } from 'rxjs';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  templateUrl: './employee-list.html',
  styleUrls: ['./employee-list.css'],
  imports: [
    RouterLink,
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule
  ]
})
export class EmployeeListComponent implements OnInit, OnDestroy {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  employees: any[] = [];
  displayedColumns: string[] = ['id', 'name', 'email', 'jobRole', 'systemRole', 'actions'];
  loading = true;
  error: string | null = null;
  private destroy$ = new Subject<void>();
  private searchSubject$ = new Subject<string>();
  private retryCount = 0;
  private maxRetries = 2;

  // Pagination properties
  pageNumber = 1;
  pageSize = 10;
  totalRecords = 0;
  totalPages = 0;
  sortBy = 'Id';
  sortOrder: 'ASC' | 'DESC' = 'ASC';
  searchTerm = '';

  // Filter properties
  selectedJobRole: string = '';
  selectedSystemRole: string = '';
  jobRoles = [
    'Software Engineer',
    'Senior Software Engineer',
    'Team Lead',
    'Project Manager',
    'Product Manager',
    'QA Engineer',
    'DevOps Engineer',
    'Business Analyst',
    'UI/UX Designer',
    'HR Manager',
    'Accountant',
    'Sales Executive'
  ];
  systemRoles = ['Admin', 'HR', 'Manager', 'Employee'];

  // Role-based access
  currentUserRole: string = '';
  isAdmin = false;
  isHR = false;
  isManager = false;
  isEmployee = false;

  // For template usage
  Math = Math;

  constructor(
    private employeeService: EmployeeService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    console.log('EmployeeListComponent initialized');
  }

  ngOnInit(): void {
    console.log('ngOnInit called - loading employees on page initialization/refresh');

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
      this.loadEmployees();
    });

    // Get user role from token
    const token = this.authService.getToken();
    console.log('Token:', token);
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        console.log('Token payload:', payload);
        this.currentUserRole = payload.role || payload.Role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
        const roleLower = this.currentUserRole.toLowerCase();
        this.isAdmin = roleLower === 'admin';
        this.isHR = roleLower === 'hr';
        this.isManager = roleLower === 'manager';
        this.isEmployee = roleLower === 'employee';
        console.log('Parsed role:', this.currentUserRole);
        console.log('Role flags - Admin:', this.isAdmin, 'HR:', this.isHR, 'Manager:', this.isManager, 'Employee:', this.isEmployee);
      } catch (e) {
        console.error('Error parsing token:', e);
      }
    }

    this.updateDisplayedColumns();
    this.loadEmployees();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEmployees() {
    console.log(' loadEmployees called - Page:', this.pageNumber, 'PageSize:', this.pageSize, 'Search:', this.searchTerm);
    console.log(' API URL will be: http://localhost:5127/api/Employees/paged?pageNumber=' + this.pageNumber + '&pageSize=' + this.pageSize);
    this.loading = true;
    this.error = null;
    this.retryCount = 0;

    this.employeeService.getEmployeesPaged(
      this.pageNumber, this.pageSize, this.sortBy, this.sortOrder, this.searchTerm,
      undefined, this.selectedJobRole || undefined, this.selectedSystemRole || undefined
    )
      .pipe(
        retryWhen(errors => 
          errors.pipe(
            take(this.maxRetries),
            delay(1000)
          )
        ),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: (res) => {
          console.log('Paged employees received:', res);
          this.employees = res.data || [];
          this.totalRecords = res.totalRecords || 0;
          this.totalPages = res.totalPages || 0;
          this.loading = false;
          this.error = null;
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error('Error loading employees after retries:', err);
          if (err.status === 401) {
            console.warn('Unauthorized - redirecting to login');
            this.router.navigate(['/login']);
            return;
          }
          this.error = err.message || 'Failed to load employees';
          this.employees = [];
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  refreshEmployees() {
    console.log('Manual refresh triggered - invalidating cache');
    this.employeeService.invalidateCache();
    this.pageNumber = 1;
    this.loadEmployees();
  }

  // Pagination methods
  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.loadEmployees();
    }
  }

  getVisiblePages(): number[] {
    const maxVisible = 5; // Show max 5 page numbers at a time
    const pages: number[] = [];
    
    if (this.totalPages <= maxVisible) {
      // Show all pages if total is less than max
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Smart pagination logic
      let start = Math.max(1, this.pageNumber - 2);
      let end = Math.min(this.totalPages, this.pageNumber + 2);
      
      // Adjust if near the beginning
      if (this.pageNumber <= 3) {
        start = 1;
        end = maxVisible;
      }
      
      // Adjust if near the end
      if (this.pageNumber >= this.totalPages - 2) {
        start = this.totalPages - maxVisible + 1;
        end = this.totalPages;
      }
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
    }
    
    return pages;
  }

  changePageSize(size: number) {
    this.pageSize = size;
    this.pageNumber = 1;
    this.loadEmployees();
  }

  sortColumn(column: string) {
    if (this.sortBy === column) {
      this.sortOrder = this.sortOrder === 'ASC' ? 'DESC' : 'ASC';
    } else {
      this.sortBy = column;
      this.sortOrder = 'ASC';
    }
    this.loadEmployees();
  }

  search(term: string) {
    this.searchSubject$.next(term);
  }

  onJobRoleFilterChange(value: string): void {
    this.selectedJobRole = value;
    this.pageNumber = 1;
    this.loadEmployees();
  }

  onSystemRoleFilterChange(value: string): void {
    this.selectedSystemRole = value;
    this.pageNumber = 1;
    this.loadEmployees();
  }

  clearFilters(): void {
    this.selectedJobRole = '';
    this.selectedSystemRole = '';
    this.pageNumber = 1;
    this.loadEmployees();
  }

  clearSearch(input: HTMLInputElement) {
    input.value = '';
    this.searchTerm = '';
    this.pageNumber = 1;
    this.loadEmployees();
  }

  dismissError() {
    this.error = null;
    this.cdr.markForCheck();
  }

  deleteEmployee(id: number) {
    if (confirm('Are you sure?')) {
      this.employeeService.deleteEmployee(id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            console.log('Employee deleted successfully');
            this.loadEmployees();
          },
          error: (err) => {
            console.error('Error deleting employee:', err);
            alert('Failed to delete employee');
          }
        });
    }
  }

  // Material Paginator event handler
  onPageChange(event: PageEvent): void {
    console.log(' PAGINATOR CLICKED! Event:', event);
    console.log(' New page index:', event.pageIndex, 'New page number:', event.pageIndex + 1);
    this.pageNumber = event.pageIndex + 1; // Material uses 0-based index
    this.pageSize = event.pageSize;
    console.log(' About to call loadEmployees with pageNumber:', this.pageNumber);
    this.loadEmployees();
  }

  // Computed properties for MatSort binding (camelCase column names)
  get matSortActive(): string {
    return this.sortBy.charAt(0).toLowerCase() + this.sortBy.slice(1);
  }

  get matSortDir(): 'asc' | 'desc' | '' {
    return this.sortOrder === 'DESC' ? 'desc' : 'asc';
  }

  // Material Sort event handler
  onSortChange(sort: Sort): void {
    if (sort.direction) {
      this.sortBy = sort.active.charAt(0).toUpperCase() + sort.active.slice(1); // Capitalize first letter
      this.sortOrder = sort.direction === 'asc' ? 'ASC' : 'DESC';
    } else {
      this.sortBy = 'Id';
      this.sortOrder = 'ASC';
    }
    this.pageNumber = 1;
    this.loadEmployees();
  }

  // Search filter for Material table
  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.search(filterValue);
  }

  // Determine columns to display based on role
  updateDisplayedColumns(): void {
    if (this.isAdmin) {
      this.displayedColumns = ['id', 'name', 'email', 'jobRole', 'systemRole', 'actions'];
    } else if (this.isHR || this.isManager) {
      this.displayedColumns = ['id', 'name', 'email', 'jobRole', 'actions'];
    } else {
      // Employee role: no actions column
      this.displayedColumns = ['id', 'name', 'email', 'jobRole'];
    }
  }
}
