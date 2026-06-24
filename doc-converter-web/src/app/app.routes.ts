import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';

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
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard-page')
        .then(m => m.DashboardPage)
  },

  {
  path: 'dashboard/word-to-pdf',
  canActivate: [authGuard],
  loadComponent: () =>
  import('./features/conversions/word-to-pdf/word-to-pdf-page')
  .then(m => m.WordToPdfPage)
  },

  {
  path: 'dashboard/pdf-merge',
  canActivate: [authGuard],
  loadComponent: () =>
  import('./features/conversions/pdf-merge/pdf-merge-page')
  .then(m => m.PdfMergePage)
  },

  {
  path: 'dashboard/pdf-split',
  canActivate: [authGuard],
  loadComponent: () =>
  import('./features/conversions/pdf-split/pdf-split-page')
  .then(m => m.PdfSplitPage)
  },


  {
    path: '**',
    redirectTo: 'auth'
  }
];