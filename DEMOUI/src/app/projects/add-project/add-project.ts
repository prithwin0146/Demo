import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ProjectService } from '../project.service';
import { NotificationService } from '../../shared/services/notification.service';
import { CreateProject } from '../project.models';
import { AuthService } from '../../login/auth.service';

@Component({
  selector: 'app-add-project',
  standalone: true,
  templateUrl: './add-project.html',
  styleUrls: ['./add-project.css'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule
  ]
})
export class AddProjectComponent implements OnInit {
  projectForm: FormGroup;
  saving = false;
  statuses = ['Active', 'Completed', 'On-Hold'];
  canEdit: boolean = false;

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private notificationService: NotificationService,
    private router: Router,
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
      this.notificationService.showError('You do not have permission to create projects');
      this.router.navigate(['/projects']);
    }
  }

  onSubmit(): void {
    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const formValue = this.projectForm.value;

    const project: CreateProject = {
      projectName: formValue.projectName,
      description: formValue.description,
      startDate: formValue.startDate ? new Date(formValue.startDate).toISOString().split('T')[0] : '',
      endDate: formValue.endDate ? new Date(formValue.endDate).toISOString().split('T')[0] : null,
      status: formValue.status
    };

    this.projectService.createProject(project).subscribe({
      next: () => {
        this.notificationService.showSuccess('Project created successfully!');
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        console.error('Error creating project:', err);
        const errorMessage = err.error?.message || 'Failed to create project. Please try again.';
        this.notificationService.showError(errorMessage);
        this.saving = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/projects']);
  }
}
