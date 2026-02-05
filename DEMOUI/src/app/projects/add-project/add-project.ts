import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ProjectService } from '../project.service';
import { CreateProject } from '../project.models';
import { AuthService } from '../../login/auth.service';

@Component({
  selector: 'app-add-project',
  standalone: true,
  templateUrl: './add-project.html',
  styleUrls: ['./add-project.css'],
  imports: [CommonModule, FormsModule, RouterLink]
})
export class AddProjectComponent implements OnInit {
  project: CreateProject = {
    projectName: '',
    description: '',
    startDate: '',
    endDate: null,
    status: 'Active'
  };

  loading = false;
  error: string | null = null;
  statuses = ['Active', 'Completed', 'On-Hold'];

  constructor(
    private projectService: ProjectService,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Check if user has permission to create projects
    if (!this.authService.isHROrAdmin()) {
      alert('You do not have permission to create projects');
      this.router.navigate(['/projects']);
    }
  }

  onSubmit(): void {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    this.error = null;

    this.projectService.createProject(this.project).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        console.error('Error creating project:', err);
        this.error = 'Failed to create project. Please try again.';
        this.loading = false;
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
}
