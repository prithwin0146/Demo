# AI Coding Agent Instructions

## Project Overview
Employee Management System: .NET 8.0 backend (DEMOAPI) + Angular 21 frontend (DEMOUI). JWT authentication, role-based access, full CRUD for employees/departments/projects.

## Architecture

**Monorepo Structure**: `/DEMOAPI` (backend) + `/DEMOUI` (frontend)

**Backend Layering**:
```
Controllers → Services → Repositories → Database (EF Core + Stored Procedures)
```

**Data Access**: Hybrid approach using EF Core for basic CRUD and SQL Server stored procedures for complex pagination/filtering queries. Custom extension method `DbContextExtensions.LoadStoredProc<T>()` enables type-safe stored procedure execution.

**Frontend**: Angular 21 standalone components (no NgModules), feature-based organization, server-side rendering enabled.

## Development Workflow

**Prerequisites**: SQL Server LocalDB with `TaskDb` database, .NET 8.0 SDK, Node.js v20+

**Backend** (runs on `http://localhost:5127`):
```bash
cd DEMOAPI
dotnet run
```

**Frontend** (runs on `http://localhost:4200`):
```bash
cd DEMOUI
npm install
npm start
```

**Database**: Connection string in `DEMOAPI/appsettings.json`. Stored procedures located in `DEMOAPI/SQL/sp_*.sql`.

## Critical Backend Patterns

**Stored Procedure Execution**: Use custom extension in `/DEMOAPI/Extensions/DbContextExtensions.cs`:
```csharp
var results = await context.LoadStoredProc<EmployeePagedResult>(
    "sp_GetEmployeesPaged",
    new SqlParameter("@PageNumber", pageNumber)
);
```

**Repository Pattern**: All repositories inherit from generic `Repository<T>` base class (`/Repositories/Repository.cs`) providing standard CRUD operations. Override methods when custom logic is needed.

**Pagination**: Repositories return tuples `(List<T> Data, int TotalCount)`, converted to `PagedResponse<T>` at service layer.

**Mapping**: Custom service-based mappers (e.g., `IEmployeeMapper`) instead of AutoMapper. Supports async mapping for database lookups.

**DI Configuration**: All services/repositories registered as Scoped in `/DEMOAPI/Program.cs`.

**Authentication**: JWT Bearer tokens configured in `appsettings.json` (Jwt:Key, Jwt:Issuer, Jwt:Audience). Tokens expire after 1 hour.

**Error Handling**: Custom exceptions in `/Exceptions/` directory (e.g., `InvalidPasswordException`, `DuplicateEmailException`). Controllers catch and return appropriate HTTP status codes.

## Critical Frontend Patterns

**Component Naming**: Files are `component-name.ts` (NOT `component-name.component.ts`). Example: `employee-list.ts`, `login.ts`.

**API URLs**: Hardcoded in each service (no environment files). Base: `http://localhost:5127/api/[EntityName]`. Update all service files when backend URL changes.

**Manual Caching**: See `/DEMOUI/src/app/employees/employee.service.ts` for pattern - 5-minute TTL with explicit `invalidateCache()` calls after mutations.

**SSR Safety**: All components check `isPlatformBrowser(this.platformId)` before accessing `localStorage` or making API calls.

**Token Interceptor** (`/DEMOUI/src/app/token-interceptor.ts`): Automatically attaches JWT to all requests except `/login` and `/register`. Handles 401 by redirecting to login.

**Auth Guard** (`/DEMOUI/src/app/auth-guard.ts`): Functional guard using `CanActivateFn`. Checks localStorage for token, SSR-safe with `isPlatformBrowser`.

**Forms**: Template-driven with `FormsModule`, two-way binding via `[(ngModel)]`.

## Database Conventions

**DbContext**: `TaskDbContext` in `/DEMOAPI/Models/TaskDbContext.cs`.

**Stored Procedures**: Located in `/DEMOAPI/SQL/`. Use dynamic SQL with parameter validation to prevent injection. Example: `sp_GetEmployeesPaged.sql`.

**Paged Result Models**: Configured as keyless entities in DbContext:
```csharp
modelBuilder.Entity<EmployeePagedResult>().HasNoKey();
```

**Connection String**: Primary in `appsettings.json`. Avoid hardcoded fallback in `TaskDbContext.OnConfiguring()`.

## Integration & API Patterns

**REST Endpoints**: `/api/{EntityName}` (plural). Example: `/api/Employees`, `/api/Projects`.

**Pagination Query Params**: `?pageNumber=1&pageSize=10&sortBy=Id&sortOrder=ASC&searchTerm=&departmentId=`

**JWT Flow**: Login → store token in localStorage → interceptor adds `Authorization: Bearer {token}` header → 401 response clears token and redirects.

**CORS**: Configured for `http://localhost:4200` in `DEMOAPI/Program.cs`.

## File Organization

**Backend**:
- `/Controllers` - Plural names (EmployeesController)
- `/Services` - Singular names with interfaces (IEmployeeService, EmployeeService)
- `/Repositories` - Singular names with interfaces (IEmployeeRepository, EmployeeRepository)
- `/Models` - EF Core entities + paged result models
- `/DTOs` - Request/response objects with `[JsonPropertyName]` for camelCase

**Frontend**:
- `src/app/{feature}/` - Feature folders (employees, projects, departments)
- Each feature: `{entity}.service.ts`, `{entity}.models.ts`, component subfolders
- Component folders: `{component-name}/{component-name}.ts|.html|.css`
- Shared models: `src/app/shared/`

## Quick Reference

**Add New Entity**:
1. Backend: Create Model → DTO → Repository (extend Repository<T>) → Service → Controller
2. Add DbSet to TaskDbContext, optional stored procedure in `/SQL`
3. Frontend: Create models.ts → service.ts → components (list/add/edit)
4. Update routing in `app.routes.ts`

**Common Commands**:
- Backend build: `dotnet build` (from DEMOAPI)
- Frontend build: `npm run build` (from DEMOUI)
- Run tests: `npm test` (frontend only, uses Vitest)
