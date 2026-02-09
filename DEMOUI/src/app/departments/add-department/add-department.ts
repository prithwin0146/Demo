import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { DepartmentService } from '../department.service';
import { EmployeeService } from '../../employees/employee.service';
import { NotificationService } from '../../shared/services/notification.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CreateDepartment } from '../department.models';

@Component({
  selector: 'app-add-department',
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
  templateUrl: './add-department.html',
  styleUrls: ['./add-department.css']
})
export class AddDepartmentComponent implements OnInit {
  departmentForm: FormGroup;
  employees: any[] = [];
  saving = false;

  constructor(
    private fb: FormBuilder,
    private departmentService: DepartmentService,
    private employeeService: EmployeeService,
    private notificationService: NotificationService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.departmentForm = this.fb.group({
      departmentName: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)],
      managerId: [null]
    });
  }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: (data: any) => {
        this.employees = data;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading employees:', err);
        this.notificationService.showError('Failed to load employees');
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (this.departmentForm.invalid) {
      this.departmentForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const formValue = this.departmentForm.value;

    const department: CreateDepartment = {
      departmentName: formValue.departmentName,
      description: formValue.description || null,
      managerId: formValue.managerId
    };

    this.departmentService.createDepartment(department).subscribe({
      next: () => {
        this.notificationService.showSuccess('Department created successfully!');
        this.router.navigate(['/departments']);
      },
      error: (err) => {
        console.error('Error creating department:', err);
        const errorMessage = err.error?.message || 'Failed to create department';
        this.notificationService.showError(errorMessage);
        this.saving = false;
        this.cdr.detectChanges();
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/departments']);
  }
}
