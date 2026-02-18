import { Component, PLATFORM_ID, Inject } from '@angular/core';
import { AuthService } from '../login/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { isPlatformBrowser } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class Login {
  email = '';
  password = '';
  error: string | null = null;

  constructor(
    private auth: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  dismissError(): void {
    this.error = null;
  }

  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  login() {
    if (!this.email || !this.password) {
      this.error = 'Please enter both email and password';
      return;
    }

    if (!this.isValidEmail(this.email)) {
      this.error = 'Please enter a valid email address';
      return;
    }

    this.error = null;

    this.auth.login(this.email, this.password).subscribe({
      next: (res: any) => {
        console.log('Login successful');
        console.log('Login response:', res);
        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('token', res.token);
          const userRole = res.role || 'Employee';
          localStorage.setItem('userRole', userRole);
          localStorage.setItem('username', res.username || 'User');
          console.log('Stored userRole:', userRole);
        }
        this.router.navigate(['/employees']);
      },
      error: (err) => {
        console.error('Login error:', err);
        this.error = err.error?.message || err.message || 'Invalid credentials. Please try again.';
      }
    });
  }
}

