import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'auth',
    pathMatch: 'full'
  },

  {
    path: 'auth',
    loadComponent: () =>
      import('./features/auth/auth-page.component')
        .then(c => c.AuthPageComponent)
  },

  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard-page.component')
        .then(c => c.DashboardPageComponent)
  },

  {
    path: '**',
    redirectTo: 'auth'
  }
];