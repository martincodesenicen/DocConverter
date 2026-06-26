export type JobStatus =
  | 'Pendiente'
  | 'Procesando'
  | 'Completado'
  | 'Fallido';

export interface JobResponse {
  jobId: string;
  status: JobStatus;
  createdAt: string;
}