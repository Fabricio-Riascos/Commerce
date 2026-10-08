import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import {
  CommerceRecord,
  PagedResult,
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

  /** Obtiene una página de la tabla commerce, opcionalmente filtrada por fecha. */
  getCommerce(page: number, pageSize: number, processDate?: string): Observable<PagedResult<CommerceRecord>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (processDate) {
      params = params.set('processDate', processDate);
    }
    return this.http.get<PagedResult<CommerceRecord>>(this.baseUrl, { params });
  }

  /** Obtiene una página de los registros en cuarentena. */
  getQuarantine(page: number, pageSize: number): Observable<PagedResult<QuarantineRecord>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<QuarantineRecord>>(`${this.baseUrl}/quarantine`, { params });
  }
}
