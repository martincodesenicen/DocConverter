import {
  Component,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { ConversionService } from '../../../core/services/conversion.service';
import { PollingService } from '../../../core/services/polling.service';
import { JobStatusComponent }
from '../../../shared/components/job-status/job-status.component';

import { ConversionLayoutComponent }
from '../../../shared/components/conversion-layout/conversion-layout.component';

import { FileDropzoneComponent } from '../../../shared/components/file-dropzone/file-dropzone.component';

import {
  FileDownloadService
} from '../../../core/services/file-download.service';

import { NotificationService }
from '../../../core/services/notification.service';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    JobStatusComponent,
    ConversionLayoutComponent,
    FileDropzoneComponent
  ],
  templateUrl: './pdf-merge-page.html',
  styleUrl: './pdf-merge-page.scss'
})
export class PdfMergePage {

  files =
    signal<File[]>([]);

  jobStatus =
    signal('');

  jobId =
    signal('');

  loading =
    signal(false);

  constructor(
    private conversionService: ConversionService,
    private fileDownloadService: FileDownloadService,  
    private pollingService: PollingService,
    private notification: NotificationService
  ) {}

  onFilesSelected(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    if (!input.files?.length) {
      return;
    }

    this.files.set(
      Array.from(input.files)
    );
  }

  onFilesDropped(
  files: File[]
): void {

  this.files.set(files);
}

  merge(): void {

    if (
      this.files().length < 2
    ) {

      this.notification.error(
        'Selecciona al menos dos archivos PDF para combinar.'
      );

      return;
    }

    const invalidFile =
      this.files().find(
        file =>
          !file.name
            .toLowerCase()
            .endsWith('.pdf')
      );

    if (invalidFile) {

      this.notification.error(
        'Solo se permiten archivos PDF.'
      );

      return;
    }

    this.loading.set(true);

    this.conversionService
      .pdfMerge(
        this.files()
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
          response.status === 'Completado' ||
          response.status === 'Fallido'
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
            'merged.pdf'
          );
      });
  }
}