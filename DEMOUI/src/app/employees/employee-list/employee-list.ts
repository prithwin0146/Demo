import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { RouterLink, Router } from '@angular/router';
import { NgFor, CommonModule } from '@angular/common';
import { AuthService } from '../../login/auth.service';
import { Subject } from 'rxjs';
import { takeUntil, retryWhen, delay } from 'rxjs/operators';
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
  private retryCount = 0;
  private maxRetries = 2;

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
    this.loadEmployees();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEmployees() {
    console.log('loadEmployees called (retry: ' + this.retryCount + '/' + this.maxRetries + ')');
    this.loading = true;
    this.error = null;
    this.retryCount = 0;
    
    this.employeeService.getEmployees()
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
          console.log('Employees received successfully:', res?.length || 0, 'items');
          this.employees = Array.isArray(res) ? res : [];
          this.loading = false;
          this.error = null;
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error('Error loading employees after retries:', err);
          // Check if it's an auth error (401)
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
