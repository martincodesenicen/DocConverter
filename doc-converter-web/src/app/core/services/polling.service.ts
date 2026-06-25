import { Injectable } from '@angular/core';

import {
  timer,
  switchMap,
  takeWhile
} from 'rxjs';

import { ConversionService } from './conversion.service';

@Injectable({
  providedIn: 'root'
})
export class PollingService {

  constructor(
    private conversionService: ConversionService
  ) {}

  pollJob(
    jobId: string
  ) {

    return timer(0, 3000).pipe(

      switchMap(() =>
        this.conversionService.getStatus(jobId)
      ),

      takeWhile(
        response =>
          response.status !== 'Completed' &&
          response.status !== 'Failed',
        true
      )
    );
  }
}