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
      import('./features/auth/auth-page')
        .then(m => m.AuthPage)
  },

  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard-page')
        .then(m => m.DashboardPage)
  },

  {
    path: '**',
    redirectTo: 'auth'
  }
];