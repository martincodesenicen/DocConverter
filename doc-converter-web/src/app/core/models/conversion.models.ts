export type JobStatus =
  | 'Pending'
  | 'Processing'
  | 'Completed'
  | 'Failed';

export interface JobResponse {
  jobId: string;
  status: JobStatus;
  createdAt: string;
}