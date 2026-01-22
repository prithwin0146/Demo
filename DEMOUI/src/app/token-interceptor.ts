import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  // Skip intercepting if localStorage is not available (SSR safety)
  if (typeof window === 'undefined') return next(req);

  const token = localStorage.getItem('token');
  const router = inject(Router);

  // Skip login/register
  if (
    req.url.includes('/Users/login') ||
    req.url.includes('/Users/register')
  ) {
    return next(req);
  }

  // Attach token
  const authReq = token
    ? req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      })
    : req;

  return next(authReq).pipe(
    catchError((err) => {
      if (err.status === 401) {
        console.warn('Token expired or invalid, clearing storage and redirecting to login');
        localStorage.removeItem('token');
        router.navigate(['/login']);
      }
      throw err;
    })
  );
};
