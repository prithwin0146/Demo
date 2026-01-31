# Contributing Guide

Thank you for your interest in contributing to the Employee Management System! This guide will help you get started.

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Testing Requirements](#testing-requirements)
- [Common Patterns](#common-patterns)

---

## Code of Conduct

### Our Standards

- Be respectful and inclusive
- Accept constructive criticism
- Focus on what's best for the project
- Show empathy towards other contributors

---

## Getting Started

### Prerequisites

1. **Install Required Software**:
   - Node.js v20.x or higher
   - npm v11.7.0 or higher
   - Git
   - Angular CLI v21.0.3

2. **Fork the Repository**:
   ```bash
   # Fork on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/DEMOUI.git
   cd DEMOUI
   ```

3. **Install Dependencies**:
   ```bash
   npm install
   ```

4. **Create a Branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

---

## Development Workflow

### 1. Local Development

Start the development server:
```bash
npm start
```

Run tests in watch mode:
```bash
npm test
```

### 2. Make Your Changes

- Follow the coding standards below
- Write or update tests
- Update documentation if needed

### 3. Test Your Changes

```bash
# Run all tests
npm test

# Build the project
npm run build

# Check for errors
ng lint  # if linting is configured
```

### 4. Commit Your Changes

Follow the [commit guidelines](#commit-guidelines) below.

### 5. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

Then create a Pull Request on GitHub.

---

## Coding Standards

### Angular Components

#### Use Standalone Components

```typescript
@Component({
  selector: 'app-example',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './example.html',
  styleUrls: ['./example.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExampleComponent { }
```

#### Always Use OnPush Strategy

```typescript
changeDetection: ChangeDetectionStrategy.OnPush
```

### Services

#### Dependency Injection

```typescript
export class ExampleService {
  constructor(private http: HttpClient) { }
}
```

#### Return Observables

```typescript
getData(): Observable<Data[]> {
  return this.http.get<Data[]>(this.apiUrl);
}
```

### Subscription Management

Always clean up subscriptions:

```typescript
export class ExampleComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  ngOnInit() {
    this.service.getData()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => { /* Handle data */ },
        error: (err) => { /* Handle error */ }
      });
  }
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

### TypeScript

#### Use Strong Typing

```typescript
// Good
interface Employee {
  id: number;
  name: string;
  email: string;
}

function getEmployee(id: number): Observable<Employee> {
  return this.http.get<Employee>(`/api/employees/${id}`);
}

// Bad
function getEmployee(id: any): any {
  return this.http.get(`/api/employees/${id}`);
}
```

#### Avoid `any`

Use specific types or interfaces instead.

### HTML Templates

#### Use Proper Binding Syntax

```html
<!-- Property binding -->
<input [value]="employee.name">

<!-- Event binding -->
<button (click)="saveEmployee()">Save</button>

<!-- Two-way binding -->
<input [(ngModel)]="employee.name">

<!-- Structural directives -->
<div *ngIf="loading">Loading...</div>
<div *ngFor="let employee of employees">{{ employee.name }}</div>
```

### CSS/Styling

#### Use Component-Level Styles

Prefer component-scoped styles over global styles.

```css
/* employee-list.css */
.employee-card {
  padding: 1rem;
  border: 1px solid #ddd;
}
```

### File Naming Conventions

- Components: `component-name.ts`, `component-name.html`, `component-name.css`
- Services: `service-name.service.ts`
- Models: `model-name.models.ts`
- Guards: `guard-name.guard.ts`
- Interceptors: `interceptor-name.interceptor.ts`

---

## Commit Guidelines

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style changes (formatting, etc.)
- **refactor**: Code refactoring
- **test**: Adding or updating tests
- **chore**: Maintenance tasks

### Examples

```
feat(employees): add employee search functionality

Implemented a search feature that filters employees by name,
department, or position.

Closes #123
```

```
fix(auth): resolve token expiration issue

Fixed bug where expired tokens weren't properly handled,
causing authentication errors.

Fixes #456
```

```
docs(readme): update installation instructions

Added prerequisites section and clarified setup steps.
```

---

## Pull Request Process

### Before Submitting

- [ ] All tests pass
- [ ] Code follows style guidelines
- [ ] Documentation updated
- [ ] No console errors or warnings
- [ ] Commit messages follow guidelines

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
How has this been tested?

## Screenshots (if applicable)
Add screenshots for UI changes

## Checklist
- [ ] Tests pass
- [ ] Code follows style guide
- [ ] Documentation updated
- [ ] Self-review completed
```

### Review Process

1. Automated tests run
2. Code review by maintainer
3. Address feedback
4. Approval and merge

---

## Testing Requirements

### Component Tests

Every component must have a test file:

```typescript
describe('EmployeeListComponent', () => {
  let component: EmployeeListComponent;
  let service: EmployeeService;
  
  beforeEach(() => {
    // Setup
    service = new EmployeeService(httpClientStub);
    component = new EmployeeListComponent(service, router);
  });
  
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  
  it('should load employees on init', () => {
    // Test implementation
    expect(component.employees.length).toBeGreaterThan(0);
  });
});
```

### Service Tests

```typescript
describe('EmployeeService', () => {
  let service: EmployeeService;
  let httpMock: HttpTestingController;
  
  it('should fetch employees', () => {
    const mockEmployees = [
      { id: 1, name: 'John Doe', /* ... */ }
    ];
    
    service.getEmployees().subscribe(employees => {
      expect(employees).toEqual(mockEmployees);
    });
    
    const req = httpMock.expectOne('http://localhost:5127/api/employees');
    expect(req.request.method).toBe('GET');
    req.flush(mockEmployees);
  });
});
```

### Test Coverage

Aim for:
- **Components**: 80%+ coverage
- **Services**: 90%+ coverage
- **Guards/Interceptors**: 100% coverage

---

## Common Patterns

### 1. API Call with Retry Logic

```typescript
this.service.getData()
  .pipe(
    retryWhen(errors => 
      errors.pipe(
        take(2),
        delay(1000)
      )
    ),
    takeUntil(this.destroy$)
  )
  .subscribe({
    next: (data) => { /* Handle success */ },
    error: (err) => { /* Handle error */ }
  });
```

### 2. Loading State Management

```typescript
export class ExampleComponent {
  loading = false;
  
  loadData() {
    this.loading = true;
    this.service.getData()
      .subscribe({
        next: (data) => {
          this.data = data;
          this.loading = false;
        },
        error: (err) => {
          this.loading = false;
          alert('Error loading data');
        }
      });
  }
}
```

### 3. Form Handling

```typescript
export class FormComponent {
  formData = {
    name: '',
    email: ''
  };
  
  onSubmit() {
    if (this.isValid()) {
      this.service.save(this.formData)
        .subscribe({
          next: () => {
            alert('Saved successfully');
            this.router.navigate(['/list']);
          },
          error: (err) => {
            alert('Error saving data');
          }
        });
    }
  }
  
  isValid(): boolean {
    return this.formData.name.trim() !== '' &&
           this.formData.email.includes('@');
  }
}
```

### 4. SSR-Safe Code

```typescript
ngOnInit() {
  if (typeof window !== 'undefined') {
    // Browser-only code
    const token = localStorage.getItem('token');
  }
}
```

### 5. Route Guard Pattern

```typescript
export const myGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const service = inject(MyService);
  
  if (service.isAuthorized()) {
    return true;
  }
  
  return router.createUrlTree(['/unauthorized'], {
    replaceUrl: true
  });
};
```

---

## Documentation

### Code Comments

Use comments for complex logic:

```typescript
// Retry failed requests up to 2 times with 1 second delay
retryWhen(errors => 
  errors.pipe(
    take(2),
    delay(1000)
  )
)
```

### JSDoc for Public APIs

```typescript
/**
 * Retrieves all employees from the API
 * @returns Observable of Employee array
 * @throws Error if API request fails after retries
 */
getEmployees(): Observable<Employee[]> {
  return this.http.get<Employee[]>(this.apiUrl);
}
```

---

## Questions?

If you have questions:

1. Check existing documentation
2. Search closed issues
3. Open a new issue with the `question` label

---

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

**Happy Contributing! 🎉**
