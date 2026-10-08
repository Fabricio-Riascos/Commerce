import { CommerceRow } from '../models/commerce.models';

const SEPARATOR = ';';
const EXPECTED_COLUMNS = 6;
const FILE_NAME_PATTERN = /^commerce_\d{8}\.csv$/i;

/** Resultado de leer el CSV para la previsualización. */
export interface CsvParseResult {
  rows: CommerceRow[];
  errors: string[];
}

/** Indica si el nombre cumple el formato commerce_DDMMYYYY.csv. */
export function isValidCommerceFileName(fileName: string): boolean {
  return FILE_NAME_PATTERN.test(fileName);
}

/**
 * Convierte el contenido del CSV (separado por ';', con encabezado) en filas.
 * Solo sirve para previsualizar: la validación definitiva la hace el backend.
 */
export function parseCommerceCsv(content: string): CsvParseResult {
  const rows: CommerceRow[] = [];
  const errors: string[] = [];

  const lines = content.split(/\r?\n/);

  lines.slice(1).forEach((line, index) => {
    if (!line.trim()) {
      return;
    }

    const columns = line.split(SEPARATOR).map((c) => c.trim());
    const lineNumber = index + 2;

    if (columns.length !== EXPECTED_COLUMNS) {
      errors.push(`Línea ${lineNumber}: se esperaban ${EXPECTED_COLUMNS} columnas y hay ${columns.length}.`);
      return;
    }

    const [processDate, commerceCode, commerceName, documentType, documentNumber, city] = columns;
    rows.push({ processDate, commerceCode, commerceName, documentType, documentNumber, city });
  });

  return { rows, errors };
}
