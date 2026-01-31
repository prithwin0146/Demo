# Architecture Documentation

## Overview

The Employee Management System is built using Angular 21 with a modern, scalable architecture featuring standalone components, functional programming patterns, and server-side rendering capabilities.

---

## Core Architecture Principles

### 1. Standalone Components

All components use the standalone API (`standalone: true`), eliminating the need for NgModules:

```typescript
@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-list.html',
  styleUrls: ['./employee-list.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeeListComponent { }
```

**Benefits**:
- Simpler component structure
- Better tree-shaking
- Reduced boilerplate code
- Easier testing and maintenance

### 2. Functional Guards & Interceptors

The application uses functional programming patterns for cross-cutting concerns:

#### Auth Guard (Functional)
```typescript
export const authGuard: CanActivateFn = (route, state) => {
  // SSR safety check
  if (typeof window === 'undefined') {
    return true;
  }
  
  const router = inject(Router);
  const token = localStorage.getItem('token');
  
  if (token) {
    return true;
  }
  
  return router.createUrlTree(['/login'], {
    replaceUrl: true
  });
};
```

#### Token Interceptor (Functional)
```typescript
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  // Skip for auth endpoints
  if (req.url.includes('/login') || req.url.includes('/register')) {
    return next(req);
  }
  
  // SSR safety
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('token');
    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }
  }
  
  return next(req);
};
```

### 3. Change Detection Strategy

All components use `ChangeDetectionStrategy.OnPush` for optimal performance:

**Advantages**:
- Reduces change detection cycles
- Improves application performance
- Forces explicit data flow patterns
- Better predictability

---

## Application Layers

### Presentation Layer (Components)

**Responsibilities**:
- Render UI
- Handle user interactions
- Display data from services
- Manage component-level state

**Key Components**:
- `EmployeeListComponent`: Display and manage employee list
- `AddEmployeeComponent`: Create new employees
- `EditEmployeeComponent`: Update existing employees
- `LoginComponent`: User authentication
- `RegisterComponent`: User registration

### Business Logic Layer (Services)

**Responsibilities**:
- API communication
- Data transformation
- Business rules enforcement
- State management

**Key Services**:
- `EmployeeService`: Employee CRUD operations
- `AuthService`: Authentication and authorization

### Infrastructure Layer

**Responsibilities**:
- HTTP interceptors
- Route guards
- Error handling
- SSR configuration

---

## Data Flow

### Request Flow

```
User Action → Component → Service → HTTP Client → Interceptor → API
                                                        ↓
                                                   Add Token
                                                        ↓
                                                   Backend API
```

### Response Flow

```
Backend API → HTTP Response → Interceptor → Service → Component → View Update
                                   ↓
                            Error Handling
                                   ↓
                          Retry Logic (if applicable)
```

---

## Routing Architecture

### Route Configuration

The application uses a hierarchical routing structure with parent-level protection:

```typescript
export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    canActivateChild: [authGuard],
    runGuardsAndResolvers: 'always',
    children: [
      { path: 'employees', component: EmployeeListComponent },
      { path: 'add-employee', component: AddEmployeeComponent },
      { path: 'edit-employee/:id', component: EditEmployeeComponent },
      { path: '', redirectTo: 'employees', pathMatch: 'full' }
    ]
  }
];
```

**Key Features**:
- **Parent-level guard**: Single `canActivateChild` protects all child routes
- **No redundant guards**: Children inherit parent protection
- **Always run resolvers**: Ensures guards re-run on page reload
- **SSR-safe**: Guards check for server-side context

---

## State Management

### Local Component State

Components manage their own state using:
- Component properties
- RxJS Subjects for cross-component communication
- Service-level caching (when needed)

### Example Pattern

```typescript
export class EmployeeListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  employees: Employee[] = [];
  loading = false;
  
  ngOnInit() {
    this.loadEmployees();
  }
  
  loadEmployees() {
    this.loading = true;
    this.employeeService.getEmployees()
      .pipe(
        retryWhen(errors => errors.pipe(
          take(2),
          delay(1000)
        )),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: (data) => {
          this.employees = data;
          this.loading = false;
        },
        error: (err) => {
          this.loading = false;
          // Handle error
        }
      });
  }
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

---

## Error Handling Strategy

### Levels of Error Handling

1. **Component Level**
   - Display user-friendly messages
   - Show loading/error states
   - Handle navigation after errors

2. **Service Level**
   - Implement retry logic
   - Transform error responses
   - Log errors for debugging

3. **Global Level**
   - HTTP interceptors catch auth errors
   - Redirect to login on 401
   - Provide fallback UI

### Retry Logic Implementation

```typescript
this.employeeService.getEmployees()
  .pipe(
    retryWhen(errors => 
      errors.pipe(
        tap(err => console.log('Retry attempt', err)),
        take(2),  // Max 2 retries
        delay(1000)  // 1 second delay
      )
    )
  )
  .subscribe({
    next: (data) => { /* Success */ },
    error: (err) => { /* Final failure */ }
  });
