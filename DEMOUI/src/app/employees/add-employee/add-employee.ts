import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { EmployeeService } from '../employee.service';
import { DepartmentService } from '../../departments/department.service';
import { NotificationService } from '../../shared/services/notification.service';
import { Router } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-add-employee',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './add-employee.html',
  styleUrls: ['./add-employee.css']
})
export class AddEmployeeComponent implements OnInit {
  employeeForm: FormGroup;
  saving = false;
  hidePassword = true;
  hideConfirmPassword = true;

  currentUserRole = '';
  canEditSystemRole = false;
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

  systemRoles = ['Admin', 'HR', 'Manager', 'Employee'];

  constructor(
    private fb: FormBuilder,
    private empService: EmployeeService,
    private departmentService: DepartmentService,
    private notificationService: NotificationService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.employeeForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email, Validators.pattern(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)]],
      jobRole: ['', Validators.required],
      systemRole: ['Employee', Validators.required],
      departmentId: [null],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(8)]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    this.detectUserRole();
    this.loadDepartments();
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  loadDepartments(): void {
    this.departmentService.getAllDepartments().subscribe({
      next: (data) => {
        this.departments = data;
      },
      error: (err) => {
        console.error('Error loading departments:', err);
        this.notificationService.showError('Failed to load departments');
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
          this.canEditSystemRole = this.currentUserRole === 'Admin' || this.currentUserRole === 'HR' || this.currentUserRole === 'Manager';

          if (!this.canEditSystemRole) {
            this.employeeForm.get('systemRole')?.disable();
          }
        } catch (e) {
          console.error('Error parsing token', e);
        }
      }
    }
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const formValue = this.employeeForm.getRawValue();

    const employee = {
      name: formValue.name,
      email: formValue.email,
      jobRole: formValue.jobRole,
      systemRole: formValue.systemRole,
      password: formValue.password,
      departmentId: formValue.departmentId
    };

    console.log('📤 Sending employee data:', employee);
    this.empService.createEmployee(employee).subscribe({
      next: () => {
        this.notificationService.showSuccess('Employee added successfully!');
        this.router.navigate(['/employees']);
      },
      error: (err) => {
        console.error('❌ Full error object:', err);
        console.error('❌ Error status:', err.status);
        console.error('❌ Error response:', err.error);

        let errorMessage = 'Failed to add employee. Please try again.';

        if (err.status === 409) {
          errorMessage = 'Email already exists. Please use a different email address.';
        } else if (err.error?.message) {
          errorMessage = err.error.message;
        } else if (typeof err.error === 'string') {
          errorMessage = err.error;
        }

        this.notificationService.showError(errorMessage);
        this.saving = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }
}
