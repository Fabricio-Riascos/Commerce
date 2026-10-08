-- =============================================
-- Tablas: commerce y commerce_quarantine
-- Script idempotente: se puede ejecutar varias veces.
-- =============================================
USE dbcommerce;
GO

DROP TABLE IF EXISTS dbo.commerce_quarantine;
DROP TABLE IF EXISTS dbo.commerce;
GO

CREATE TABLE dbo.commerce (
    id              INT IDENTITY(1,1) NOT NULL,
    pc_processdate  DATE              NOT NULL,
    pc_codcomercio  NVARCHAR(20)      NULL,
    pc_nomcomred    NVARCHAR(150)     NULL,
    pc_tipodoc      NVARCHAR(10)      NULL,
    pc_numdoc       VARCHAR(20)       NULL,
    pc_ciudad       NVARCHAR(100)     NULL,
    CONSTRAINT PK_commerce PRIMARY KEY (id)
);
GO

CREATE INDEX IX_commerce_processdate ON dbo.commerce (pc_processdate);
GO

CREATE TABLE dbo.commerce_quarantine (
    id                INT IDENTITY(1,1) NOT NULL,
    pc_processdate    DATE              NOT NULL,
    pc_codcomercio    NVARCHAR(20)      NULL,
    pc_nomcomred      NVARCHAR(150)     NULL,
    pc_tipodoc        NVARCHAR(10)      NULL,
    pc_numdoc         VARCHAR(20)       NULL,
    pc_ciudad         NVARCHAR(100)     NULL,
    motivo            NVARCHAR(500)     NOT NULL,
    fecha_cuarentena  DATETIME2         NOT NULL CONSTRAINT DF_commerce_quarantine_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT PK_commerce_quarantine PRIMARY KEY (id)
);
GO
