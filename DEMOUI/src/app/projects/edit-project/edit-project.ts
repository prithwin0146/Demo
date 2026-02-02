import { Component, OnInit, ChangeDetectorRef, ApplicationRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { ProjectService } from '../project.service';
import { UpdateProject } from '../project.models';
import { EmployeeService } from '../../employees/employee.service';
import { Employee } from '../../employees/employee.models';
import { EmployeeProjectService } from '../employee-project.service';
import { EmployeeProjectDto, AssignEmployeeDto } from '../employee-project.models';
import { AuthService } from '../../login/auth.service';

@Component({
  selector: 'app-edit-project',
  standalone: true,
  templateUrl: './edit-project.html',
  styleUrls: ['./edit-project.css'],
  imports: [CommonModule, FormsModule, RouterLink]
})
export class EditProjectComponent implements OnInit {
  project: UpdateProject = {
    projectName: '',
    description: '',
    startDate: '',
    endDate: null,
    status: 'Pending'
  };

  projectId!: number;
  loading = true;
  saving = false;
  error: string | null = null;
  statuses = ['Pending', 'Active', 'Completed', 'On-Hold'];

  employees: Employee[] = [];
  assignedEmployees: EmployeeProjectDto[] = [];
  availableEmployees: Employee[] = [];
  selectedEmployeeId: number = 0;
  assignRole: string = '';
  canEdit: boolean = false;

  constructor(
    private projectService: ProjectService,
    private employeeService: EmployeeService,
    private employeeProjectService: EmployeeProjectService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private appRef: ApplicationRef,
    private authService: AuthService
  ) {
    this.canEdit = this.authService.isHROrAdmin();
  }

  ngOnInit(): void {
    // Check if user has permission to edit
    if (!this.canEdit) {
      alert('You do not have permission to edit projects');
      this.router.navigate(['/projects']);
      return;
    }
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.projectId = +id;
      this.loadProject();
      this.loadEmployees();
      this.loadAssignedEmployees();
    } else {
      this.error = 'Invalid project ID';
      this.loading = false;
    }
  }

  loadProject(): void {
    this.loading = true;
    this.error = null;
    console.log('Loading project for edit, ID:', this.projectId);

    this.projectService.getProjectById(this.projectId).subscribe({
      next: (data) => {
        console.log('Project data loaded:', data);
        this.project = {
          projectName: data.projectName,
          description: data.description,
          startDate: this.formatDateForInput(data.startDate),
          endDate: data.endDate ? this.formatDateForInput(data.endDate) : null,
          status: data.status
        };
        this.loading = false;
        console.log('Loading set to false, project ready for editing', this.loading);
        this.cdr.markForCheck();
        this.appRef.tick();
      },
      error: (err) => {
        console.error('Error loading project:', err);
        this.error = 'Failed to load project. Please try again.';
        this.loading = false;
      }
    });
  }

  formatDateForInput(date: string): string {
    // Convert date to YYYY-MM-DD format for input[type="date"]
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  onSubmit(): void {
    if (!this.validateForm()) {
      return;
    }

    this.saving = true;
    this.error = null;

    this.projectService.updateProject(this.projectId, this.project).subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        console.error('Error updating project:', err);
        this.error = 'Failed to update project. Please try again.';
        this.saving = false;
      }
    });
  }

  validateForm(): boolean {
    if (!this.project.projectName.trim()) {
      this.error = 'Project name is required';
      return false;
    }
    if (!this.project.description.trim()) {
      this.error = 'Description is required';
      return false;
    }
    if (!this.project.startDate) {
      this.error = 'Start date is required';
      return false;
    }
    if (!this.project.status) {
      this.error = 'Status is required';
      return false;
    }
    return true;
  }

  cancel(): void {
    this.router.navigate(['/projects']);
  }

  loadEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: (data) => {
        this.employees = data;
        this.updateAvailableEmployees();
      },
      error: (err) => {
        console.error('Error loading employees:', err);
      }
    });
  }

  loadAssignedEmployees(): void {
    this.employeeProjectService.getByProject(this.projectId).subscribe({
      next: (data) => {
        this.assignedEmployees = data;
        this.updateAvailableEmployees();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading assigned employees:', err);
      }
    });
  }

  updateAvailableEmployees(): void {
    const assignedIds = this.assignedEmployees.map(e => e.employeeId);
    this.availableEmployees = this.employees.filter(e => !assignedIds.includes(e.id));
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
      },
      error: (err) => {
        console.error('Error assigning employee:', err);
        alert('Failed to assign employee');
      }
    });
  }

  removeEmployee(employeeId: number): void {
    if (!confirm('Remove this employee from the project?')) {
      return;
    }

    this.employeeProjectService.remove(employeeId, this.projectId).subscribe({
      next: () => {
        this.loadAssignedEmployees();
      },
      error: (err) => {
        console.error('Error removing employee:', err);
        alert('Failed to remove employee');
      }
    });
  }
}
