import { Component, OnInit, ChangeDetectorRef, ApplicationRef, Inject, PLATFORM_ID } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { ProjectService } from '../project.service';
import { NotificationService } from '../../shared/services/notification.service';
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
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatTableModule
  ]
})
export class EditProjectComponent implements OnInit {
  projectForm: FormGroup;
  projectId!: number;
  loading = true;
  saving = false;
  statuses = ['Active', 'Completed', 'On-Hold'];

  employees: Employee[] = [];
  assignedEmployees: EmployeeProjectDto[] = [];
  availableEmployees: Employee[] = [];
  selectedEmployeeId: number = 0;
  assignRole: string = '';
  canEdit: boolean = false;

  displayedColumns: string[] = ['employeeId', 'employeeName', 'role', 'assignedDate', 'action'];

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private employeeService: EmployeeService,
    private employeeProjectService: EmployeeProjectService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private appRef: ApplicationRef,
    private authService: AuthService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.canEdit = this.authService.isHROrAdmin();
    this.projectForm = this.fb.group({
      projectName: ['', [Validators.required, Validators.minLength(2)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      startDate: ['', Validators.required],
      endDate: [''],
      status: ['Active', Validators.required]
    });
  }

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    this.canEdit = this.authService.isHROrAdmin();
    if (!this.canEdit) {
      this.notificationService.showError('You do not have permission to edit projects');
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
      this.notificationService.showError('Invalid project ID');
      this.loading = false;
    }
  }

  loadProject(): void {
    this.loading = true;
    console.log('Loading project for edit, ID:', this.projectId);

    this.projectService.getProjectById(this.projectId).subscribe({
      next: (data) => {
        console.log('Project data loaded:', data);
        this.projectForm.patchValue({
          projectName: data.projectName,
          description: data.description,
          startDate: new Date(data.startDate),
          endDate: data.endDate ? new Date(data.endDate) : null,
          status: data.status
        });
        this.loading = false;
        console.log('Loading set to false, project ready for editing', this.loading);
        this.cdr.markForCheck();
        this.appRef.tick();
      },
      error: (err) => {
        console.error('Error loading project:', err);
        this.notificationService.showError('Failed to load project. Please try again.');
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const formValue = this.projectForm.value;

    const project: UpdateProject = {
      projectName: formValue.projectName,
      description: formValue.description,
      startDate: formValue.startDate ? new Date(formValue.startDate).toISOString().split('T')[0] : '',
      endDate: formValue.endDate ? new Date(formValue.endDate).toISOString().split('T')[0] : null,
      status: formValue.status
    };

    this.projectService.updateProject(this.projectId, project).subscribe({
      next: () => {
        this.notificationService.showSuccess('Project updated successfully!');
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        console.error('Error updating project:', err);
        this.notificationService.showError('Failed to update project. Please try again.');
        this.saving = false;
      }
    });
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
        this.notificationService.showError('Failed to load employees');
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
        this.notificationService.showError('Failed to load assigned employees');
      }
    });
  }

  updateAvailableEmployees(): void {
    const assignedIds = this.assignedEmployees.map(e => e.employeeId);
    this.availableEmployees = this.employees.filter(e => !assignedIds.includes(e.id));
  }

  onEmployeeSelect(): void {
    if (this.selectedEmployeeId > 0) {
      const selectedEmployee = this.employees.find(emp => emp.id === this.selectedEmployeeId);
      if (selectedEmployee) {
        this.assignRole = selectedEmployee.jobRole;
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
        this.loadAssignedEmployees();
        this.selectedEmployeeId = 0;
        this.assignRole = '';
      },
      error: (err) => {
        console.error('Error assigning employee:', err);
        this.notificationService.showError('Failed to assign employee');
      }
    });
  }

  removeEmployee(employeeId: number, employeeName: string): void {
    if (confirm(`Remove ${employeeName} from the project?`)) {
      this.employeeProjectService.remove(employeeId, this.projectId).subscribe({
        next: () => {
          this.notificationService.showSuccess('Employee removed successfully');
          this.loadAssignedEmployees();
        },
        error: (err) => {
          console.error('Error removing employee:', err);
          this.notificationService.showError('Failed to remove employee');
        }
      });
    }
  }

  deleteProject(): void {
    const projectName = this.projectForm.get('projectName')?.value || 'this project';
    if (confirm(`Are you sure you want to delete "${projectName}"? This action cannot be undone.`)) {
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
  }
}
