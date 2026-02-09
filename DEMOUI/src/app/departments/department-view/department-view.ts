import { Component, OnInit, ChangeDetectorRef, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DepartmentService } from '../department.service';
import { Department } from '../department.models';
import { EmployeeService } from '../../employees/employee.service';
import { AuthService } from '../../login/auth.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-department-view',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './department-view.html',
  styleUrls: ['./department-view.css']
})
export class DepartmentViewComponent implements OnInit {
  department: Department | null = null;
  employees: any[] = [];
  loading = true;
  loadingEmployees = false;
  error: string | null = null;
  canEdit: boolean = false;

  // Pagination for employees
  currentPage = 1;
  pageSize = 10;
  totalRecords = 0;
  totalPages = 0;
  sortBy = 'Name';
  sortOrder: 'ASC' | 'DESC' = 'ASC';
  searchTerm = '';

  // Make Math available in template
  Math = Math;

  displayedColumns: string[] = ['employeeId', 'name', 'email', 'jobRole'];

  constructor(
    private departmentService: DepartmentService,
    private employeeService: EmployeeService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private notificationService: NotificationService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.canEdit = this.authService.isHROrAdmin();
  }

  ngOnInit(): void {
    // Only load data in the browser, not during SSR
    if (!isPlatformBrowser(this.platformId)) {
      this.loading = false;
      return;
    }

    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDepartment(id);
  }

  loadDepartment(id: number): void {
    this.loading = true;
    this.error = null;
    this.departmentService.getDepartmentById(id).subscribe({
      next: (data) => {
        console.log('Department data received:', data);
        this.department = data;
        this.loading = false;
        this.loadEmployees(id);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading department:', err);
        this.error = 'Failed to load department details';
        this.loading = false;
        this.cdr.detectChanges();
      },
      complete: () => {
        console.log('Department loading complete');
      }
    });
  }

  loadEmployees(departmentId: number): void {
    this.loadingEmployees = true;
    this.employeeService.getEmployeesPaged(
      this.currentPage,
      this.pageSize,
      this.sortBy,
      this.sortOrder,
      this.searchTerm,
      departmentId
    ).subscribe({
      next: (response) => {
        this.employees = response.data;
        this.totalRecords = response.totalRecords;
        this.totalPages = response.totalPages;
        this.loadingEmployees = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading employees:', err);
        this.loadingEmployees = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    if (this.department) {
      this.loadEmployees(this.department.departmentId);
    }
  }

  onPageEvent(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex + 1;
    if (this.department) {
      this.loadEmployees(this.department.departmentId);
    }
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.currentPage = 1;
    if (this.department) {
      this.loadEmployees(this.department.departmentId);
    }
  }

  onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortOrder = this.sortOrder === 'ASC' ? 'DESC' : 'ASC';
    } else {
      this.sortBy = column;
      this.sortOrder = 'ASC';
    }
    if (this.department) {
      this.loadEmployees(this.department.departmentId);
    }
  }

  deleteDepartment(): void {
    if (!this.department) return;

    if (!confirm(`Are you sure you want to delete "${this.department.departmentName}"? This action cannot be undone.`)) {
      return;
    }

    this.departmentService.deleteDepartment(this.department.departmentId).subscribe({
      next: () => {
        this.notificationService.showSuccess('Department deleted successfully');
        this.router.navigate(['/departments']);
      },
      error: (err) => {
        console.error('Error deleting department:', err);
        const message = err.error?.message || 'Failed to delete department. Please try again.';
        this.notificationService.showError(message);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/departments']);
  }
}
