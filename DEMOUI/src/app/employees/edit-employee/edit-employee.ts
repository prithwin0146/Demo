import { Component, OnInit, ChangeDetectorRef, PLATFORM_ID, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EmployeeService } from '../employee.service';
import { DepartmentService } from '../../departments/department.service';
import { NotificationService } from '../../shared/services/notification.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-edit-employee',
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
  templateUrl: './edit-employee.html',
  styleUrls: ['./edit-employee.css']
})
export class EditEmployeeComponent implements OnInit {
  id!: number;
  employeeForm: FormGroup;
  loading = false;
  saving = false;

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

  systemRoles = ['Admin', 'HR', 'Manager', 'Employee'];

  constructor(
    private fb: FormBuilder,
    private empService: EmployeeService,
    private departmentService: DepartmentService,
    private notificationService: NotificationService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.employeeForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      jobRole: ['', Validators.required],
      systemRole: ['Employee', Validators.required],
      departmentId: [null]
    });
  }

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    this.detectUserRole();
    this.loadDepartments();
    this.loadEmployee();
  }

  loadEmployee(): void {
    this.loading = true;
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.empService.getEmployee(this.id).subscribe({
      next: (emp) => {
        this.employeeForm.patchValue({
          name: emp.name,
          email: emp.email,
          jobRole: emp.jobRole || '',
          systemRole: emp.systemRole || 'Employee',
          departmentId: emp.departmentId || null
        });
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.notificationService.showError(err.error?.message || 'Employee not found');
        this.loading = false;
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
          this.isAdmin = this.currentUserRole === 'Admin' || this.currentUserRole === 'HR' || this.currentUserRole === 'Manager';

          if (!this.isAdmin) {
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
      departmentId: formValue.departmentId
    };

    this.empService.updateEmployee(this.id, employee).subscribe({
      next: () => {
        this.notificationService.showSuccess('Employee updated successfully!');
        this.router.navigate(['/employees']);
      },
      error: (err) => {
        const errorMessage = err.error?.message || 'Update failed';
        this.notificationService.showError(errorMessage);
        this.saving = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }
}
