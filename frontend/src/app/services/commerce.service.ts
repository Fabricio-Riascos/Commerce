import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import {
  ProcessRequest,
  ProcessResult,
  QuarantineRecord,
  UploadResult,
} from '../models/commerce.models';

/** Cliente HTTP para el recurso /commerce de la API. */
@Injectable({ providedIn: 'root' })
export class CommerceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/commerce`;

  /** Envía el archivo CSV como multipart/form-data. */
  upload(file: File): Observable<UploadResult> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<UploadResult>(`${this.baseUrl}/upload`, formData);
  }

  /** Ejecuta el proceso de validación para una fecha (yyyy-MM-dd). */
  process(processDate: string): Observable<ProcessResult> {
    const body: ProcessRequest = { processDate };
    return this.http.post<ProcessResult>(`${this.baseUrl}/process`, body);
  }

  /** Obtiene los registros en cuarentena. */
  getQuarantine(): Observable<QuarantineRecord[]> {
    return this.http.get<QuarantineRecord[]>(`${this.baseUrl}/quarantine`);
  }
}
