import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { DepartmentService } from '../department.service';
import { EmployeeService } from '../../employees/employee.service';
import { NotificationService } from '../../shared/services/notification.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UpdateDepartment } from '../department.models';

@Component({
  selector: 'app-edit-department',
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
  templateUrl: './edit-department.html',
  styleUrls: ['./edit-department.css']
})
export class EditDepartmentComponent implements OnInit {
  departmentId: number = 0;
  departmentForm: FormGroup;
  employees: any[] = [];
  loading = false;
  saving = false;

  constructor(
    private fb: FormBuilder,
    private departmentService: DepartmentService,
    private employeeService: EmployeeService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {
    this.departmentForm = this.fb.group({
      departmentName: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)],
      managerId: [null]
    });
  }

  ngOnInit(): void {
    this.departmentId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDepartment();
    this.loadEmployees();
  }

  loadDepartment(): void {
    this.loading = true;
    this.departmentService.getDepartmentById(this.departmentId).subscribe({
      next: (data) => {
        console.log('Department data received:', data);
        this.departmentForm.patchValue({
          departmentName: data.departmentName,
          description: data.description || '',
          managerId: data.managerId
        });
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading department:', err);
        this.notificationService.showError('Failed to load department');
        this.loading = false;
        this.cdr.detectChanges();
      },
      complete: () => {
        console.log('Department loading complete');
      }
    });
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

    const department: UpdateDepartment = {
      departmentName: formValue.departmentName,
      description: formValue.description || null,
      managerId: formValue.managerId
    };

    this.departmentService.updateDepartment(this.departmentId, department).subscribe({
      next: () => {
        this.notificationService.showSuccess('Department updated successfully!');
        this.router.navigate(['/departments']);
      },
      error: (err) => {
        console.error('Error updating department:', err);
        const errorMessage = err.error?.message || 'Failed to update department';
        this.notificationService.showError(errorMessage);
        this.saving = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/departments']);
  }
}
