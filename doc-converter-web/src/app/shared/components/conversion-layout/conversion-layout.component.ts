import {
  Component,
  Input
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  MatCardModule
} from '@angular/material/card';

@Component({
  selector: 'app-conversion-layout',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule
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