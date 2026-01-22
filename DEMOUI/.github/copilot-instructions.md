# AI Copilot Instructions - Employee UI

## Project Overview
Employee Management System built with Angular 21, standalone components, and token-based authentication. API backend runs on `http://localhost:5127`.

## Architecture

### Core Patterns
- **Standalone Components**: All components use standalone API (`standalone: true`), no NgModules
- **Functional Routing**: Routes defined in `app.routes.ts` with functional guards (`authGuard`)
- **Functional Interceptors**: HTTP interceptors use functional `HttpInterceptorFn` (not class-based)
- **Bootstrap**: `bootstrapApplication()` in `main.ts` with providers, no traditional AppModule
- **ChangeDetectionStrategy**: Components use `OnPush` strategy for optimal performance

### Feature Structure
```
src/app/
├── employees/          # Employee CRUD module
│   ├── employee.service.ts   # API calls
│   ├── add-employee/   # Create new employee
│   ├── edit-employee/  # Update existing employee
│   └── employee-list/  # Display all employees (with retry logic & animated loader)
├── login/              # Authentication
│   └── auth.service.ts # Login/register API calls
├── dashboard/          # Protected dashboard view
├── auth-guard.ts       # Prevents access without token (SSR-safe, reload-proof)
├── token-interceptor.ts # Automatically attaches JWT (SSR-safe)
└── app.routes.ts       # Central route definitions (parent-level guard protection)
```

## Authentication & Authorization
1. **Token Storage**: JWT stored in `localStorage` (key: `'token'`)
2. **Auth Guard**: Functional guard in `auth-guard.ts` checks token existence
   - Returns `true` during SSR (server-side rendering)
   - Uses `replaceUrl: true` when redirecting to prevent history corruption on reload
   - Single `canActivateChild: [authGuard]` at parent level protects all child routes
3. **Token Injection**: `tokenInterceptor` auto-attaches `Authorization: Bearer {token}` header
4. **SSR Safety**: 
   - Interceptor checks `typeof window !== 'undefined'` before accessing localStorage
   - Auth guard checks `typeof window === 'undefined'` and returns true during SSR
5. **Bypass Routes**: Login/register endpoints skip token interception
6. **Reload Protection**: Routes use `runGuardsAndResolvers: 'always'` to handle page refreshes

## Key Services
- **AuthService** (`login/auth.service.ts`): `login()`, `register()`, `getToken()`
- **EmployeeService** (`employees/employee.service.ts`): `getEmployees()`, `createEmployee()`, `getEmployee()`, `updateEmployee()`, `deleteEmployee()`
- Both use hardcoded API URLs (not configuration-driven)

## Employee List Component Enhancements
- **Retry Logic**: Uses `retryWhen()` with 2 automatic retries on network errors (1-second delay)
- **401 Handling**: Detects unauthorized responses and redirects to login
- **Animated Loader**: Shows spinning loader during data fetch with disabled "Add Employee" button
- **ChangeDetectionStrategy.OnPush**: Optimized for performance
- **Proper Cleanup**: Uses `takeUntil()` with Subject for subscription management on destroy
- **Comprehensive Logging**: Console logs track retry attempts and data load status

## Route Protection Strategy
- **Parent-level guard**: `canActivateChild: [authGuard]` on root protected path protects all children
- **No individual `canActivate`**: Child routes inherit parent protection (prevents redundant guard execution)
- **Reload-safe**: `runGuardsAndResolvers: 'always'` ensures guards re-run on page reload
- **Clean redirects**: `replaceUrl: true` prevents browser history issues

## Development Commands
```bash
npm start              # Dev server on localhost:4200
npm run build          # Production build
npm test               # Run tests (Vitest)
npm run watch          # Build watch mode
npm run serve:ssr:*    # Run SSR server
```

## Important Implementation Details
- **Component Imports**: Each component manually imports `FormsModule`, `CommonModule` as needed
- **RxJS Subscriptions**: Components use `.subscribe()` with `next:`, `error:` callbacks
- **Error Handling**: Components handle errors appropriately (alerts, redirects, UI messages)
- **Routing**: Single parent-level `canActivateChild: [authGuard]` protects all child routes
- **API Endpoint**: `http://localhost:5127/api/` for all services (must be running)
- **Loader UI**: Animated spinner + disabled buttons during async operations

## Common Patterns to Follow
1. **Use `.subscribe()` with error/next callbacks** for async operations
2. **Always provide component templates/styles** via `templateUrl` and `styleUrls`
3. **Inject services in constructor** using private members
4. **Use parent-level `canActivateChild` guard** instead of individual route guards
5. **Handle SSR compatibility** by checking `typeof window === 'undefined'` before DOM/localStorage access
6. **Implement retry logic** for API calls using `retryWhen()` operator
7. **Use `ChangeDetectionStrategy.OnPush`** for performance optimization
8. **Cleanup subscriptions** with `takeUntil()` and proper `ngOnDestroy` implementation
9. **Show loaders** with disabled UI during async operations
10. **Handle 401 errors** with redirects to login using `replaceUrl: true`

## Testing
- Tests use **Vitest** (see `package.json`)
- All components have `.spec.ts` files
- Run with `npm test`

## Build & Deploy
- Production build: `npm run build` → creates `dist/` directory
- SSR enabled: Check `main.server.ts` and `server.ts`
- No build errors expected with current esbuild configuration
- Routes are reload-proof and SSR-safe

