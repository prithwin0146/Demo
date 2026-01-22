import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Routes with parameters should use SSR (not prerender) since we don't know the values
  {
    path: 'employees/edit/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'employees/add',
    renderMode: RenderMode.Server
  },
  {
    path: 'employees',
    renderMode: RenderMode.Server
  },
  {
    path: 'login',
    renderMode: RenderMode.Server
  },
  // Catch-all for other routes
  {
    path: '**',
    renderMode: RenderMode.Server
  }
];
