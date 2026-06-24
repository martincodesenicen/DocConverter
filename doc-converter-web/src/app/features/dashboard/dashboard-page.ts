import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';

@Component({
selector: 'app-dashboard-page',
standalone: true,
imports: [
CommonModule,
RouterLink,
MatCardModule,
NavbarComponent
],
templateUrl: './dashboard-page.html',
styleUrl: './dashboard-page.scss'
})
export class DashboardPage {}
