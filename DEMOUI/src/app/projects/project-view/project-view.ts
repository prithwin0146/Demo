import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../project.service';
import { Project } from '../project.models';
import { EmployeeProjectService } from '../employee-project.service';
import { EmployeeProjectDto, AssignEmployeeDto } from '../employee-project.models';
import { EmployeeService } from '../../employees/employee.service';
import { Employee } from '../../employees/employee.models';
import { AuthService } from '../../login/auth.service';

@Component({
  selector: 'app-project-view',
  standalone: true,
  templateUrl: './project-view.html',
  styleUrls: ['./project-view.css'],
  imports: [CommonModule, FormsModule]
})
export class ProjectViewComponent implements OnInit {
  project: Project | null = null;
  assignedEmployees: EmployeeProjectDto[] = [];
  displayedEmployees: EmployeeProjectDto[] = [];
  loading = true;
  error: string | null = null;
  projectId!: number;
  
  showAssignForm = false;
  allEmployees: Employee[] = [];
  availableEmployees: Employee[] = [];
  selectedEmployeeId: number = 0;
  assignRole: string = '';
  canEdit: boolean = false;

  // Pagination for assigned employees
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  Math = Math;

  constructor(
    private projectService: ProjectService,
    private employeeProjectService: EmployeeProjectService,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) {
    this.canEdit = this.authService.isHROrAdmin();
  }

  ngOnInit(): void {
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
    this.employeeProjectService.getByProject(this.projectId).subscribe({
      next: (data) => {
        this.assignedEmployees = data;
        this.totalPages = Math.ceil(this.assignedEmployees.length / this.pageSize);
        this.updateDisplayedEmployees();
        this.updateAvailableEmployees();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading assigned employees:', err);
      }
    });
  }

  updateDisplayedEmployees(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.displayedEmployees = this.assignedEmployees.slice(startIndex, endIndex);
  }

  changePageSize(newSize: number): void {
    this.pageSize = newSize;
    this.currentPage = 1;
    this.totalPages = Math.ceil(this.assignedEmployees.length / this.pageSize);
    this.updateDisplayedEmployees();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updateDisplayedEmployees();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updateDisplayedEmployees();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updateDisplayedEmployees();
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
      alert('Please select an employee');
      return;
    }

    const assignment: AssignEmployeeDto = {
      employeeId: this.selectedEmployeeId,
      projectId: this.projectId,
      role: this.assignRole || null
    };

    this.employeeProjectService.assign(assignment).subscribe({
      next: () => {
        this.loadAssignedEmployees();
        this.selectedEmployeeId = 0;
        this.assignRole = '';
        this.showAssignForm = false;
      },
      error: (err) => {
        console.error('Error assigning employee:', err);
        alert('Failed to assign employee. Please try again.');
      }
    });
  }

  removeEmployee(employeeId: number): void {
    if (!confirm('Are you sure you want to remove this employee from the project?')) {
      return;
    }

    this.employeeProjectService.remove(employeeId, this.projectId).subscribe({
      next: () => {
        this.loadAssignedEmployees();
        // Reset to page 1 if current page becomes empty
        if (this.displayedEmployees.length === 1 && this.currentPage > 1) {
          this.currentPage--;
        }
      },
      error: (err) => {
        console.error('Error removing employee:', err);
        alert('Failed to remove employee. Please try again.');
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
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        console.error('Error deleting project:', err);
        alert('Failed to delete project. Please try again.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/projects']);
  }
}
