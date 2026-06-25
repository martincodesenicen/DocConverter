import {
  Component,
  Input
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  MatCardModule
} from '@angular/material/card';

import { RouterLink } from '@angular/router';

import {
  MatButtonModule
} from '@angular/material/button';

import {
  MatIconModule
} from '@angular/material/icon';

@Component({
  selector: 'app-conversion-layout',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    RouterLink,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './conversion-layout.component.html',
  styleUrl: './conversion-layout.component.scss'
})
export class ConversionLayoutComponent {

  @Input()
  title = '';

  @Input()
  description = '';
}