```

---

## Server-Side Rendering (SSR)

### SSR Configuration

The application supports SSR for improved SEO and initial load performance.

**Bootstrap Configuration**:

```typescript
// main.ts (Client)
bootstrapApplication(AppComponent, appConfig);

// main.server.ts (Server)
const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(),
    ...appConfig.providers
  ]
};
```

### SSR Safety Patterns

**Guards**:
```typescript
if (typeof window === 'undefined') {
  return true;  // Allow during SSR
}
```

**Interceptors**:
```typescript
if (typeof window !== 'undefined') {
  const token = localStorage.getItem('token');
  // Use token
}
```

**Components**:
```typescript
ngOnInit() {
  if (typeof window !== 'undefined') {
    // Browser-only code
  }
}
```

---

## Performance Optimization

### Strategies Implemented

1. **OnPush Change Detection**
   - Reduces unnecessary checks
   - Explicit change triggers

2. **Lazy Loading**
   - Route-based code splitting
   - On-demand component loading

3. **RxJS Optimization**
   - Proper subscription cleanup
   - Use of `takeUntil` operator
   - Unsubscribe in `ngOnDestroy`

4. **Build Optimization**
   - Tree-shaking with standalone components
   - Production mode minification
   - Ahead-of-Time (AOT) compilation

---

## Security Architecture

### Token Management

```
┌─────────────┐
│   Login     │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│ Store Token in  │
│  localStorage   │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│ Token Attached  │
│ via Interceptor │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  API Requests   │
└─────────────────┘
```

### Protected Routes

```
User Request
    │
    ▼
Auth Guard
    │
    ├─ Token Exists? ──► Allow Access
    │                         │
    └─ No Token ──────────► Redirect to Login
```

---

## Testing Architecture

### Component Testing

```typescript
describe('EmployeeListComponent', () => {
  let component: EmployeeListComponent;
  let service: EmployeeService;
  
  beforeEach(() => {
    // Setup
  });
  
  it('should load employees on init', () => {
    // Test implementation
  });
});
```

### Service Testing

```typescript
describe('EmployeeService', () => {
  let service: EmployeeService;
  let httpMock: HttpTestingController;
  
  it('should fetch employees', () => {
    // Test HTTP calls
  });
});
```

---

## Deployment Architecture

### Build Process

```
Source Code
    │
    ▼
TypeScript Compilation
    │
    ▼
Angular Build
    │
    ├─ Client Bundle
    │
    └─ Server Bundle (SSR)
    │
    ▼
Production Dist
```

### Runtime Environment

- **Client**: Angular application runs in browser
- **Server**: Express server for SSR
- **API**: Backend service on localhost:5127

---

## Future Enhancements

### Potential Improvements

1. **State Management**: Implement NgRx or Signals for complex state
2. **Caching**: Add HTTP caching layer
3. **Offline Support**: Progressive Web App (PWA) capabilities
4. **Real-time**: WebSocket integration for live updates
5. **Internationalization**: Multi-language support
6. **Advanced Error Tracking**: Sentry or similar service

---

## Architectural Decision Records (ADRs)

### ADR-001: Standalone Components
**Decision**: Use standalone components instead of NgModules  
**Rationale**: Simpler architecture, better tree-shaking, future-proof  
**Status**: Accepted

### ADR-002: Functional Guards
**Decision**: Use functional guards instead of class-based  
**Rationale**: Less boilerplate, easier testing, aligned with modern Angular  
**Status**: Accepted

### ADR-003: OnPush Change Detection
**Decision**: Use OnPush strategy for all components  
**Rationale**: Better performance, explicit data flow  
**Status**: Accepted

### ADR-004: Parent-level Route Guards
**Decision**: Single `canActivateChild` on parent route  
**Rationale**: Avoid redundant guard execution, cleaner route config  
**Status**: Accepted

---

**Last Updated**: January 31, 2026
