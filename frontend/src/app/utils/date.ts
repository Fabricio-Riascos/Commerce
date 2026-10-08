/**
 * Fecha de hoy en formato yyyy-MM-dd según la hora local.
 * No se usa toISOString() porque convierte a UTC y por la noche devolvería el día siguiente.
 */
export function todayAsIsoDate(): string {
  const now = new Date();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  return `${now.getFullYear()}-${month}-${day}`;
}
