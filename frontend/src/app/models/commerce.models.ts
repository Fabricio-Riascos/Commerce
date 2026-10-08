/** Fila del CSV tal como se muestra en la previsualización. */
export interface CommerceRow {
  processDate: string;
  commerceCode: string;
  commerceName: string;
  documentType: string;
  documentNumber: string;
  city: string;
}

/** Respuesta de POST /commerce/upload. */
export interface UploadResult {
  fileName: string;
  insertedCount: number;
}

/** Cuerpo de POST /commerce/process. */
export interface ProcessRequest {
  processDate: string;
}

/** Respuesta de POST /commerce/process. */
export interface ProcessResult {
  processDate: string;
  processedCount: number;
  quarantinedCount: number;
}

/** Registro de GET /commerce/quarantine. */
export interface QuarantineRecord {
  id: number;
  processDate: string;
  commerceCode: string | null;
  commerceName: string | null;
  documentType: string | null;
  documentNumber: string | null;
  city: string | null;
  reason: string;
  quarantinedAt: string;
}

/** Error estándar (ProblemDetails) que devuelve la API. */
export interface ApiProblem {
  title?: string;
  detail?: string;
  status?: number;
}
