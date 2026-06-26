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

import { JobStatusComponent } from '../../../shared/components/job-status/job-status.component';
import { FileDropzoneComponent } from '../../../shared/components/file-dropzone/file-dropzone.component';
import { ConversionLayoutComponent } from '../../../shared/components/conversion-layout/conversion-layout.component';

import {
  FileDownloadService
} from '../../../core/services/file-download.service';

import { NotificationService } from '../../../core/services/notification.service';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    FileDropzoneComponent,
    JobStatusComponent,
    ConversionLayoutComponent
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
    private fileDownloadService: FileDownloadService,
    private pollingService: PollingService,
    private notification: NotificationService
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
    const file =
      this.selectedFile();

    const {
      startPage,
      endPage
    } = this.form.getRawValue();

    if (!file) {
      this.notification.error(
        'Por favor, selecciona un archivo PDF.'
      );

      return;
    }

    if (
      !file.name
        .toLowerCase()
        .endsWith('.pdf')
    ) {
      this.notification.error(
        'Solo se permiten archivos PDF.'
      );

      return;
    }

    if (
      this.form.invalid
    ) {
      this.form.markAllAsTouched();

      this.notification.error(
        'Por favor, ingresa números de página válidos.'
      );

      return;
    }

    if (
      startPage! > endPage!
    ) {
      this.notification.error(
        'La página de fin debe ser mayor que la página de inicio.'
      );

      return;
    }

    this.loading.set(true);

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
        this.fileDownloadService
          .downloadBlob(
            blob,
            'split.pdf'
          );
      });
  }

  onFilesDropped(
    files: File[]
  ): void {
    if (!files.length) {
      return;
    }

    this.selectedFile.set(
      files[0]
    );
  }
}