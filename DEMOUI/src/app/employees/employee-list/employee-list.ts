import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { RouterLink, Router } from '@angular/router';
import { NgFor, CommonModule } from '@angular/common';
import { AuthService } from '../../login/auth.service';
import { Subject } from 'rxjs';
import { takeUntil, retryWhen, delay, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { take } from 'rxjs';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  templateUrl: './employee-list.html',
  styleUrls: ['./employee-list.css'],
  imports: [NgFor, RouterLink, CommonModule]
})
export class EmployeeListComponent implements OnInit, OnDestroy {
  employees: any[] = [];
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
  ascending = true;
  searchTerm = '';

  // Role-based access
  currentUserRole: string = '';
  isAdmin = false;
  isHR = false;
  isEmployee = false;

  // For template usage
  Math = Math;

  constructor(
    private employeeService: EmployeeService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) {
    console.log('EmployeeListComponent initialized');
  }

  ngOnInit(): void {
    console.log('ngOnInit called - loading employees on page initialization/refresh');
    
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
        this.isEmployee = roleLower === 'employee';
        console.log('Parsed role:', this.currentUserRole);
        console.log('Role flags - Admin:', this.isAdmin, 'HR:', this.isHR, 'Employee:', this.isEmployee);
      } catch (e) {
        console.error('Error parsing token:', e);
      }
    }
    
    this.loadEmployees();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEmployees() {
    console.log('loadEmployees called - Page:', this.pageNumber, 'Search:', this.searchTerm);
    this.loading = true;
    this.error = null;
    this.retryCount = 0;
    
    this.employeeService.getEmployeesPaged(this.pageNumber, this.pageSize, this.sortBy, this.ascending, this.searchTerm)
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
      this.ascending = !this.ascending;
    } else {
      this.sortBy = column;
      this.ascending = true;
    }
    this.loadEmployees();
  }

  search(term: string) {
    this.searchSubject$.next(term);
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

  logout() {
    this.authService.logout();
    this.router.navigate(['/login'], { replaceUrl: true });
  }
}
