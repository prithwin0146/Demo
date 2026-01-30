import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-add-employee',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './add-employee.html',
  styleUrls: ['./add-employee.css']
})
export class AddEmployeeComponent implements OnInit {
  name = '';
  email = '';
  jobRole = '';
  systemRole = 'Employee';
  password = '';
  confirmPassword = '';
  error: string | null = null;
  
  currentUserRole = '';
  canEditSystemRole = false;
  
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
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.detectUserRole();
  }

  detectUserRole(): void {
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          this.currentUserRole = payload.role || payload.Role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
          this.canEditSystemRole = this.currentUserRole === 'Admin' || this.currentUserRole === 'HR';
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

  save(): void {
    if (!this.name || !this.email || !this.jobRole || !this.password || !this.confirmPassword) {
      this.error = 'Please fill in all fields';
      return;
    }

    if (this.password.length < 8) {
      this.error = 'Password must be at least 8 characters long';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    const employee = {
      name: this.name,
      email: this.email,
      jobRole: this.jobRole,
      systemRole: this.systemRole,
      password: this.password
    };

    this.error = null;
    this.empService.createEmployee(employee).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err) => {
        this.error = err.error?.message || 'Failed to add employee';
      }
    });
  }
}
