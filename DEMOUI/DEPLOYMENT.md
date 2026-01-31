# Deployment Guide

This guide covers deploying the Employee Management System to production environments.

---

## Table of Contents

- [Prerequisites](#prerequisites)
- [Build Process](#build-process)
- [Environment Configuration](#environment-configuration)
- [Deployment Options](#deployment-options)
- [Server-Side Rendering](#server-side-rendering)
- [Security Considerations](#security-considerations)
- [Performance Optimization](#performance-optimization)
- [Monitoring & Logging](#monitoring--logging)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software

- Node.js v20.x or higher
- npm v11.7.0 or higher
- Web server (Nginx, Apache, or similar)
- SSL certificate (for HTTPS)

### Backend API

Ensure your backend API is deployed and accessible:
- Production API URL
- CORS configured to allow frontend domain
- SSL enabled

---

## Build Process

### 1. Production Build

```bash
# Install dependencies
npm install

# Build for production
npm run build
```

This creates optimized files in the `dist/employee-ui/` directory.

### 2. Build Output

```
dist/employee-ui/
├── browser/           # Client-side bundle
│   ├── index.html
│   ├── main.js
│   ├── polyfills.js
│   └── styles.css
└── server/            # Server-side bundle (SSR)
    └── server.mjs
```

### 3. Build Configuration

Located in `angular.json`:

```json
{
  "configurations": {
    "production": {
      "optimization": true,
      "outputHashing": "all",
      "sourceMap": false,
      "namedChunks": false,
      "aot": true,
      "extractLicenses": true,
      "budgets": [
        {
          "type": "initial",
          "maximumWarning": "500kB",
          "maximumError": "1MB"
        }
      ]
    }
  }
}
```

---

## Environment Configuration

### API Endpoint Configuration

Currently hardcoded in services. For production, update:

**src/app/employees/employee.service.ts**:
```typescript
private apiUrl = 'https://api.yourproduction.com/api/employees';
```

**src/app/login/auth.service.ts**:
```typescript
private apiUrl = 'https://api.yourproduction.com/api';
```

### Recommended: Use Environment Files

Create environment configuration:

**src/environments/environment.prod.ts**:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.yourproduction.com/api'
};
```

Update services to use:
```typescript
import { environment } from '../environments/environment';

private apiUrl = environment.apiUrl;
```

---

## Deployment Options

### Option 1: Static Hosting (Client-Side Only)

Deploy the `dist/employee-ui/browser/` folder to:

- **Netlify**
- **Vercel**
- **AWS S3 + CloudFront**
- **Firebase Hosting**
- **GitHub Pages**

#### Example: Netlify

1. **Build**:
   ```bash
   npm run build
   ```

2. **Deploy**:
   ```bash
   # Install Netlify CLI
   npm install -g netlify-cli
   
   # Deploy
   netlify deploy --prod --dir=dist/employee-ui/browser
   ```

3. **Configure Redirects** (`netlify.toml`):
   ```toml
   [[redirects]]
     from = "/*"
     to = "/index.html"
     status = 200
   ```

---

### Option 2: SSR with Node.js Server

Deploy the full SSR application.

#### Step 1: Prepare Server

**server.ts** (already configured):
```typescript
import express from 'express';
import { APP_BASE_HREF } from '@angular/common';
import { CommonEngine } from '@angular/ssr';
import { fileURLToPath } from 'node:url';
import { dirname, join, resolve } from 'node:path';
import bootstrap from './src/main.server';

const app = express();
const serverDistFolder = dirname(fileURLToPath(import.meta.url));
const browserDistFolder = resolve(serverDistFolder, '../browser');

app.get('**', async (req, res, next) => {
  try {
    const html = await commonEngine.render({
      bootstrap,
      documentFilePath: indexHtml,
      url: req.originalUrl,
      publicPath: browserDistFolder,
      providers: [{ provide: APP_BASE_HREF, useValue: req.baseUrl }],
    });
    res.send(html);
  } catch (err) {
    next(err);
  }
});

const port = process.env['PORT'] || 4000;
app.listen(port, () => {
  console.log(`Node Express server listening on http://localhost:${port}`);
});
```

#### Step 2: Build SSR

```bash
npm run build
```

#### Step 3: Run Server

```bash
npm run serve:ssr:employee-ui
```

#### Step 4: Production Deployment

Use a process manager like **PM2**:

```bash
# Install PM2
npm install -g pm2

# Start application
pm2 start dist/employee-ui/server/server.mjs --name employee-ui

# Save PM2 configuration
pm2 save

# Enable startup script
pm2 startup
```

---

### Option 3: Docker Deployment

#### Dockerfile

```dockerfile
# Build stage
FROM node:20-alpine AS build

WORKDIR /app
COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# Production stage
FROM node:20-alpine

WORKDIR /app
COPY --from=build /app/dist ./dist
COPY --from=build /app/package*.json ./

RUN npm ci --production

EXPOSE 4000

CMD ["node", "dist/employee-ui/server/server.mjs"]
```

#### Build and Run

```bash
# Build image
docker build -t employee-ui .

# Run container
docker run -p 4000:4000 employee-ui
```

#### Docker Compose

```yaml
version: '3.8'

services:
  frontend:
    build: .
    ports:
      - "4000:4000"
    environment:
      - NODE_ENV=production
      - PORT=4000
    restart: unless-stopped
```

---

## Server-Side Rendering

### Benefits

- Improved SEO
- Faster initial page load
- Better social media sharing

### SSR Configuration

Already configured in:
- `src/main.server.ts`
- `src/app/app.config.server.ts`
- `server.ts`

### SSR Considerations

1. **No window/document access during SSR**:
   ```typescript
   if (typeof window !== 'undefined') {
     // Browser-only code
   }
   ```

2. **localStorage handling**:
   ```typescript
   const token = typeof window !== 'undefined' 
     ? localStorage.getItem('token') 
     : null;
   ```

3. **Guards are SSR-safe**:
   ```typescript
   if (typeof window === 'undefined') {
     return true; // Allow during SSR
   }
   ```

---

## Security Considerations

### 1. HTTPS Only

Force HTTPS in production:

**Nginx Configuration**:
```nginx
server {
    listen 80;
    server_name yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com;
    
    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;
    
    location / {
        proxy_pass http://localhost:4000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 2. Environment Variables

Never commit sensitive data. Use environment variables:

```bash
export API_URL=https://api.production.com
export JWT_SECRET=your_secret_key
```

### 3. Content Security Policy

Add CSP headers:

```nginx
add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';";
```

### 4. CORS Configuration

Backend should only allow your frontend domain:

```javascript
app.use(cors({
  origin: 'https://yourdomain.com',
  credentials: true
}));
```

---

## Performance Optimization

### 1. Enable Compression

**Nginx**:
```nginx
gzip on;
gzip_vary on;
gzip_min_length 256;
gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
```

### 2. Browser Caching

```nginx
location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
    expires 1y;
    add_header Cache-Control "public, immutable";
}
```

### 3. CDN Integration

Serve static assets via CDN:
- Upload `dist/employee-ui/browser/assets/` to CDN
- Update asset URLs in build config

### 4. Lazy Loading

Already implemented with route-based code splitting.

---

## Monitoring & Logging

### 1. Application Monitoring

Use services like:
- **New Relic**
- **Datadog**
- **Application Insights**

### 2. Error Tracking

Implement error tracking:

**Install Sentry**:
```bash
npm install @sentry/angular
```

**Configure** (`src/main.ts`):
```typescript
import * as Sentry from "@sentry/angular";

Sentry.init({
  dsn: "YOUR_SENTRY_DSN",
  environment: "production"
});
```

### 3. Log Aggregation

For SSR logs, use:
- **CloudWatch** (AWS)
- **Stackdriver** (GCP)
- **Winston** (Node.js logger)

---

## Troubleshooting

### Build Errors

**Issue**: Build fails with memory error
```bash
# Solution: Increase Node memory
NODE_OPTIONS=--max_old_space_size=4096 npm run build
```

### SSR Issues

**Issue**: "localStorage is not defined"
```typescript
// Solution: Add window check
if (typeof window !== 'undefined') {
  localStorage.getItem('token');
}
```

### Routing Issues

**Issue**: 404 on page refresh
```nginx
# Solution: Configure Nginx to serve index.html for all routes
try_files $uri $uri/ /index.html;
```

### Performance Issues

**Issue**: Slow initial load
- Check bundle sizes with `npm run build -- --stats-json`
- Analyze with webpack-bundle-analyzer
- Enable compression and caching

---

## Deployment Checklist

Pre-deployment:
- [ ] Update API URLs to production
- [ ] Run production build locally
- [ ] Test SSR functionality
- [ ] Verify all routes work
- [ ] Check authentication flow
- [ ] Test error handling
- [ ] Verify HTTPS configuration
- [ ] Configure CORS
- [ ] Set up monitoring
- [ ] Configure backup strategy

Post-deployment:
- [ ] Verify application loads
- [ ] Test all features
- [ ] Check SSL certificate
- [ ] Monitor error logs
- [ ] Performance testing
- [ ] Security audit

---

## Rollback Strategy

### Quick Rollback

If using PM2:
```bash
pm2 stop employee-ui
pm2 delete employee-ui
# Deploy previous version
pm2 start previous-version/server.mjs --name employee-ui
```

### Version Management

Keep previous builds:
```bash
mv dist/employee-ui dist/employee-ui-backup-$(date +%Y%m%d)
```

---

**Deployment Date**: January 31, 2026  
**Version**: 1.0.0
