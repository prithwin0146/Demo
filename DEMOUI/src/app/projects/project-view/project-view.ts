import { Component, OnInit, ChangeDetectorRef, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ProjectService } from '../project.service';
import { Project } from '../project.models';
import { EmployeeProjectService } from '../employee-project.service';
import { EmployeeProjectDto, AssignEmployeeDto } from '../employee-project.models';
import { EmployeeService } from '../../employees/employee.service';
import { Employee } from '../../employees/employee.models';
import { AuthService } from '../../login/auth.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-project-view',
  standalone: true,
  templateUrl: './project-view.html',
  styleUrls: ['./project-view.css'],
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ]
})
export class ProjectViewComponent implements OnInit {
  project: Project | null = null;
  assignedEmployees: EmployeeProjectDto[] = [];
  loading = true;
  loadingEmployees = false;
  error: string | null = null;
  projectId!: number;

  showAssignForm = false;
  allEmployees: Employee[] = [];
  availableEmployees: Employee[] = [];
  selectedEmployeeId: number = 0;
  assignRole: string = '';
  canEdit: boolean = false;

  // Pagination for assigned employees (server-side)
  currentPage = 1;
  pageSize = 10;
  totalRecords = 0;
  totalPages = 0;
  sortBy = 'AssignedDate';
  sortOrder: 'ASC' | 'DESC' = 'DESC';
  searchTerm = '';
  Math = Math;

  displayedColumns: string[] = ['employeeId', 'employeeName', 'role', 'assignedDate', 'action'];

  constructor(
    private projectService: ProjectService,
    private employeeProjectService: EmployeeProjectService,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private notificationService: NotificationService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.canEdit = this.authService.isHROrAdmin();
  }

  ngOnInit(): void {
    // Only load data in the browser, not during SSR
    if (!isPlatformBrowser(this.platformId)) {
      this.loading = false;
      return;
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.projectId = +id;
      this.loadProject();
      this.loadAllEmployees();
    } else {
      this.error = 'Invalid project ID';
      this.loading = false;
    }
  }

  loadProject(): void {
    this.loading = true;
    this.error = null;
    console.log('Loading project with ID:', this.projectId);

    this.projectService.getProjectById(this.projectId).subscribe({
      next: (data) => {
        console.log('Project loaded:', data);
        this.project = data;
        this.loading = false;
        this.loadAssignedEmployees();
        console.log('Loading set to false, project:', this.project);
        this.cdr.detectChanges();
        console.log('Change detection triggered');
      },
      error: (err) => {
        console.error('Error loading project:', err);
        this.error = 'Failed to load project details. Please try again later.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadAssignedEmployees(): void {
    this.loadingEmployees = true;
    this.employeeProjectService.getEmployeeProjectsPaged(
      this.projectId,
      this.currentPage,
      this.pageSize,
      this.sortBy,
      this.sortOrder,
      this.searchTerm
    ).subscribe({
      next: (response) => {
        this.assignedEmployees = response.data;
        this.totalRecords = response.totalRecords;
        this.totalPages = response.totalPages;
        this.loadingEmployees = false;
        this.updateAvailableEmployees();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading assigned employees:', err);
        this.loadingEmployees = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadAssignedEmployees();
  }

  onPageEvent(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex + 1;
    this.loadAssignedEmployees();
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.currentPage = 1;
    this.loadAssignedEmployees();
  }

  onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortOrder = this.sortOrder === 'ASC' ? 'DESC' : 'ASC';
    } else {
      this.sortBy = column;
      this.sortOrder = 'ASC';
    }
    this.loadAssignedEmployees();
  }

  changePageSize(newSize: number): void {
    this.pageSize = newSize;
    this.currentPage = 1;
    this.loadAssignedEmployees();
  }

  goToPage(page: number): void {
    this.onPageChange(page);
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadAssignedEmployees();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadAssignedEmployees();
    }
  }

  loadAllEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: (data) => {
        console.log('All employees loaded:', data);
        this.allEmployees = data;
        this.updateAvailableEmployees();
      },
      error: (err) => {
        console.error('Error loading employees:', err);
      }
    });
  }

  updateAvailableEmployees(): void {
    const assignedIds = this.assignedEmployees.map(e => e.employeeId);
    this.availableEmployees = this.allEmployees.filter(e => !assignedIds.includes(e.id));
    console.log('Available employees:', this.availableEmployees);
  }

  toggleAssignForm(): void {
    this.showAssignForm = !this.showAssignForm;
    if (!this.showAssignForm) {
      this.selectedEmployeeId = 0;
      this.assignRole = '';
    }
  }

  onEmployeeSelect(): void {
    console.log('Employee selected, ID:', this.selectedEmployeeId);
    if (this.selectedEmployeeId > 0) {
      const selectedEmployee = this.allEmployees.find(emp => emp.id === this.selectedEmployeeId);
      console.log('Selected employee:', selectedEmployee);
      if (selectedEmployee) {
        this.assignRole = selectedEmployee.jobRole;
        console.log('Auto-filled role:', this.assignRole);
      }
    } else {
      this.assignRole = '';
    }
  }

  assignEmployee(): void {
    if (this.selectedEmployeeId <= 0) {
      this.notificationService.showError('Please select an employee');
      return;
    }

    const assignment: AssignEmployeeDto = {
      employeeId: this.selectedEmployeeId,
      projectId: this.projectId,
      role: this.assignRole || null
    };

    this.employeeProjectService.assign(assignment).subscribe({
      next: () => {
        this.notificationService.showSuccess('Employee assigned successfully');
        this.currentPage = 1;
        this.loadAssignedEmployees();
        this.selectedEmployeeId = 0;
        this.assignRole = '';
        this.showAssignForm = false;
      },
      error: (err) => {
        console.error('Error assigning employee:', err);
        this.notificationService.showError('Failed to assign employee. Please try again.');
      }
    });
  }

  removeEmployee(employeeId: number): void {
    if (!confirm('Are you sure you want to remove this employee from the project?')) {
      return;
    }

    this.employeeProjectService.remove(employeeId, this.projectId).subscribe({
      next: () => {
        this.notificationService.showSuccess('Employee removed successfully');
        if (this.assignedEmployees.length === 1 && this.currentPage > 1) {
          this.currentPage--;
        }
        this.loadAssignedEmployees();
      },
      error: (err) => {
        console.error('Error removing employee:', err);
        this.notificationService.showError('Failed to remove employee. Please try again.');
      }
    });
  }

  deleteProject(): void {
    if (!confirm(`Are you sure you want to delete the project "${this.project?.projectName}"? This action cannot be undone.`)) {
      return;
    }

    this.projectService.deleteProject(this.projectId).subscribe({
      next: () => {
        console.log('Project deleted successfully');
        this.notificationService.showSuccess('Project deleted successfully');
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        console.error('Error deleting project:', err);
        this.notificationService.showError('Failed to delete project. Please try again.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/projects']);
  }
}
