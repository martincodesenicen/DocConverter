import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import {MatIconModule} from '@angular/material/icon';

@Component({
selector: 'app-dashboard-page',
standalone: true,
imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    NavbarComponent,
    MatIconModule
],
    templateUrl: './dashboard-page.html',
    styleUrl: './dashboard-page.scss'
})

export class DashboardPage {}
