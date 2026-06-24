import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../../../core/services/auth.service';

@Component({
selector: 'app-navbar',
standalone: true,
imports: [
MatToolbarModule,
MatButtonModule
],
template: ` <mat-toolbar>


  <span>Doc Converter</span>

  <span class="spacer"></span>

  <button
    mat-button
    (click)="logout()">

    Logout

  </button>

</mat-toolbar>


`,
  styles: [`
.spacer {
flex: 1;
}
`]
})
export class NavbarComponent {

private authService = inject(AuthService);
private router = inject(Router);

logout(): void {


this.authService.logout();

this.router.navigate(['/auth']);


}
}
