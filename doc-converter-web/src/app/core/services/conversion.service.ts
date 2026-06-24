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
