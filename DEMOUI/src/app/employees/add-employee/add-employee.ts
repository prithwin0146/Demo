import { Component } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-employee',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './add-employee.html',
  styleUrls: ['./add-employee.css']
})
export class AddEmployeeComponent {
  name = '';
  email = '';
  role = '';
  error: string | null = null;

  constructor(private empService: EmployeeService, private router: Router) {}

  dismissError(): void {
    this.error = null;
  }

  goBack(): void {
    this.router.navigate(['/employees']);
  }

  save(): void {
    if (!this.name || !this.email || !this.role) {
      this.error = 'Please fill in all fields';
      return;
    }

    const employee = {
      name: this.name,
      email: this.email,
      role: this.role
    };

    this.error = null;
    this.empService.createEmployee(employee).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err) => {
        this.error = err.error?.message || 'Failed to add employee';
      }
    });
  }
}
