-- =============================================
-- Tipo tabla y stored procedures
-- Requiere: 01_tables.sql
-- Script idempotente: se puede ejecutar varias veces.
-- =============================================
USE dbcommerce;
GO

-- Los SP dependen del tipo, por eso se eliminan primero.
DROP PROCEDURE IF EXISTS dbo.sp_create_commerce;
DROP PROCEDURE IF EXISTS dbo.sp_process_commerce;
DROP TYPE IF EXISTS dbo.CommerceTableType;
GO

-- Table-valued parameter: permite enviar todas las filas del CSV en una sola llamada.
CREATE TYPE dbo.CommerceTableType AS TABLE (
    pc_processdate  DATE           NOT NULL,
    pc_codcomercio  NVARCHAR(20)   NULL,
    pc_nomcomred    NVARCHAR(150)  NULL,
    pc_tipodoc      NVARCHAR(10)   NULL,
    pc_numdoc       VARCHAR(20)    NULL,
    pc_ciudad       NVARCHAR(100)  NULL
);
GO

-- =============================================
-- sp_create_commerce
-- Inserta los registros del archivo en un solo INSERT.
-- Retorna la cantidad de registros insertados.
-- =============================================
CREATE PROCEDURE dbo.sp_create_commerce
    @rows dbo.CommerceTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.commerce (pc_processdate, pc_codcomercio, pc_nomcomred, pc_tipodoc, pc_numdoc, pc_ciudad)
    SELECT pc_processdate, pc_codcomercio, pc_nomcomred, pc_tipodoc, pc_numdoc, pc_ciudad
    FROM @rows;

    SELECT @@ROWCOUNT AS inserted_count;
END;
GO

-- =============================================
-- sp_process_commerce
-- Valida los registros de la fecha indicada y mueve los inválidos
-- a commerce_quarantine con su motivo (motivos concatenados).
-- Retorna la cantidad de registros enviados a cuarentena.
-- =============================================
CREATE PROCEDURE dbo.sp_process_commerce
    @processdate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;  -- ante cualquier error se revierte toda la transacción

    DECLARE @quarantined INT = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Calcular el motivo de cada registro inválido.
        --    CONCAT_WS ignora los NULL, así solo se concatenan las reglas que fallan.
        SELECT id, motivo
        INTO #invalid
        FROM (
            SELECT
                id,
                CONCAT_WS(N' | ',
                    CASE WHEN NULLIF(LTRIM(RTRIM(pc_nomcomred)), N'') IS NULL
                         THEN N'pc_nomcomred está vacío' END,
                    CASE WHEN NULLIF(LTRIM(RTRIM(pc_numdoc)), '') IS NULL
                         THEN N'pc_numdoc está vacío'
                         WHEN pc_numdoc LIKE '%[^0-9]%'
                         THEN N'pc_numdoc contiene letras o caracteres especiales' END
                ) AS motivo
            FROM dbo.commerce
            WHERE pc_processdate = @processdate
        ) AS v
        WHERE v.motivo <> N'';

        -- 2. Copiar a cuarentena.
        INSERT INTO dbo.commerce_quarantine (pc_processdate, pc_codcomercio, pc_nomcomred, pc_tipodoc, pc_numdoc, pc_ciudad, motivo)
        SELECT c.pc_processdate, c.pc_codcomercio, c.pc_nomcomred, c.pc_tipodoc, c.pc_numdoc, c.pc_ciudad, i.motivo
        FROM dbo.commerce AS c
        INNER JOIN #invalid AS i ON i.id = c.id;

        SET @quarantined = @@ROWCOUNT;

        -- 3. Eliminar de commerce.
        DELETE c
        FROM dbo.commerce AS c
        INNER JOIN #invalid AS i ON i.id = c.id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT @quarantined AS quarantined_count;
END;
GO
