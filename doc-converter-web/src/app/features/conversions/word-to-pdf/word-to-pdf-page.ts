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

@Component({
selector: 'app-word-to-pdf-page',
standalone: true,
imports: [
CommonModule,
MatButtonModule,
MatCardModule,
MatProgressSpinnerModule
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
private pollingService: PollingService
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

    const url =
      window.URL.createObjectURL(blob);

    const a =
      document.createElement('a');

    a.href = url;

    a.download = 'result.pdf';

    a.click();

    window.URL.revokeObjectURL(url);
  });


}
}
