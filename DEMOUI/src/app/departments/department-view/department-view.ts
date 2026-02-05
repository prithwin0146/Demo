import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DepartmentService } from '../department.service';
import { EmployeeService } from '../../employees/employee.service';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Department } from '../department.models';

@Component({
  selector: 'app-department-view',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './department-view.html',
  styleUrls: ['./department-view.css']
})
export class DepartmentViewComponent implements OnInit {
  department: Department | null = null;
  employees: any[] = [];
  loading = true;
  loadingEmployees = false;
  error: string | null = null;

  // Pagination for employees
  currentPage = 1;
  pageSize = 10;
  totalRecords = 0;
  totalPages = 0;
  sortBy = 'Name';
  sortOrder: 'ASC' | 'DESC' = 'ASC';
  searchTerm = '';

  // Make Math available in template
  Math = Math;

  constructor(
    private departmentService: DepartmentService,
    private employeeService: EmployeeService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDepartment(id);
  }

  loadDepartment(id: number): void {
    this.loading = true;
    this.error = null;
    this.departmentService.getDepartmentById(id).subscribe({
      next: (data) => {
        console.log('Department data received:', data);
        this.department = data;
        this.loading = false;
        this.loadEmployees(id);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading department:', err);
        this.error = 'Failed to load department details';
        this.loading = false;
        this.cdr.detectChanges();
      },
      complete: () => {
        console.log('Department loading complete');
      }
    });
  }

  loadEmployees(departmentId: number): void {
    this.loadingEmployees = true;
    this.employeeService.getEmployeesPaged(
      this.currentPage,
      this.pageSize,
      this.sortBy,
      this.sortOrder,
      this.searchTerm,
      departmentId
    ).subscribe({
      next: (response) => {
        this.employees = response.data;
        this.totalRecords = response.totalRecords;
        this.totalPages = response.totalPages;
        this.loadingEmployees = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading employees:', err);
        this.loadingEmployees = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    if (this.department) {
      this.loadEmployees(this.department.departmentId);
    }
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.currentPage = 1;
    if (this.department) {
      this.loadEmployees(this.department.departmentId);
    }
  }

  onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortOrder = this.sortOrder === 'ASC' ? 'DESC' : 'ASC';
    } else {
      this.sortBy = column;
      this.sortOrder = 'ASC';
    }
    if (this.department) {
      this.loadEmployees(this.department.departmentId);
    }
  }

  goBack(): void {
    this.router.navigate(['/departments']);
  }
}
