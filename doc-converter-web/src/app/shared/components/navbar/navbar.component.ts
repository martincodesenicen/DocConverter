import { Component, inject } from '@angular/core';

import { Router } from '@angular/router';

import { AuthService }
from '../../../core/services/auth.service';

import { MatToolbarModule }
from '@angular/material/toolbar';

import { MatButtonModule }
from '@angular/material/button';

import { MatIconModule }
from '@angular/material/icon';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

  private auth =
    inject(AuthService);

  private router =
    inject(Router);

  logout() {

    this.auth.logout();
  }
}