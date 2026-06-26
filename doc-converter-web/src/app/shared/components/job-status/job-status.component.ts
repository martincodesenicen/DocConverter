import {
  Component,
  Input,
  Output,
  EventEmitter
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatProgressSpinnerModule }
from '@angular/material/progress-spinner';

import { MatButtonModule }
from '@angular/material/button';

import { MatIconModule }
from '@angular/material/icon';

@Component({
  selector: 'app-job-status',
  standalone: true,
  imports: [
    CommonModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './job-status.component.html',
  styleUrl: './job-status.component.scss'
})
export class JobStatusComponent {

  @Input()
  status = '';

  @Input()
  loading = false;

  @Output()
  downloadClicked =
    new EventEmitter<void>();

  download() {

    this.downloadClicked.emit();
  }
}