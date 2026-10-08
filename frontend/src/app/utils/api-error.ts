import { HttpErrorResponse } from '@angular/common/http';

import { ApiProblem } from '../models/commerce.models';

/** Obtiene un mensaje legible a partir de un error de la API. */
export function getApiErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'No se pudo conectar con el servidor. Verifique que la API esté en ejecución.';
    }
    const problem = error.error as ApiProblem | null;
    return problem?.detail ?? problem?.title ?? `Error ${error.status} al comunicarse con la API.`;
  }
  return 'Ocurrió un error inesperado.';
}
