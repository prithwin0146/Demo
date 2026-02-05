import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { DepartmentService } from '../department.service';
import { Department } from '../department.models';
import { AuthService } from '../../login/auth.service';

@Component({
  selector: 'app-department-list',
  standalone: true,
  templateUrl: './department-list.html',
  styleUrls: ['./department-list.css'],
  imports: [CommonModule, RouterLink]
})
export class DepartmentListComponent implements OnInit {
  departments: Department[] = [];
  loading = true;
  error: string | null = null;
  isAdmin = false;
  isHR = false;

  constructor(
    private departmentService: DepartmentService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private router: Router
  ) {
    const token = this.authService.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = (payload.role || payload.Role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '').toLowerCase();
        this.isAdmin = role === 'admin';
        this.isHR = role === 'hr';
      } catch (e) {
        console.error('Error parsing token', e);
      }
    }
  }

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments(): void {
    this.loading = true;
    this.error = null;

    this.departmentService.getAllDepartments().subscribe({
      next: (data) => {
        this.departments = data;
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

  deleteDepartment(department: Department): void {
    if (!confirm(`Are you sure you want to delete "${department.departmentName}"?`)) {
      return;
    }

    this.departmentService.deleteDepartment(department.departmentId).subscribe({
      next: () => {
        alert('Department deleted successfully');
        this.loadDepartments();
      },
      error: (err) => {
        console.error('Error deleting department:', err);
        const message = err.error?.message || 'Failed to delete department';
        alert(message);
      }
    });
  }

  dismissError(): void {
    this.error = null;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
