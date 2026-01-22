import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5127/api/Users';

  constructor(private http: HttpClient) {}

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
    return localStorage.getItem('token');
  }

  logout() {
    localStorage.removeItem('token');
  }
}

