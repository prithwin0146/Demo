# API Documentation

## Overview

This document describes the API endpoints used by the Employee Management System frontend. The backend API runs on `http://localhost:5127`.

---

## Base URL

```
http://localhost:5127/api
```

---

## Authentication

### Headers

All protected endpoints require a JWT token in the Authorization header:

```
Authorization: Bearer <token>
```

The token is automatically attached by the `tokenInterceptor` in the Angular application.

---

## Endpoints

### Authentication Endpoints

#### POST /login

Authenticate a user and receive a JWT token.

**Request**:
```http
POST /api/login
Content-Type: application/json

{
  "username": "string",
  "password": "string"
}
```

**Response** (Success - 200):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "john.doe"
  }
}
```

**Response** (Error - 401):
```json
{
  "error": "Invalid credentials"
}
```

---

#### POST /register

Register a new user account.

**Request**:
```http
POST /api/register
Content-Type: application/json

{
  "username": "string",
  "password": "string",
  "email": "string"
}
```

**Response** (Success - 201):
```json
{
  "message": "User registered successfully",
  "userId": 1
}
```

**Response** (Error - 400):
```json
{
  "error": "Username already exists"
}
```

---

### Employee Endpoints

#### GET /employees

Retrieve all employees.

**Request**:
```http
GET /api/employees
Authorization: Bearer <token>
```

**Response** (Success - 200):
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "position": "Software Engineer",
    "department": "Engineering",
    "salary": 75000,
    "hireDate": "2023-01-15"
  },
  {
    "id": 2,
    "name": "Jane Smith",
    "email": "jane.smith@example.com",
    "position": "Product Manager",
    "department": "Product",
    "salary": 85000,
    "hireDate": "2022-06-20"
  }
]
```

**Response** (Error - 401):
```json
{
  "error": "Unauthorized"
}
```

---

#### GET /employees/:id

Retrieve a specific employee by ID.

**Request**:
```http
GET /api/employees/1
Authorization: Bearer <token>
```

**Response** (Success - 200):
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "position": "Software Engineer",
  "department": "Engineering",
  "salary": 75000,
  "hireDate": "2023-01-15"
}
```

**Response** (Error - 404):
```json
{
  "error": "Employee not found"
}
```

---

#### POST /employees

Create a new employee.

**Request**:
```http
POST /api/employees
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Alice Johnson",
  "email": "alice.johnson@example.com",
  "position": "UI/UX Designer",
  "department": "Design",
  "salary": 70000,
  "hireDate": "2024-01-31"
}
```

**Response** (Success - 201):
```json
{
  "id": 3,
  "name": "Alice Johnson",
  "email": "alice.johnson@example.com",
  "position": "UI/UX Designer",
  "department": "Design",
  "salary": 70000,
  "hireDate": "2024-01-31"
}
```

**Response** (Error - 400):
```json
{
  "error": "Invalid employee data",
  "details": {
    "email": "Email already exists"
  }
}
```

---

#### PUT /employees/:id

Update an existing employee.

**Request**:
```http
PUT /api/employees/1
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "position": "Senior Software Engineer",
  "department": "Engineering",
  "salary": 85000,
  "hireDate": "2023-01-15"
}
```

**Response** (Success - 200):
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "position": "Senior Software Engineer",
  "department": "Engineering",
  "salary": 85000,
  "hireDate": "2023-01-15"
}
```

**Response** (Error - 404):
```json
{
  "error": "Employee not found"
}
```

---

#### DELETE /employees/:id

Delete an employee.

**Request**:
```http
DELETE /api/employees/1
Authorization: Bearer <token>
```

**Response** (Success - 204):
```
No Content
```

**Response** (Error - 404):
```json
{
  "error": "Employee not found"
}
```

---

## Data Models

### Employee Model

```typescript
interface Employee {
  id: number;
  name: string;
  email: string;
  position: string;
  department: string;
  salary: number;
  hireDate: string; // ISO 8601 date format
}
```

### User Model

```typescript
interface User {
  id: number;
  username: string;
  email: string;
}
```

### Login Request

```typescript
interface LoginRequest {
  username: string;
  password: string;
}
```

### Login Response

```typescript
interface LoginResponse {
  token: string;
  user: User;
}
```

### Register Request

```typescript
interface RegisterRequest {
  username: string;
  password: string;
  email: string;
}
```

---

## Error Handling

### HTTP Status Codes

| Status Code | Meaning | Description |
|-------------|---------|-------------|
| 200 | OK | Request successful |
| 201 | Created | Resource created successfully |
| 204 | No Content | Request successful, no content to return |
| 400 | Bad Request | Invalid request data |
| 401 | Unauthorized | Missing or invalid token |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error |

### Error Response Format

All error responses follow this format:

```json
{
  "error": "Error message",
  "details": {
    "field": "Field-specific error"
  }
}
```

---

## Rate Limiting

The API may implement rate limiting. Check response headers:

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 99
X-RateLimit-Reset: 1643673600
```

---

## CORS Configuration

The API should be configured to allow requests from:

```
http://localhost:4200
```

---

## Frontend Integration

### Service Implementation

The Angular application uses the following services:

#### AuthService

```typescript
export class AuthService {
  private apiUrl = 'http://localhost:5127/api';
  
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`, 
      credentials
    );
  }
  
  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }
  
  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
```

#### EmployeeService

```typescript
export class EmployeeService {
  private apiUrl = 'http://localhost:5127/api/employees';
  
  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiUrl);
  }
  
  getEmployee(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`);
  }
  
  createEmployee(employee: Employee): Observable<Employee> {
    return this.http.post<Employee>(this.apiUrl, employee);
  }
  
  updateEmployee(id: number, employee: Employee): Observable<Employee> {
    return this.http.put<Employee>(`${this.apiUrl}/${id}`, employee);
  }
  
  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

---

## Retry Logic

The frontend implements automatic retry logic for failed requests:

- **Retry Attempts**: 2
- **Retry Delay**: 1000ms (1 second)
- **Applicable To**: GET requests for employee list
- **Excluded**: Authentication endpoints

```typescript
this.employeeService.getEmployees()
  .pipe(
    retryWhen(errors => 
      errors.pipe(
        take(2),
        delay(1000),
        tap(() => console.log('Retrying request...'))
      )
    )
  )
  .subscribe({
    next: (data) => { /* Handle success */ },
    error: (err) => { /* Handle final error */ }
  });
```

---

## Testing with curl

### Login

```bash
curl -X POST http://localhost:5127/api/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"password123"}'
```

### Get Employees (with token)

```bash
curl -X GET http://localhost:5127/api/employees \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Create Employee

```bash
curl -X POST http://localhost:5127/api/employees \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Test Employee",
    "email":"test@example.com",
    "position":"Tester",
    "department":"QA",
    "salary":60000,
    "hireDate":"2024-01-31"
  }'
```

---

## WebSocket Support

Currently not implemented. Future versions may include:

- Real-time employee updates
- Live notifications
- Collaborative editing

---

**API Version**: 1.0  
**Last Updated**: January 31, 2026
