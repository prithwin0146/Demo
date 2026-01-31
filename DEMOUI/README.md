# Employee Management System

A modern, full-featured Employee Management System built with **Angular 21** using standalone components, TypeScript, and token-based authentication. This application provides a complete CRUD interface for managing employee records with secure authentication and authorization.

## 📋 Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Development](#development)
- [Architecture](#architecture)
- [Authentication & Security](#authentication--security)
- [API Integration](#api-integration)
- [Testing](#testing)
- [Build & Deployment](#build--deployment)
- [Project Structure](#project-structure)
- [Contributing](#contributing)

---

## ✨ Features

- **User Authentication**: Secure login and registration with JWT tokens
- **Employee Management**: Full CRUD operations (Create, Read, Update, Delete)
- **Protected Routes**: Role-based access control with route guards
- **Responsive Design**: Mobile-friendly UI built with modern Angular
- **Server-Side Rendering (SSR)**: Improved SEO and performance
- **Retry Logic**: Automatic retry on network errors with visual feedback
- **Loading States**: Animated loaders during async operations
- **Error Handling**: Comprehensive error handling with user feedback
- **Performance Optimized**: OnPush change detection strategy

---

## 🛠 Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **Angular** | 21.0.0 | Frontend framework |
| **TypeScript** | 5.9.2 | Type-safe JavaScript |
| **RxJS** | 7.8.0 | Reactive programming |
| **Vitest** | 4.0.8 | Unit testing |
| **Express** | 5.1.0 | SSR server |
| **Angular SSR** | 21.0.3 | Server-side rendering |

---

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js**: v20.x or higher
- **npm**: v11.7.0 or higher
- **Angular CLI**: v21.0.3
- **Backend API**: Running on `http://localhost:5127`

---

## 🚀 Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd DEMOUI
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Verify installation**:
   ```bash
   ng version
   ```

---

## ⚙️ Configuration

### API Endpoint

The application connects to a backend API at `http://localhost:5127/api/`. Ensure your backend server is running before starting the application.

### Environment Setup

- **Development**: Uses localhost:4200
- **Production**: Builds to `dist/` directory
- **SSR**: Runs on configured port (see `server.ts`)

---

## 🏃 Development

### Start Development Server

```bash
npm start
# or
ng serve
```

Navigate to `http://localhost:4200/`. The application will automatically reload when you modify source files.

### Available Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Start development server |
| `npm run build` | Build for production |
| `npm run watch` | Build in watch mode |
| `npm test` | Run unit tests |
| `npm run serve:ssr:employee-ui` | Run SSR server |

---

## 🏗 Architecture

### Core Patterns

This application uses modern Angular best practices:

- **Standalone Components**: All components use `standalone: true` (no NgModules)
- **Functional Guards**: Route protection using functional `authGuard`
- **Functional Interceptors**: HTTP interceptors using `HttpInterceptorFn`
- **Bootstrap API**: Uses `bootstrapApplication()` in `main.ts`
- **OnPush Strategy**: Optimized change detection for better performance

### Component Structure

```
src/app/
├── app.ts                   # Root component
├── app.routes.ts            # Route definitions
├── auth-guard.ts            # Authentication guard
├── token-interceptor.ts     # JWT token interceptor
├── employees/               # Employee module
│   ├── employee.service.ts  # Employee API service
│   ├── employee.models.ts   # TypeScript interfaces
│   ├── employee-list/       # List all employees
│   ├── add-employee/        # Create new employee
│   └── edit-employee/       # Update employee
├── login/                   # Authentication
│   ├── auth.service.ts      # Login/register service
│   └── login.ts             # Login component
└── register/                # User registration
    └── register.ts          # Register component
```

---

## 🔐 Authentication & Security

### Token-Based Authentication

1. **Login Flow**:
   - User submits credentials to `/api/login`
   - Backend returns JWT token
   - Token stored in `localStorage` (key: `'token'`)
   - Subsequent requests include token in `Authorization` header

2. **Protected Routes**:
   - Parent-level `canActivateChild` guard on root path
   - Child routes inherit protection automatically
   - Unauthorized users redirected to login with `replaceUrl: true`

3. **SSR Safety**:
   - Guards check `typeof window === 'undefined'` for SSR
   - Interceptor validates window object before localStorage access
   - Returns `true` during server-side rendering

4. **Reload Protection**:
   - Routes configured with `runGuardsAndResolvers: 'always'`
   - Guards re-run on page refresh
   - Clean redirects prevent history corruption

### Security Features

- JWT tokens automatically attached to all API requests
- Login/register endpoints bypass token interception
- 401 responses trigger automatic redirect to login
- Secure token storage and retrieval

---

## 🌐 API Integration

### Employee Service

**Base URL**: `http://localhost:5127/api/employees`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Get all employees |
| GET | `/{id}` | Get employee by ID |
| POST | `/` | Create new employee |
| PUT | `/{id}` | Update employee |
| DELETE | `/{id}` | Delete employee |

### Authentication Service

**Base URL**: `http://localhost:5127/api`

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/login` | User login |
| POST | `/register` | User registration |

### Retry Logic

- **Automatic Retries**: 2 retry attempts on network errors
- **Delay**: 1-second delay between retries
- **Error Handling**: Comprehensive logging and user feedback
- **Loading States**: Visual indicators during retries

---

## 🧪 Testing

### Unit Testing with Vitest

```bash
npm test
```

- All components have `.spec.ts` files
- Tests use Vitest framework
- Coverage includes components, services, guards, and interceptors

### Test Structure

```typescript
// Example test pattern
describe('ComponentName', () => {
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
```

---

## 📦 Build & Deployment

### Production Build

```bash
npm run build
```

Output: `dist/employee-ui/` directory

### Build Configuration

- **Optimization**: Enabled by default
- **Source Maps**: Disabled in production
- **Bundle Size**: Optimized for performance
- **Output**: Hashed filenames for cache busting

### SSR Deployment

```bash
npm run serve:ssr:employee-ui
```

Runs the SSR server using the production build.

### Deployment Checklist

- [ ] Update API endpoint URLs
- [ ] Configure environment variables
- [ ] Run production build
- [ ] Test SSR functionality
- [ ] Verify authentication flow
- [ ] Check route protection
- [ ] Validate error handling

---

## 📁 Project Structure

```
DEMOUI/
├── src/
│   ├── app/                    # Application source
│   │   ├── employees/          # Employee feature module
│   │   ├── login/              # Authentication
│   │   └── register/           # User registration
│   ├── index.html              # Main HTML file
│   ├── main.ts                 # Client bootstrap
│   ├── main.server.ts          # Server bootstrap
│   └── styles.css              # Global styles
├── public/                     # Static assets
├── angular.json                # Angular configuration
├── package.json                # Dependencies
├── tsconfig.json               # TypeScript config
└── README.md                   # This file
```

---

## 🎨 Code Style & Conventions

### Prettier Configuration

```json
{
  "printWidth": 100,
  "singleQuote": true,
  "parser": "angular"
}
```

### Coding Guidelines

1. **Components**: Use standalone API, OnPush strategy
2. **Services**: Inject in constructor with private modifier
3. **Subscriptions**: Always use `takeUntil()` for cleanup
4. **Error Handling**: Use `.subscribe()` with error callbacks
5. **Imports**: Manual imports for FormsModule, CommonModule
6. **Routing**: Parent-level guards, functional approach
7. **SSR**: Always check `typeof window` before DOM access

---

## 🤝 Contributing

### Development Workflow

1. Create a feature branch
2. Make your changes
3. Run tests: `npm test`
4. Build project: `npm run build`
5. Submit pull request

### Common Patterns

- Use `retryWhen()` for API calls with retry logic
- Implement loading states for async operations
- Handle 401 errors with redirects to login
- Use `ChangeDetectionStrategy.OnPush` for performance
- Follow SSR-safe practices (window checks)

---

## 📞 Support

For issues, questions, or contributions, please refer to the project repository.

---

## 📄 License

This project is private and proprietary.

---

**Built with ❤️ using Angular 21**
