# Security Guide

This document outlines security best practices and measures implemented in the Employee Management System.

---

## Table of Contents

- [Authentication & Authorization](#authentication--authorization)
- [Token Management](#token-management)
- [Data Protection](#data-protection)
- [API Security](#api-security)
- [Frontend Security](#frontend-security)
- [Common Vulnerabilities](#common-vulnerabilities)
- [Security Checklist](#security-checklist)
- [Incident Response](#incident-response)

---

## Authentication & Authorization

### JWT Token-Based Authentication

The application uses JSON Web Tokens (JWT) for authentication.

#### Login Flow

```
1. User submits credentials (username/password)
2. Backend validates credentials
3. Backend generates JWT token
4. Token stored in localStorage
5. Token attached to all subsequent requests
6. Backend validates token for protected routes
```

#### Token Storage

**Current Implementation**:
```typescript
// Store token after login
localStorage.setItem('token', response.token);

// Retrieve token for requests
const token = localStorage.getItem('token');

// Remove token on logout
localStorage.removeItem('token');
```

**Security Considerations**:
- localStorage is vulnerable to XSS attacks
- Consider httpOnly cookies for production
- Implement token expiration
- Use refresh tokens for extended sessions

### Route Protection

#### Auth Guard Implementation

```typescript
export const authGuard: CanActivateFn = (route, state) => {
  // SSR safety check
  if (typeof window === 'undefined') {
    return true;
  }
  
  const router = inject(Router);
  const token = localStorage.getItem('token');
  
  if (token) {
    // TODO: Validate token expiration
    return true;
  }
  
  // Redirect to login with replaceUrl to prevent history issues
  return router.createUrlTree(['/login'], {
    replaceUrl: true
  });
};
```

**Security Features**:
- Prevents unauthorized access to protected routes
- SSR-safe implementation
- Clean navigation without history pollution
- Parent-level guard protects all child routes

---

## Token Management

### Token Interceptor

Automatically attaches JWT to API requests:

```typescript
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  // Bypass auth endpoints
  if (req.url.includes('/login') || req.url.includes('/register')) {
    return next(req);
  }
  
  // SSR safety check
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('token');
    
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }
  }
  
  return next(req);
};
```

### Token Security Best Practices

#### 1. Token Expiration

**Recommended Implementation**:
```typescript
interface DecodedToken {
  exp: number;
  userId: number;
  username: string;
}

function isTokenExpired(token: string): boolean {
  try {
    const decoded = jwt_decode<DecodedToken>(token);
    const currentTime = Date.now() / 1000;
    return decoded.exp < currentTime;
  } catch {
    return true;
  }
}

// Use in auth guard
if (token && !isTokenExpired(token)) {
  return true;
}
```

#### 2. Refresh Tokens

**Future Enhancement**:
```typescript
interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}

function refreshAccessToken(): Observable<RefreshTokenResponse> {
  const refreshToken = localStorage.getItem('refreshToken');
  return this.http.post<RefreshTokenResponse>(
    `${this.apiUrl}/refresh`,
    { refreshToken }
  );
}
```

#### 3. Secure Token Storage

**Current**: localStorage (vulnerable to XSS)

**Recommended for Production**: httpOnly cookies

```typescript
// Backend sets httpOnly cookie
res.cookie('token', jwtToken, {
  httpOnly: true,
  secure: true,
  sameSite: 'strict',
  maxAge: 3600000 // 1 hour
});

// Frontend: Token automatically sent with requests
// No localStorage access needed
```

---

## Data Protection

### Input Validation

Always validate user input:

```typescript
// Client-side validation
isValid(): boolean {
  if (!this.employee.name || this.employee.name.trim() === '') {
    return false;
  }
  
  if (!this.employee.email || !this.isValidEmail(this.employee.email)) {
    return false;
  }
  
  if (this.employee.salary < 0) {
    return false;
  }
  
  return true;
}

isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}
```

**Note**: Always validate on backend as well. Client-side validation can be bypassed.

### Data Sanitization

Prevent XSS attacks:

```typescript
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

constructor(private sanitizer: DomSanitizer) {}

getSafeHtml(html: string): SafeHtml {
  return this.sanitizer.sanitize(SecurityContext.HTML, html);
}
```

### Sensitive Data Handling

**Never**:
- Log passwords or tokens to console
- Display tokens in UI
- Store sensitive data in localStorage without encryption
- Include sensitive data in URLs

**Always**:
- Use HTTPS in production
- Encrypt sensitive data at rest
- Sanitize user input
- Validate on both client and server

---

## API Security

### CORS Configuration

Backend should restrict origins:

```javascript
// Backend CORS configuration
app.use(cors({
  origin: 'https://yourdomain.com', // Specific domain only
  credentials: true,
  methods: ['GET', 'POST', 'PUT', 'DELETE'],
  allowedHeaders: ['Content-Type', 'Authorization']
}));
```

### Rate Limiting

Protect against brute force attacks:

```javascript
// Backend rate limiting (example with express-rate-limit)
const rateLimit = require('express-rate-limit');

const loginLimiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutes
  max: 5, // 5 requests per windowMs
  message: 'Too many login attempts, please try again later'
});

app.post('/api/login', loginLimiter, (req, res) => {
  // Login logic
});
```

### SQL Injection Prevention

**Backend**: Use parameterized queries

```sql
-- Bad (vulnerable to SQL injection)
SELECT * FROM employees WHERE id = ${req.params.id}

-- Good (parameterized query)
SELECT * FROM employees WHERE id = ?
```

### API Endpoint Protection

All employee endpoints require authentication:

```typescript
// Token must be present and valid
GET    /api/employees        - Requires: Bearer token
GET    /api/employees/:id    - Requires: Bearer token
POST   /api/employees        - Requires: Bearer token
PUT    /api/employees/:id    - Requires: Bearer token
DELETE /api/employees/:id    - Requires: Bearer token
```

Public endpoints:
```typescript
POST   /api/login            - No token required
POST   /api/register         - No token required
```

---

## Frontend Security

### XSS Prevention

Angular provides built-in XSS protection:

1. **Template Binding**: Automatically sanitizes
   ```html
   <!-- Safe: Angular escapes this -->
   <div>{{ employee.name }}</div>
   ```

2. **Property Binding**: Also sanitized
   ```html
   <!-- Safe -->
   <input [value]="employee.name">
   ```

3. **Dangerous HTML**: Requires explicit bypass
   ```typescript
   // Only use when absolutely necessary
   constructor(private sanitizer: DomSanitizer) {}
   
   getSafeHtml(html: string): SafeHtml {
     return this.sanitizer.bypassSecurityTrustHtml(html);
   }
   ```

### Content Security Policy (CSP)

Add CSP headers to prevent XSS:

```html
<!-- In index.html -->
<meta http-equiv="Content-Security-Policy" 
      content="default-src 'self'; 
               script-src 'self' 'unsafe-inline'; 
               style-src 'self' 'unsafe-inline';
               img-src 'self' data: https:;">
```

Or via server headers (Nginx):
```nginx
add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';";
```

### HTTPS Enforcement

**Always use HTTPS in production**:

```typescript
// Redirect HTTP to HTTPS (Nginx)
server {
    listen 80;
    server_name yourdomain.com;
    return 301 https://$server_name$request_uri;
}
```

### Secure Headers

Implement security headers:

```nginx
# Nginx configuration
add_header X-Frame-Options "SAMEORIGIN" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-XSS-Protection "1; mode=block" always;
add_header Referrer-Policy "strict-origin-when-cross-origin" always;
add_header Permissions-Policy "geolocation=(), microphone=(), camera=()" always;
```

---

## Common Vulnerabilities

### 1. Cross-Site Scripting (XSS)

**Risk**: Malicious scripts injected into application

**Mitigation**:
- Angular's built-in sanitization
- CSP headers
- Validate and sanitize all user input
- Never use `innerHTML` with user data

### 2. Cross-Site Request Forgery (CSRF)

**Risk**: Unauthorized actions performed on behalf of authenticated user

**Mitigation**:
- Use CSRF tokens
- Implement SameSite cookie attribute
- Verify Origin/Referer headers

**Future Implementation**:
```typescript
// Backend CSRF protection
const csrf = require('csurf');
app.use(csrf({ cookie: true }));

// Send token to frontend
app.get('/api/csrf-token', (req, res) => {
  res.json({ csrfToken: req.csrfToken() });
});
```

### 3. Insecure Direct Object References

**Risk**: Accessing unauthorized resources by changing IDs

**Mitigation**:
```typescript
// Backend: Verify user owns resource
app.get('/api/employees/:id', authMiddleware, (req, res) => {
  const employeeId = req.params.id;
  const userId = req.user.id;
  
  // Verify user has permission to access this employee
  if (!userCanAccessEmployee(userId, employeeId)) {
    return res.status(403).json({ error: 'Forbidden' });
  }
  
  // Proceed with request
});
```

### 4. Sensitive Data Exposure

**Risk**: Exposing sensitive information

**Mitigation**:
- Never log passwords or tokens
- Use HTTPS
- Encrypt sensitive data
- Implement proper access controls

### 5. Broken Authentication

**Risk**: Weak authentication mechanisms

**Mitigation**:
- Strong password requirements
- Account lockout after failed attempts
- Token expiration
- Secure password storage (bcrypt, argon2)

---

## Security Checklist

### Development

- [ ] All user input is validated
- [ ] No console.log of sensitive data
- [ ] HTTPS used for all API calls
- [ ] Passwords not stored in code
- [ ] Environment variables for secrets
- [ ] Token expiration implemented
- [ ] XSS protection verified
- [ ] CSRF protection enabled

### Deployment

- [ ] HTTPS enforced
- [ ] Security headers configured
- [ ] CORS properly configured
- [ ] Rate limiting enabled
- [ ] Error messages don't expose system details
- [ ] Logging configured (not exposing sensitive data)
- [ ] Regular dependency updates
- [ ] Security audit performed

### Ongoing

- [ ] Regular dependency updates
- [ ] Security patch monitoring
- [ ] Access logs reviewed
- [ ] Error logs monitored
- [ ] Security testing performed
- [ ] Penetration testing (periodic)

---

## Incident Response

### If Security Breach Occurs

1. **Immediate Actions**:
   - Disable affected accounts
   - Revoke compromised tokens
   - Block suspicious IP addresses
   - Take affected systems offline if necessary

2. **Investigation**:
   - Review access logs
   - Identify attack vector
   - Assess data exposure
   - Document timeline

3. **Remediation**:
   - Patch vulnerability
   - Reset all user passwords
   - Issue new tokens
   - Update security measures

4. **Communication**:
   - Notify affected users
   - Report to relevant authorities
   - Document lessons learned
   - Update security policies

### Security Contacts

- **Security Team**: security@yourcompany.com
- **Emergency**: emergency@yourcompany.com

---

## Security Updates

### Dependency Management

Keep dependencies updated:

```bash
# Check for vulnerabilities
npm audit

# Fix vulnerabilities
npm audit fix

# Update packages
npm update
```

### Security Advisories

Monitor:
- Angular security advisories
- npm security advisories
- OWASP Top 10
- CVE database

---

## Additional Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Angular Security Guide](https://angular.io/guide/security)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [Web Security Cheat Sheet](https://cheatsheetseries.owasp.org/)

---

**Last Updated**: January 31, 2026  
**Version**: 1.0
