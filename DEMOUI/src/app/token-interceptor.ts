
import { HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);
  
  // Skip intercepting if localStorage is not available (SSR safety)
  if (!isPlatformBrowser(platformId)) return next(req);

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
        if (isPlatformBrowser(platformId)) {
          localStorage.removeItem('token');
        }
        router.navigate(['/login']);
      }
      throw err;
    })
  );
};
