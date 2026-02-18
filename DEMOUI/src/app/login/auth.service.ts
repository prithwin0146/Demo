import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5127/api/Users';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  login(email: string, password: string) {
    return this.http.post(`${this.apiUrl}/login`, { email, password });
  }

  register(user: { username: string; email: string; password: string }) {
    console.log('AuthService.register called with user:', { username: user.username, email: user.email });
    return this.http.post(`${this.apiUrl}/register`, {
      username: user.username,
      email: user.email,
      password: user.password
    });
  }

  getToken() {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('token');
    }
    return null;
  }

  getUsername(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('username');
    }
    return null;
  }

  getUserRole(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('userRole');
    }
    return null;
  }

  isHROrAdmin(): boolean {
    const role = this.getUserRole();
    return role === 'HR' || role === 'Admin' || role === 'Manager';
  }

  isAdmin(): boolean {
    const role = this.getUserRole();
    return role === 'Admin' || role === 'Manager';
  }

  isEmployee(): boolean {
    const role = this.getUserRole();
    return role === 'Employee';
  }

  logout() {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('token');
      localStorage.removeItem('userRole');
      localStorage.removeItem('username');
    }
  }
}

