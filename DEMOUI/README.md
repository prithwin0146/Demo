# Employee Management System

A modern, full-featured **Employee Management System** built with **Angular 21**, **Angular Material**, and **Server-Side Rendering (SSR)**. The application provides complete CRUD interfaces for managing **Employees**, **Projects**, and **Departments**, with JWT-based authentication, role-based access control, server-side pagination, and a responsive Material Design UI.

---

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Development](#development)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Routing](#routing)
- [Authentication & Security](#authentication--security)
- [Data Models](#data-models)
- [API Integration](#api-integration)
- [Services](#services)
- [Components](#components)
- [Testing](#testing)
- [Build & Deployment](#build--deployment)
- [Code Style & Conventions](#code-style--conventions)
- [Known Issues & Notes](#known-issues--notes)
- [Contributing](#contributing)

---

## Features

### Core Functionality
- **Employee Management** — Create, view, edit, and delete employee records with job role and system role assignments
- **Project Management** — Full CRUD for projects; assign/remove employees to projects with role tracking
- **Department Management** — Full CRUD for departments; assign managers and view department members
- **User Registration** — Self-service user account creation

### Authentication & Authorization
- **JWT Authentication** — Token-based login with automatic `Authorization` header injection
- **Role-Based Access Control (RBAC)** — Four system roles: `Admin`, `Manager`, `HR`, `Employee`
- **Route Guards** — Functional `canActivateChild` guard protects all authenticated routes
- **401 Interception** — Automatic redirect to login on expired or invalid tokens

### Data & UX
- **Server-Side Pagination** — All list views use paginated API endpoints with configurable page size, sort, and search
- **Debounced Search** — 300ms debounce on search inputs to reduce API calls
- **Retry Logic** — Automatic retry (2 attempts, 1s delay) on network failures
- **In-Memory Caching** — Employee list cached for 5 minutes with manual invalidation on writes
- **Angular Material UI** — Tables, toolbars, cards, forms, chips, progress spinners, snackbar notifications
- **Loading States** — Spinners during async operations
- **SSR Support** — Server-side rendering with Express for improved SEO and initial load performance

---

## Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| **Angular** | 21.x | Frontend framework (standalone components) |
| **Angular Material** | 21.x | UI component library |
| **Angular CDK** | 21.x | Component Dev Kit (tables, overlays) |
| **TypeScript** | ~5.9.2 | Type-safe JavaScript |
| **RxJS** | ~7.8.0 | Reactive programming & async streams |
| **Vitest** | ^4.0.8 | Unit testing framework |
| **Express** | ^5.1.0 | SSR server |
| **Angular SSR** | ^21.0.3 | Server-side rendering integration |
| **esbuild** | (bundled) | Fast build tool via `@angular/build` |

---

## Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js** >= 20.x
- **npm** >= 11.7.0
- **Angular CLI** >= 21.0.3 (`npm install -g @angular/cli`)
- **Backend API** running at `http://localhost:5127` (see [Configuration](#configuration))

---

## Installation

```bash
# 1. Clone the repository
git clone <repository-url>
cd DEMOUI

# 2. Install dependencies
npm install

# 3. Verify Angular CLI
ng version
```

---

## Configuration

### API Base URL

All services connect to a backend REST API at **`http://localhost:5127/api/`**. The URLs are currently hardcoded in each service file:

| Service | Base URL |
|---|---|
| `AuthService` | `http://localhost:5127/api/Users` |
| `EmployeeService` | `http://localhost:5127/api/Employees` |
| `ProjectService` | `http://localhost:5127/api/Projects` |
| `EmployeeProjectService` | `http://localhost:5127/api/EmployeeProjects` |
| `DepartmentService` | `http://localhost:5127/api/Departments` |

> **Note:** There are no `environment.ts` files. To target a different API server, update the `apiUrl` constant in each service file.

### Environment Setup

| Environment | URL | Notes |
|---|---|---|
| Development | `http://localhost:4200` | `ng serve` with live reload |
| Production | Built to `dist/employee-ui/` | Optimized, no source maps |
| SSR | Configurable via `server.ts` | Express-based server rendering |

---

## Development

### Start Development Server

```bash
npm start
# or
ng serve
```

Navigate to `http://localhost:4200/`. The application auto-reloads on source file changes.

### Available Scripts

| Command | Description |
|---|---|
| `npm start` | Start dev server (`ng serve`) |
| `npm run build` | Production build |
| `npm run watch` | Build in watch mode (development) |
| `npm test` | Run unit tests with Vitest |
| `npm run serve:ssr:employee-ui` | Run SSR production server |

---

## Architecture

### Core Design Principles

| Principle | Implementation |
|---|---|
| **Standalone Components** | All components use `standalone: true` — no `NgModule` declarations |
| **Functional Guards** | Route protection via functional `CanActivateFn` |
| **Functional Interceptors** | HTTP interceptors via `HttpInterceptorFn` |
| **Bootstrap API** | `bootstrapApplication()` in `main.ts` (no `AppModule`) |
| **OnPush Change Detection** | Components use `ChangeDetectionStrategy.OnPush` for performance |
| **SSR-Safe Code** | `isPlatformBrowser()` checks before any `window`/`localStorage` access |
| **Reactive Forms** | Employee/Project/Department forms use `FormBuilder` with validators |
| **Template-Driven Forms** | Login and Register use `FormsModule` with `ngModel` |

### Application Bootstrap

```
main.ts
  └─ bootstrapApplication(App, appConfig)
       ├─ provideRouter(routes)
       ├─ provideHttpClient(withFetch(), withInterceptors([tokenInterceptor]))
       └─ provideAnimations()
```

For SSR, `app.config.server.ts` merges in `provideServerRendering(withRoutes(serverRoutes))` where all routes use `RenderMode.Server`.

---

## Project Structure

```
DEMOUI/
├── angular.json                    # Angular CLI & build configuration
├── package.json                    # Dependencies & scripts
├── tsconfig.json                   # Base TypeScript config
├── tsconfig.app.json               # App-specific TS config
├── tsconfig.spec.json              # Test-specific TS config
├── public/                         # Static assets
├── src/
│   ├── index.html                  # Root HTML template
│   ├── main.ts                     # Client-side bootstrap
│   ├── main.server.ts              # Server-side bootstrap
│   ├── server.ts                   # Express SSR server
│   ├── styles.css                  # Global styles
│   └── app/
│       ├── app.ts                  # Root component (RouterOutlet)
│       ├── app.html                # Root template
│       ├── app.css                 # Root styles
│       ├── app.routes.ts           # Route definitions
│       ├── app.routes.server.ts    # SSR route config
│       ├── app.config.ts           # Client providers
│       ├── app.config.server.ts    # Server providers
│       ├── auth-guard.ts           # Functional route guard
│       ├── token-interceptor.ts    # JWT HTTP interceptor
│       │
│       ├── login/                  # Login feature
│       │   ├── auth.service.ts     #   Authentication API service
│       │   ├── login.ts            #   Login component
│       │   ├── login.html          #   Login template
│       │   ├── login.css           #   Login styles
│       │   └── login.spec.ts       #   Login tests
│       │
│       ├── register/               # Registration feature
│       │   ├── register.ts         #   Register component
│       │   ├── register.html       #   Register template
│       │   └── register.css        #   Register styles
│       │
│       ├── employees/              # Employee feature
│       │   ├── employee.models.ts  #   Employee interfaces
│       │   ├── employee.service.ts #   Employee API service (cached)
│       │   ├── employee-list/      #   List view (paginated, searchable)
│       │   ├── add-employee/       #   Create form (reactive)
│       │   └── edit-employee/      #   Edit form (reactive)
│       │
│       ├── projects/               # Project feature
│       │   ├── project.models.ts           # Project interfaces
│       │   ├── project.service.ts          # Project API service
│       │   ├── employee-project.models.ts  # Assignment interfaces
│       │   ├── employee-project.service.ts # Assignment API service
│       │   ├── project-list/               # List view (paginated)
│       │   ├── project-view/               # Detail view + employee assignments
│       │   ├── add-project/                # Create form
│       │   └── edit-project/               # Edit form
│       │
│       ├── departments/            # Department feature
│       │   ├── department.models.ts    # Department interfaces
│       │   ├── department.service.ts   # Department API service
│       │   ├── department-list/        # List view (paginated)
│       │   ├── department-view/        # Detail view + employee list
│       │   ├── add-department/         # Create form
│       │   └── edit-department/        # Edit form
│       │
│       └── shared/                 # Shared utilities
│           ├── pagination.models.ts    # PagedResponse<T>, PaginationRequest
│           ├── layout/                 # Shell component (navbar + outlet)
│           ├── navbar/                 # Top navigation bar
│           └── services/
│               └── notification.service.ts  # MatSnackBar wrapper
```

---

## Routing

All authenticated routes are children of `LayoutComponent`, which renders the navbar and a `<router-outlet>`.

| Path | Component | Auth | Description |
|---|---|---|---|
| `/` | — | No | Redirects to `/login` |
| `/login` | `Login` | No | User login page |
| `/register` | `RegisterComponent` | Yes | User registration form |
| `/employees` | `EmployeeListComponent` | Yes | Employee list (paginated, searchable) |
| `/employees/add` | `AddEmployeeComponent` | Yes | Create new employee |
| `/employees/edit/:id` | `EditEmployeeComponent` | Yes | Edit existing employee |
| `/projects` | `ProjectListComponent` | Yes | Project list (paginated) |
| `/projects/add` | `AddProjectComponent` | Yes | Create new project |
| `/projects/edit/:id` | `EditProjectComponent` | Yes | Edit existing project |
| `/projects/:id` | `ProjectViewComponent` | Yes | Project detail + employee assignments |
| `/departments` | `DepartmentListComponent` | Yes | Department list (paginated) |
| `/departments/add` | `AddDepartmentComponent` | Yes | Create new department |
| `/departments/edit/:id` | `EditDepartmentComponent` | Yes | Edit existing department |
| `/departments/:id` | `DepartmentViewComponent` | Yes | Department detail + member list |
| `**` | — | No | Wildcard redirect to `/login` |

> The employees route uses `runGuardsAndResolvers: 'always'` to re-execute guards on every navigation, including same-URL refreshes.

---

## Authentication & Security

### Login Flow

```
┌────────────┐    POST /api/Users/login     ┌────────────┐
│   Login     │ ──────────────────────────── │  Backend   │
│  Component  │                              │   API      │
└──────┬──────┘    { token, role }           └────────────┘
       │
       ▼
┌──────────────┐
│ localStorage │  Stores: 'token', 'userRole'
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│ tokenInterceptor │  Attaches "Authorization: Bearer <token>" to all API requests
└──────────────────┘
```

### Auth Guard (`auth-guard.ts`)

- **Type:** Functional `CanActivateFn`
- **Applied via:** `canActivateChild` on the `LayoutComponent` route
- **Logic:** Checks `localStorage.token` existence. If absent, redirects to `/login` with `replaceUrl: true`
- **SSR Safety:** Returns `true` during server-side rendering (skips `localStorage` check)

### Token Interceptor (`token-interceptor.ts`)

- **Type:** Functional `HttpInterceptorFn`
- **Behavior:**
  1. Skips non-browser environments (SSR)
  2. Skips requests to `/Users/login` and `/Users/register`
  3. Clones all other requests with `Authorization: Bearer <token>` header
  4. Catches **401 errors** globally: clears `localStorage`, redirects to `/login`
  5. Re-throws all other HTTP errors for individual subscriber handling

### Role-Based Access Control

Four system roles control UI visibility and feature access:

| Role | Permissions |
|---|---|
| **Admin** | Full access — manage all employees, projects, departments |
| **Manager** | Same as Admin (treated equivalently in `isAdmin()`) |
| **HR** | Can manage employees and projects (included in `isHROrAdmin()`) |
| **Employee** | Read-only access to lists; limited edit capabilities |

Role checks are performed via `AuthService` methods:
- `isAdmin()` — returns `true` for `Admin` or `Manager`
- `isHROrAdmin()` — returns `true` for `Admin`, `Manager`, or `HR`
- `isEmployee()` — returns `true` for `Employee`

Components conditionally render edit/delete buttons and table columns based on these role checks.

---

## Data Models

### Employee

```typescript
interface Employee {
  id: number;
  name: string;
  email: string;
  jobRole: string;       // Business role: Developer, Designer, QA, etc.
  userId?: number;       // FK to Users table
  userRole?: string;     // System role: Admin | Manager | HR | Employee
}
```

### Project

```typescript
interface Project {
  projectId: number;
  projectName: string;
  description: string;
  startDate: string;         // ISO date string
  endDate: string | null;
  status: string;            // e.g., "Active", "Completed"
  assignedEmployees?: number;
  employeeNames?: string | null;
}

interface CreateProject {
  projectName: string;
  description: string;
  startDate: string;
  endDate: string | null;
  status: string;
}

interface UpdateProject {
  projectName: string;
  description: string;
  startDate: string;
  endDate: string | null;
  status: string;
}
```

### Employee-Project Assignment

```typescript
interface EmployeeProjectDto {
  employeeProjectId: number;
  employeeId: number;
  employeeName: string;
  projectId: number;
  projectName: string;
  assignedDate: string;
  role: string | null;     // Role within the project
}

interface AssignEmployeeDto {
  employeeId: number;
  projectId: number;
  role: string | null;
}
```

### Department

```typescript
interface Department {
  departmentId: number;
  departmentName: string;
  description: string | null;
  managerId: number | null;
  managerName: string | null;
  employeeCount: number;
}

interface CreateDepartment {
  departmentName: string;
  description: string | null;
  managerId: number | null;
}

interface UpdateDepartment {
  departmentName: string;
  description: string | null;
  managerId: number | null;
}
```

### Pagination (Shared)

```typescript
interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'ASC' | 'DESC';
  searchTerm?: string;
}

interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}
```

---

## API Integration

All API calls target **`http://localhost:5127`**. The backend is expected to be a REST API (e.g., ASP.NET Core).

### Authentication — `/api/Users`

| Method | Endpoint | Request Body | Response |
|---|---|---|---|
| POST | `/login` | `{ email, password }` | `{ token, role }` |
| POST | `/register` | `{ username, email, password }` | — |

### Employees — `/api/Employees`

| Method | Endpoint | Query Parameters | Response |
|---|---|---|---|
| GET | `/` | — | `Employee[]` |
| GET | `/{id}` | — | `Employee` |
| POST | `/` | — (body: Employee) | `Employee` |
| PUT | `/{id}` | — (body: Employee) | `Employee` |
| DELETE | `/{id}` | — | — |
| GET | `/paged` | `pageNumber`, `pageSize`, `sortBy`, `sortOrder`, `searchTerm`, `departmentId?`, `jobRole?`, `systemRole?` | `PagedResponse<Employee>` |

### Projects — `/api/Projects`

| Method | Endpoint | Query Parameters | Response |
|---|---|---|---|
| GET | `/` | — | `Project[]` |
| GET | `/{id}` | — | `Project` |
| POST | `/` | — (body: CreateProject) | `Project` |
| PUT | `/{id}` | — (body: UpdateProject) | `Project` |
| DELETE | `/{id}` | — | — |
| GET | `/paged` | `pageNumber`, `pageSize`, `sortBy`, `sortOrder`, `searchTerm`, `hasEmployeesOnly?` | `PagedResponse<Project>` |

### Employee-Project Assignments — `/api/EmployeeProjects`

| Method | Endpoint | Query Parameters | Response |
|---|---|---|---|
| GET | `/project/{projectId}` | — | `EmployeeProjectDto[]` |
| GET | `/project/{projectId}/paged` | `pageNumber`, `pageSize`, `sortBy`, `sortOrder`, `searchTerm` | `PagedResponse<EmployeeProjectDto>` |
| POST | `/` | — (body: AssignEmployeeDto) | `number` (new ID) |
| DELETE | `/{employeeId}/{projectId}` | — | — |

### Departments — `/api/Departments`

| Method | Endpoint | Query Parameters | Response |
|---|---|---|---|
| GET | `/` | — | `Department[]` |
| GET | `/{id}` | — | `Department` |
| POST | `/` | — (body: CreateDepartment) | `Department` |
| PUT | `/{id}` | — (body: UpdateDepartment) | `Department` |
| DELETE | `/{id}` | — | — |
| GET | `/paged` | `pageNumber`, `pageSize`, `sortBy`, `sortOrder`, `searchTerm` | `PagedResponse<Department>` |

---

## Services

### `AuthService` — `src/app/login/auth.service.ts`

Handles user authentication and role management.

| Method | Description |
|---|---|
| `login(email, password)` | POST to `/api/Users/login`; returns `{ token, role }` |
| `register(user)` | POST to `/api/Users/register` |
| `getToken()` | Reads JWT from `localStorage` (SSR-safe) |
| `getUserRole()` | Reads user role from `localStorage` |
| `isAdmin()` | `true` if role is `Admin` or `Manager` |
| `isHROrAdmin()` | `true` if role is `Admin`, `Manager`, or `HR` |
| `isEmployee()` | `true` if role is `Employee` |
| `logout()` | Clears `token` and `userRole` from `localStorage` |

### `EmployeeService` — `src/app/employees/employee.service.ts`

Full CRUD + paginated listing with **5-minute in-memory cache** for the list endpoint.

| Method | Description |
|---|---|
| `getEmployees()` | GET all employees (cached for 5 min) |
| `getEmployee(id)` | GET single employee by ID |
| `createEmployee(employee)` | POST new employee; invalidates cache |
| `updateEmployee(id, employee)` | PUT employee; invalidates cache |
| `deleteEmployee(id)` | DELETE employee; invalidates cache |
| `getEmployeesPaged(...)` | GET `/paged` with filters: `departmentId`, `jobRole`, `systemRole` |
| `invalidateCache()` | Manually clear the in-memory cache |

### `ProjectService` — `src/app/projects/project.service.ts`

Full CRUD + paginated listing.

| Method | Description |
|---|---|
| `getAllProjects()` | GET all projects |
| `getProjectById(id)` | GET single project |
| `createProject(project)` | POST new project |
| `updateProject(id, project)` | PUT project |
| `deleteProject(id)` | DELETE project |
| `getProjectsPaged(...)` | GET `/paged` with optional `hasEmployeesOnly` filter |

### `EmployeeProjectService` — `src/app/projects/employee-project.service.ts`

Manages employee-project assignments.

| Method | Description |
|---|---|
| `getByProject(projectId)` | GET all assignments for a project |
| `getEmployeeProjectsPaged(...)` | GET paginated assignments for a project |
| `assign(dto)` | POST new assignment; returns new ID |
| `remove(employeeId, projectId)` | DELETE assignment |

### `DepartmentService` — `src/app/departments/department.service.ts`

Full CRUD + paginated listing.

| Method | Description |
|---|---|
| `getAllDepartments()` | GET all departments |
| `getDepartmentById(id)` | GET single department |
| `createDepartment(department)` | POST new department |
| `updateDepartment(id, department)` | PUT department |
| `deleteDepartment(id)` | DELETE department |
| `getDepartmentsPaged(...)` | GET `/paged` with search |

### `NotificationService` — `src/app/shared/services/notification.service.ts`

Wraps Angular Material `MatSnackBar` for consistent toast notifications.

| Method | Duration | Style |
|---|---|---|
| `showSuccess(message)` | 3 seconds | Green |
| `showError(message)` | 5 seconds | Red |
| `showInfo(message)` | 3 seconds | Default |

---

## Components

### Layout & Navigation

| Component | Location | Description |
|---|---|---|
| **App** | `src/app/app.ts` | Root component; renders `<router-outlet>` |
| **LayoutComponent** | `src/app/shared/layout/` | Shell wrapper; renders navbar + `<router-outlet>` for authenticated routes |
| **NavbarComponent** | `src/app/shared/navbar/` | Material toolbar with navigation links (Employees, Projects, Departments), username display (decoded from JWT), and logout button |

### Employee Components

| Component | Route | Description |
|---|---|---|
| **EmployeeListComponent** | `/employees` | Server-paginated Material table with debounced search (300ms), sort, department/job-role/system-role filters, retry on failure (2 retries, 1s delay), role-based column visibility, and `destroy$` subject for cleanup |
| **AddEmployeeComponent** | `/employees/add` | Reactive form (`FormBuilder`) with validators; password confirmation cross-field validator; department dropdown; role-based system role field; handles HTTP 409 (conflict) errors |
| **EditEmployeeComponent** | `/employees/edit/:id` | Reactive form pre-populated from API; similar structure to Add |

### Project Components

| Component | Route | Description |
|---|---|---|
| **ProjectListComponent** | `/projects` | Paginated Material table with search and sort |
| **ProjectViewComponent** | `/projects/:id` | Project detail card + paginated assigned-employees table; assign/remove employees; auto-fills role from employee's `jobRole`; permission-guarded (redirects non-HR/Admin users) |
| **AddProjectComponent** | `/projects/add` | Form for creating a new project |
| **EditProjectComponent** | `/projects/edit/:id` | Form for editing an existing project |

### Department Components

| Component | Route | Description |
|---|---|---|
| **DepartmentListComponent** | `/departments` | Paginated Material table with search and sort |
| **DepartmentViewComponent** | `/departments/:id` | Department detail card + paginated employee list (filtered by `departmentId`); delete with confirmation dialog; permission-guarded |
| **AddDepartmentComponent** | `/departments/add` | Form for creating a new department |
| **EditDepartmentComponent** | `/departments/edit/:id` | Form for editing an existing department |

### Auth Components

| Component | Route | Description |
|---|---|---|
| **Login** | `/login` | Template-driven form; stores JWT token and role in `localStorage` on success; error display with dismiss |
| **RegisterComponent** | `/register` | Template-driven form with client-side validation (email regex, min 6 char password) |

---

## Testing

### Unit Testing with Vitest

```bash
npm test
```

- Tests use **Vitest** (not Karma/Jasmine)
- Builder: `@angular/build:unit-test`
- Spec files co-located with components: `*.spec.ts`
- Coverage includes components, services, guards, and interceptors

### Test Files

| Test File | Covers |
|---|---|
| `app.spec.ts` | Root App component |
| `auth-guard.spec.ts` | Route guard logic |
| `token-interceptor.spec.ts` | HTTP interceptor |
| `login/login.spec.ts` | Login component |
| `employees/employee-list/employee-list.spec.ts` | Employee list |
| `employees/add-employee/add-employee.spec.ts` | Add employee form |
| `employees/edit-employee/edit-employee.spec.ts` | Edit employee form |

---

## Build & Deployment

### Production Build

```bash
npm run build
```

Output directory: `dist/employee-ui/`

### Build Configuration (angular.json)

| Setting | Value |
|---|---|
| **Builder** | `@angular/build:application` (esbuild-based) |
| **SSR** | Enabled (`outputMode: "server"`) |
| **Initial bundle budget** | Warning: 500 kB, Error: 1 MB |
| **Component style budget** | Warning: 4 kB, Error: 8 kB |
| **Source maps** | Enabled in development, disabled in production |
| **Optimization** | Enabled for production builds |

### Run SSR Server

```bash
npm run serve:ssr:employee-ui
```

Serves the production build using the Express SSR server defined in `src/server.ts`.

### Deployment Checklist

- [ ] Update API base URLs in all service files (or introduce environment files)
- [ ] Run production build: `npm run build`
- [ ] Test SSR functionality: `npm run serve:ssr:employee-ui`
- [ ] Verify authentication flow (login, token attachment, 401 redirect)
- [ ] Verify route guards protect all authenticated pages
- [ ] Test all CRUD operations for employees, projects, and departments
- [ ] Validate role-based UI visibility across all four roles
- [ ] Check error handling and notification display

---

## Code Style & Conventions

### Prettier Configuration

```json
{
  "printWidth": 100,
  "singleQuote": true,
  "overrides": [{ "files": "*.html", "options": { "parser": "angular" } }]
}
```

### Coding Guidelines

1. **Components** — Use `standalone: true`, `ChangeDetectionStrategy.OnPush`, and explicit `imports` array
2. **Services** — Provided in `root` via `@Injectable({ providedIn: 'root' })`
3. **Subscriptions** — Use `takeUntil(destroy$)` with a `Subject` for cleanup in `ngOnDestroy`
4. **SSR Safety** — Always check `isPlatformBrowser(platformId)` before accessing `window`, `localStorage`, or `document`
5. **Forms** — Prefer `ReactiveFormsModule` with `FormBuilder` for complex forms; `FormsModule` acceptable for simple login/register
6. **Error Handling** — Subscribe with error callbacks; use `NotificationService` for user-facing messages
7. **Routing** — Functional guards, parent-level `canActivateChild`
8. **Change Detection** — Inject `ChangeDetectorRef` and call `markForCheck()` after async data loads in OnPush components

---

## Known Issues & Notes

| Issue | Description |
|---|---|
| **Hardcoded API URLs** | `http://localhost:5127` is hardcoded in every service. Consider introducing Angular `environment.ts` files for multi-environment support. |
| **Duplicate `PagedResponse`** | The `PagedResponse<T>` interface is defined in both `shared/pagination.models.ts` and `employees/employee.models.ts` with slightly different fields (`hasPreviousPage`/`hasNextPage` in the employee version). |
| **Redundant auth headers** | `DepartmentService` manually attaches `Authorization` headers via a private `getHeaders()` method, while the global `tokenInterceptor` already handles this for all other services. |
| **No token expiry check** | The auth guard only checks token existence, not JWT expiration. Expired tokens pass the guard and fail at the API level (caught by the 401 interceptor). |
| **`any` types** | `EmployeeService` uses `any` instead of the `Employee` interface for some methods. |

---

## Contributing

### Development Workflow

1. Create a feature branch from `main`
2. Make your changes
3. Ensure tests pass: `npm test`
4. Verify build succeeds: `npm run build`
5. Submit a pull request with a clear description

### Adding a New Feature Module

1. Create a folder under `src/app/<feature-name>/`
2. Define models in `<feature-name>.models.ts`
3. Create an API service in `<feature-name>.service.ts` (providedIn: `root`)
4. Create standalone components for list, add, edit, and detail views
5. Register routes in `app.routes.ts` under the `LayoutComponent` children
6. Add navigation link in `NavbarComponent`

---

## License

This project is private and proprietary.

---

**Built with Angular 21 + Angular Material + SSR**
