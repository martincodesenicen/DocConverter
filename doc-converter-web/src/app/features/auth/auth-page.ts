import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
FormBuilder,
ReactiveFormsModule,
Validators
} from '@angular/forms';

import { Router } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

import {
LoginRequest,
RegisterRequest
} from '../../core/models/auth.models';

import {MatTabsModule} from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { NotificationService } from '../../core/services/notification.service';

@Component({
selector: 'app-auth-page',
standalone: true,
imports: [
CommonModule,
ReactiveFormsModule,
MatTabsModule,
MatCardModule,
MatFormFieldModule,
MatInputModule,
MatButtonModule,
MatProgressSpinnerModule
],
templateUrl: './auth-page.html',
styleUrl: './auth-page.scss'
})
export class AuthPage {

private fb = inject(FormBuilder);
private authService = inject(AuthService);
private router = inject(Router);
private notification =
  inject(NotificationService);

loading = signal(false);
errorMessage = signal('');

loginForm = this.fb.group({
email: ['', [Validators.required, Validators.email]],
password: ['', [Validators.required]]
});

registerForm = this.fb.group({
email: ['', [Validators.required, Validators.email]],
password: ['', [Validators.required, Validators.minLength(6)]]
});

login(): void {
if (this.loginForm.invalid) {
  return;
}

this.loading.set(true);
this.errorMessage.set('');

const request = this.loginForm.value as LoginRequest;

this.authService.login(request)
  .subscribe({
    next: () => {

      this.loading.set(false);
      this.notification.success(
        'Login successful'
      );
      this.router.navigate(['/dashboard']);
    },

    error: (error) => {

      this.loading.set(false);

      this.notification.error(
        error?.error?.message ??
        'Login failed'
      );
    }
  });


}

register(): void {
if (this.registerForm.invalid) {
  return;
}

this.loading.set(true);
this.errorMessage.set('');

const request = this.registerForm.value as RegisterRequest;

this.authService.register(request)
  .subscribe({
    next: () => {

      this.loading.set(false);

      this.notification.success(
        'Registration successful'
      );
    },

    error: (error) => {

      this.loading.set(false);

      this.notification.error(
        error?.error?.message ??
        'Registration failed'
      );
    }
  });

}
}
