import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DepartmentService } from '../department.service';
import { EmployeeService } from '../../employees/employee.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UpdateDepartment } from '../department.models';

@Component({
  selector: 'app-edit-department',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './edit-department.html',
  styleUrls: ['./edit-department.css']
})
export class EditDepartmentComponent implements OnInit {
  departmentId: number = 0;
  departmentName = '';
  description = '';
  managerId: number | null = null;
  error: string | null = null;
  employees: any[] = [];
  loading = false;
  loadingData = true;

  constructor(
    private departmentService: DepartmentService,
    private employeeService: EmployeeService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.departmentId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDepartment();
    this.loadEmployees();
  }

  loadDepartment(): void {
    this.loadingData = true;
    this.error = null;
    this.departmentService.getDepartmentById(this.departmentId).subscribe({
      next: (data) => {
        console.log('Department data received:', data);
        this.departmentName = data.departmentName;
        this.description = data.description || '';
        this.managerId = data.managerId;
        this.loadingData = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading department:', err);
        this.error = 'Failed to load department';
        this.loadingData = false;
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

    const department: UpdateDepartment = {
      departmentName: this.departmentName,
      description: this.description || null,
      managerId: this.managerId
    };

    this.loading = true;
    this.departmentService.updateDepartment(this.departmentId, department).subscribe({
      next: () => {
        alert('Department updated successfully!');
        this.router.navigate(['/departments']);
      },
      error: (err) => {
        console.error('Error updating department:', err);
        this.error = err.error?.message || 'Failed to update department';
        this.loading = false;
      }
    });
  }
}
