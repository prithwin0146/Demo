import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);

  // SSR safety: Skip guard during server-side rendering
  if (typeof window === 'undefined') {
    console.log('Auth Guard - SSR context detected, skipping guard');
    return true;
  }

  const token = localStorage.getItem('token');
  console.log('Auth Guard - Token exists:', !!token, 'Value:', token ? token.substring(0, 20) + '...' : 'none');
  
  if (!token) {
    console.log('Auth Guard - No token found, redirecting to login');
    // Use replaceUrl to prevent breaking browser history on reload
    router.navigate(['/login'], { replaceUrl: true });
    return false;
  }
  
  console.log('Auth Guard - Token valid, allowing access to protected route');
  return true;
};


