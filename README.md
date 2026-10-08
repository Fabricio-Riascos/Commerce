# Carga y procesamiento de comercios

Aplicación web para cargar un archivo `commerce_DDMMYYYY.csv`, previsualizarlo, guardarlo en SQL Server,
procesarlo por fecha (`pc_processdate`) y consultar los registros con errores (cuarentena).

## Tecnologías

| Capa          | Tecnología                                          |
|---------------|-----------------------------------------------------|
| Base de datos | SQL Server 2022                                     |
| Backend       | .NET 10 Web API, Dapper, Microsoft.Data.SqlClient   |
| Frontend      | Angular 21 (standalone components, signals)         |

## Estructura

```
database/   Scripts SQL: tablas, tipo tabla y stored procedures
backend/    Solución .NET (Commerce.Api)
frontend/   Aplicación Angular (commerce-web)
```

## Requisitos

- SQL Server 2022 (o 2017+, por el uso de `CONCAT_WS`)
- .NET SDK 10
- Node.js 20.19+ y npm

## Puesta en marcha

### 1. Base de datos

Crear la base y ejecutar los scripts en orden. Ambos son idempotentes (se pueden volver a ejecutar).

```sql
CREATE DATABASE dbcommerce;
```

1. `database/01_tables.sql`: tablas `commerce` y `commerce_quarantine`.
2. `database/02_procedures.sql`: tipo `CommerceTableType`, `sp_create_commerce` y `sp_process_commerce`.

### 2. Backend

La cadena de conexión de `appsettings.json` es solo un ejemplo. Configure la real con user-secrets:

```bash
cd backend/Commerce.Api
dotnet user-secrets set "ConnectionStrings:DbCommerce" "Server=localhost,1433;Database=dbcommerce;User Id=<USUARIO>;Password=<PASSWORD>;TrustServerCertificate=True;"
dotnet run
```

La API queda en `http://localhost:5062`. El origen permitido por CORS se configura en `Cors:AllowedOrigins`.

### 3. Frontend

```bash
cd frontend
npm install
npm start
```

Abrir `http://localhost:4200`. La URL de la API se define en `src/environments/environment.ts`.

## Formato del archivo

- Nombre: `commerce_DDMMYYYY.csv` (por ejemplo `commerce_07102026.csv`).
- Separador `;`, primera línea con encabezado, fecha en formato `yyyy-MM-dd`.

```
pc_processdate;pc_codcomercio;pc_nomcomred;pc_tipodoc;pc_numdoc;pc_ciudad
2026-10-07;C001;Papeleria El Estudiante;CED;1001234567;Otavalo
2026-10-07;C006;;CED;1006789012;Ibarra
2026-10-07;C012;;RUC;ABCDEFGHIJ001;Ibarra
```

## API

| Método | Ruta                       | Descripción                                                                  |
|--------|----------------------------|------------------------------------------------------------------------------|
| POST   | `/api/commerce/upload`     | Recibe el CSV (`multipart/form-data`, campo `file`) y lo inserta.            |
| POST   | `/api/commerce/process`    | Body `{ "processDate": "2026-10-07" }`. Retorna la cantidad en cuarentena.   |
| GET    | `/api/commerce/quarantine` | Lista los registros en cuarentena con su motivo.                             |

Los errores de validación responden `400` y los errores no controlados `500`, ambos en formato `ProblemDetails`.

## Reglas de validación (`sp_process_commerce`)

- `pc_nomcomred` no puede estar vacío.
- `pc_numdoc` no puede estar vacío ni contener letras o caracteres especiales.
- Los registros inválidos se mueven de `commerce` a `commerce_quarantine` con su motivo.
  Si fallan varias reglas, los motivos se concatenan.
- Todo el proceso se ejecuta en una transacción.

## Decisiones de diseño

- **Validación de datos en la base:** la API solo rechaza lo que impide leer el archivo (nombre, archivo vacío,
  columnas o fecha ilegible). Los registros con datos inválidos se guardan y el SP los envía a cuarentena,
  para no perder información y dejar trazabilidad del motivo.
- **Table-valued parameter:** el archivo completo se envía al SP en una sola llamada, en lugar de un insert por fila.
- **`pc_numdoc` como `VARCHAR`:** es un identificador, no un número; así se pueden guardar los valores inválidos
  para detectarlos en el proceso y no se pierden ceros a la izquierda.
- **Arquitectura en capas:** Controller → Service → Repository, con interfaces e inyección de dependencias.
  DTOs separados de las entidades.
- **Secretos:** la cadena de conexión real se maneja con user-secrets y no se versiona.
- **Frontend:** un componente por pantalla, un servicio por recurso de la API y estado con signals.
  La previsualización se hace en el navegador; la validación definitiva siempre la hace el backend.

## Herramientas

Durante el desarrollo usé un asistente de IA (Claude) como apoyo en el desarrollo y la revisión de código.
