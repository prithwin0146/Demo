import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../login/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.html',
  styleUrls: ['./register.css'],
  imports: [FormsModule, CommonModule]
})
export class RegisterComponent implements OnInit {
  user = {
    username: '',
    email: '',
    password: ''
  };
  loading = false;
  error: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    console.log('RegisterComponent initialized');
  }

  ngOnInit(): void {
    console.log('RegisterComponent loaded');
  }

  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  dismissError(): void {
    this.error = null;
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  onSubmit(): void {
    
    if (!this.user.username || !this.user.email || !this.user.password) {
      console.warn('Form validation failed - missing required fields');
      return;
    }

    if (!this.isValidEmail(this.user.email)) {
      this.error = 'Please enter a valid email address';
      return;
    }

    if (this.user.password.length < 6) {
      this.error = 'Password must be at least 6 characters long';
      return;
    }

    this.loading = true;
    this.error = null;

    console.log('Submitting registration form:', { 
      username: this.user.username, 
      email: this.user.email 
    });

    this.authService.register(this.user).subscribe({
      next: (res) => {
        console.log('User created successfully:', res);
        this.loading = false;
        alert('User created successfully');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error('Registration error:', err);
        this.loading = false;
        this.error = err.error?.message || err.message || 'Failed to create user. Please try again.';
      }
    });
  }
}
