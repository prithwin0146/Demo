import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DepartmentService } from '../department.service';
import { EmployeeService } from '../../employees/employee.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CreateDepartment } from '../department.models';

@Component({
  selector: 'app-add-department',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './add-department.html',
  styleUrls: ['./add-department.css']
})
export class AddDepartmentComponent implements OnInit {
  departmentName = '';
  description = '';
  managerId: number | null = null;
  error: string | null = null;
  employees: any[] = [];
  loading = false;

  constructor(
    private departmentService: DepartmentService,
    private employeeService: EmployeeService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

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
        this.cdr.detectChanges();
      }
    });
  }

  dismissError(): void {
    this.error = null;
    this.cdr.detectChanges();
  }

  goBack(): void {
    this.router.navigate(['/departments']);
  }

  save(): void {
    if (!this.departmentName) {
      this.error = 'Department name is required';
      return;
    }

    const department: CreateDepartment = {
      departmentName: this.departmentName,
      description: this.description || null,
      managerId: this.managerId
    };

    this.loading = true;
    this.departmentService.createDepartment(department).subscribe({
      next: () => {
        alert('Department created successfully!');
        this.router.navigate(['/departments']);
      },
      error: (err) => {
        console.error('Error creating department:', err);
        this.error = err.error?.message || 'Failed to create department';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
