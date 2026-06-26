import {
  HttpErrorResponse,
  HttpInterceptorFn
} from '@angular/common/http';

import { inject } from '@angular/core';

import {
  catchError,
  throwError
} from 'rxjs';

import { Router } from '@angular/router';

import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (
  req,
  next
) => {
  const authService = inject(AuthService);
  const notification = inject(NotificationService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case 400:
          notification.error(
            error.error?.message ??
            'Request inválida'
          );
          break;

        case 401:
          notification.error(
            'Sesión expirada'
          );

          authService.logout();

          router.navigate(['/auth']);

          break;

        case 404:
          notification.error(
            'Recurso no encontrado'
          );
          break;

        case 500:
          notification.error(
            'Error de servidor'
          );
          break;

        default:
          notification.error(
            'Error inesperado'
          );
      }

      return throwError(() => error);
    })
  );
};