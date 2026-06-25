import {
  Component,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  MatButtonModule
} from '@angular/material/button';

import {
  MatCardModule
} from '@angular/material/card';

import {
  MatProgressSpinnerModule
} from '@angular/material/progress-spinner';

import {
  ConversionService
} from '../../../core/services/conversion.service';

import {
  PollingService
} from '../../../core/services/polling.service';

import { FileDropzoneComponent } from '../../../shared/components/file-dropzone/file-dropzone.component';

import { JobStatusComponent } from '../../../shared/components/job-status/job-status.component';

import { ConversionLayoutComponent } from '../../../shared/components/conversion-layout/conversion-layout.component';

import {
  FileDownloadService
} from '../../../core/services/file-download.service';

import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-word-to-pdf-page',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    FileDropzoneComponent,
    JobStatusComponent,
    ConversionLayoutComponent
  ],
  templateUrl: './word-to-pdf-page.html',
  styleUrl: './word-to-pdf-page.scss'
})
export class WordToPdfPage {
  selectedFile =
    signal<File | null>(null);

  jobStatus =
    signal<string>('');

  jobId =
    signal<string>('');

  loading =
    signal(false);

  constructor(
    private conversionService: ConversionService,
    private pollingService: PollingService,
    private fileDownloadService: FileDownloadService,
    private notification: NotificationService
  ) {}

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

  convert(): void {
    const file =
      this.selectedFile();

    if (!file) {
      this.notification.error(
        'Please select a Word file.'
      );

      return;
    }

    const extension =
      file.name
        .toLowerCase();

    if (
      !extension.endsWith('.doc') &&
      !extension.endsWith('.docx')
    ) {
      this.notification.error(
        'Only .doc and .docx files are allowed.'
      );

      return;
    }

    this.loading.set(true);

    this.conversionService
      .wordToPdf(file)
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
            'result.pdf'
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