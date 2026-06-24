import {
  Component,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  ReactiveFormsModule,
  FormBuilder,
  Validators
} from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { ConversionService } from '../../../core/services/conversion.service';
import { PollingService } from '../../../core/services/polling.service';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './pdf-split-page.html',
  styleUrl: './pdf-split-page.scss'
})
export class PdfSplitPage {

  selectedFile =
    signal<File | null>(null);

  jobStatus =
    signal('');

  jobId =
    signal('');

  loading =
    signal(false);

  form;

  constructor(
    private fb: FormBuilder,
    private conversionService: ConversionService,
    private pollingService: PollingService
  ) {

    this.form = this.fb.group({
      startPage: [
        1,
        Validators.required
      ],

      endPage: [
        1,
        Validators.required
      ]
    });
  }

  onFileSelected(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    if (!input.files?.length) {
      return;
    }

    this.selectedFile.set(
      input.files[0]
    );
  }

  split(): void {

    if (
      !this.selectedFile() ||
      this.form.invalid
    ) {
      return;
    }

    this.loading.set(true);

    const {
      startPage,
      endPage
    } = this.form.getRawValue();

    this.conversionService
      .pdfSplit(
        this.selectedFile()!,
        startPage!,
        endPage!
      )
      .subscribe({

        next: response => {

          this.jobId.set(
            response.jobId
          );

          this.startPolling(
            response.jobId
          );
        },

        error: () => {

          this.loading.set(false);
        }
      });
  }

  private startPolling(
    jobId: string
  ): void {

    this.pollingService
      .pollJob(jobId)
      .subscribe(response => {

        this.jobStatus.set(
          response.status
        );

        if (
          response.status === 'Completed' ||
          response.status === 'Failed'
        ) {
          this.loading.set(false);
        }
      });
  }

  download(): void {

    this.conversionService
      .download(this.jobId())
      .subscribe(blob => {

        const url =
          URL.createObjectURL(blob);

        const a =
          document.createElement('a');

        a.href = url;
        a.download = 'split.pdf';

        a.click();

        URL.revokeObjectURL(url);
      });
  }
}