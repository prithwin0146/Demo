import { Component, OnInit, ChangeDetectorRef, PLATFORM_ID, Inject } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { DepartmentService } from '../../departments/department.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-edit-employee',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './edit-employee.html',
  styleUrls: ['./edit-employee.css']
})
export class EditEmployeeComponent implements OnInit {
  id!: number;
  name = '';
  email = '';
  jobRole = '';
  systemRole = 'Employee';
  departmentId: number | null = null;
  error: string | null = null;
  
  currentUserRole = '';
  isAdmin = false;
  departments: any[] = [];
  
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
  
  systemRoles = ['Admin', 'HR', 'Employee'];

  constructor(
    private empService: EmployeeService,
    private departmentService: DepartmentService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.detectUserRole();
    this.loadDepartments();
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.empService.getEmployee(this.id).subscribe({
      next: (emp) => {
        this.name = emp.name;
        this.email = emp.email;
        this.jobRole = emp.jobRole || '';
        this.systemRole = emp.systemRole || 'Employee';
        this.departmentId = emp.departmentId || null;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = err.error?.message || 'Employee not found';
        this.cdr.markForCheck();
      }
    });
  }

  loadDepartments(): void {
    this.departmentService.getAllDepartments().subscribe({
      next: (data) => {
        this.departments = data;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Error loading departments:', err);
      }
    });
  }

  detectUserRole(): void {
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          this.currentUserRole = payload.role || payload.Role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
          this.isAdmin = this.currentUserRole === 'Admin';
        } catch (e) {
          console.error('Error parsing token', e);
        }
      }
    }
  }

  dismissError(): void {
    this.error = null;
  }

  goBack(): void {
    this.router.navigate(['/employees']);
  }

  update(): void {
    if (!this.name || !this.email || !this.jobRole) {
      this.error = 'Please fill in all fields';
      return;
    }

    const employee = { 
      name: this.name, 
      email: this.email, 
      jobRole: this.jobRole,
      systemRole: this.systemRole,
      departmentId: this.departmentId
    };

    this.error = null;
    this.empService.updateEmployee(this.id, employee).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err) => {
        this.error = err.error?.message || 'Update failed';
      }
    });
  }
}
