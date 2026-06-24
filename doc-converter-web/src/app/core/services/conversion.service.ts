import { Injectable } from '@angular/core';

import {
HttpClient
} from '@angular/common/http';

import {
Observable
} from 'rxjs';

import { environment } from '../../../environments/environment';

import {
JobResponse
} from '../models/conversion.models';

@Injectable({
providedIn: 'root'
})
export class ConversionService {

private readonly apiUrl =
`${environment.apiUrl}/conversions`;

constructor(
private http: HttpClient
) {}

wordToPdf(
file: File
): Observable<JobResponse> {


const formData = new FormData();

formData.append(
  'file',
  file
);

return this.http.post<JobResponse>(
  `${this.apiUrl}/word-to-pdf`,
  formData
);


}

pdfMerge(
  files: File[]
): Observable<JobResponse> {

  const formData = new FormData();

  files.forEach(file => {
    formData.append(
      'files',
      file
    );
  });

  return this.http.post<JobResponse>(
    `${this.apiUrl}/pdf-merge`,
    formData
  );
}

pdfSplit(
  file: File,
  startPage: number,
  endPage: number
): Observable<JobResponse> {

  const formData = new FormData();

  formData.append(
    'file',
    file
  );

  formData.append(
    'startPage',
    startPage.toString()
  );

  formData.append(
    'endPage',
    endPage.toString()
  );

  return this.http.post<JobResponse>(
    `${this.apiUrl}/pdf-split`,
    formData
  );
}

getStatus(
jobId: string
): Observable<JobResponse> {


return this.http.get<JobResponse>(
  `${this.apiUrl}/status/${jobId}`
);


}

download(
jobId: string
): Observable<Blob> {


return this.http.get(
  `${this.apiUrl}/download/${jobId}`,
  {
    responseType: 'blob'
  }
);


}
}
