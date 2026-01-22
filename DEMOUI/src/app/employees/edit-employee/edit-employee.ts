import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-employee',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './edit-employee.html',
  styleUrls: ['./edit-employee.css']
})
export class EditEmployeeComponent implements OnInit {
  id!: number;
  name = '';
  email = '';
  role = '';
  error: string | null = null;

  constructor(
    private empService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.empService.getEmployee(this.id).subscribe({
      next: (emp) => {
        this.name = emp.name;
        this.email = emp.email;
        this.role = emp.role;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = err.error?.message || 'Employee not found';
        this.cdr.markForCheck();
      }
    });
  }

  dismissError(): void {
    this.error = null;
  }

  goBack(): void {
    this.router.navigate(['/employees']);
  }

  update(): void {
    if (!this.name || !this.email || !this.role) {
      this.error = 'Please fill in all fields';
      return;
    }

    const employee = { name: this.name, email: this.email, role: this.role };

    this.error = null;
    this.empService.updateEmployee(this.id, employee).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err) => {
        this.error = err.error?.message || 'Update failed';
      }
    });
  }
}
