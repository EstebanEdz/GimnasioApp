/* =========================================================================
   GimnasioApp v9 - MEJORAS (migracion NO destructiva)
   -------------------------------------------------------------------------
   * Si ya tienes GymDB con datos: ejecuta SOLO este script. Es idempotente
     (se puede correr varias veces) y no borra nada.
   * Si instalas desde cero: ejecuta primero database.sql y luego este.

   Que agrega:
     1. Usuarios: bloqueo por intentos fallidos, ultimo acceso, fecha de alta.
     2. Roles y permisos: tablas Permisos y PerfilPermisos (matriz editable).
     3. Membresias: estados Vigente/Vencida/Congelada/Cancelada con CHECK.
     4. Pagos: tabla Pagos con abonos parciales, metodos y anulacion.
     5. Auditoria y LogErrores: trazabilidad de acciones y errores.
     6. Indices, vistas para reportes y datos de prueba de pagos.

   Convencion de nombres (la misma del curso): pk_ / fk_ / ck_ / uq_ / df_
   ========================================================================= */

USE GymDB;
GO

/* ---------------------------------------------------------------------
   1. USUARIOS: seguridad de cuenta
   --------------------------------------------------------------------- */
IF COL_LENGTH('dbo.Usuarios', 'IntentosFallidos') IS NULL
    ALTER TABLE dbo.Usuarios ADD IntentosFallidos INT NOT NULL
        CONSTRAINT df_Usuarios_IntentosFallidos DEFAULT 0;
GO
IF COL_LENGTH('dbo.Usuarios', 'BloqueadoHasta') IS NULL
    ALTER TABLE dbo.Usuarios ADD BloqueadoHasta DATETIME2(0) NULL;
GO
IF COL_LENGTH('dbo.Usuarios', 'UltimoAcceso') IS NULL
    ALTER TABLE dbo.Usuarios ADD UltimoAcceso DATETIME2(0) NULL;
GO
IF COL_LENGTH('dbo.Usuarios', 'FechaCreacion') IS NULL
    ALTER TABLE dbo.Usuarios ADD FechaCreacion DATETIME2(0) NOT NULL
        CONSTRAINT df_Usuarios_FechaCreacion DEFAULT SYSDATETIME();
GO

/* ---------------------------------------------------------------------
   2. ROLES Y PERMISOS
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Permisos', 'U') IS NULL
CREATE TABLE dbo.Permisos (
    Id           INT IDENTITY(1,1) CONSTRAINT pk_Permisos PRIMARY KEY,
    Codigo       NVARCHAR(50)  NOT NULL CONSTRAINT uq_Permisos_Codigo UNIQUE,
    Descripcion  NVARCHAR(150) NOT NULL,
    Modulo       NVARCHAR(30)  NOT NULL
);
GO

IF OBJECT_ID('dbo.PerfilPermisos', 'U') IS NULL
CREATE TABLE dbo.PerfilPermisos (
    PerfilId   INT NOT NULL
        CONSTRAINT fk_PerfilPermisos_Perfiles REFERENCES dbo.Perfiles(Id) ON DELETE CASCADE,
    PermisoId  INT NOT NULL
        CONSTRAINT fk_PerfilPermisos_Permisos REFERENCES dbo.Permisos(Id) ON DELETE CASCADE,
    CONSTRAINT pk_PerfilPermisos PRIMARY KEY (PerfilId, PermisoId)
);
GO

INSERT INTO dbo.Permisos (Codigo, Descripcion, Modulo)
SELECT v.Codigo, v.Descripcion, v.Modulo
FROM (VALUES
    ('MIEMBROS_VER',          N'Ver miembros',                          N'Miembros'),
    ('MIEMBROS_EDITAR',       N'Crear, editar y eliminar miembros',     N'Miembros'),
    ('PLANES_VER',            N'Ver planes',                            N'Planes'),
    ('PLANES_EDITAR',         N'Crear, editar y eliminar planes',       N'Planes'),
    ('ENTRENADORES_VER',      N'Ver entrenadores',                      N'Entrenadores'),
    ('ENTRENADORES_EDITAR',   N'Crear, editar y eliminar entrenadores', N'Entrenadores'),
    ('INSCRIPCIONES_VER',     N'Ver inscripciones',                     N'Inscripciones'),
    ('INSCRIPCIONES_EDITAR',  N'Crear, editar y eliminar inscripciones',N'Inscripciones'),
    ('MEMBRESIAS_VER',        N'Ver el control de membresias',          N'Membresias'),
    ('MEMBRESIAS_GESTIONAR',  N'Renovar, congelar y cancelar membresias',N'Membresias'),
    ('PAGOS_VER',             N'Ver pagos y saldos',                    N'Pagos'),
    ('PAGOS_REGISTRAR',       N'Registrar pagos y abonos',              N'Pagos'),
    ('PAGOS_ANULAR',          N'Anular pagos',                          N'Pagos'),
    ('REPORTES_BASICOS',      N'Ver reportes basicos',                  N'Reportes'),
    ('REPORTES_FINANCIEROS',  N'Ver reportes financieros y de cartera', N'Reportes'),
    ('USUARIOS_ADMIN',        N'Administrar usuarios',                  N'Seguridad'),
    ('PERMISOS_ADMIN',        N'Editar permisos por perfil',            N'Seguridad'),
    ('AUDITORIA_VER',         N'Ver auditoria y log de errores',        N'Seguridad')
) AS v(Codigo, Descripcion, Modulo)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Permisos p WHERE p.Codigo = v.Codigo);
GO

/* Matriz inicial por perfil. Solo se carga si PerfilPermisos esta vacia, asi
   no pisa los cambios que el administrador haga luego desde la aplicacion. */
IF NOT EXISTS (SELECT 1 FROM dbo.PerfilPermisos)
BEGIN
    -- Administrador: todo
    INSERT INTO dbo.PerfilPermisos (PerfilId, PermisoId)
    SELECT pf.Id, pm.Id
    FROM dbo.Perfiles pf CROSS JOIN dbo.Permisos pm
    WHERE pf.Nombre = N'Administrador';

    -- Operador: operacion diaria, sin anular pagos, sin finanzas ni seguridad
    INSERT INTO dbo.PerfilPermisos (PerfilId, PermisoId)
    SELECT pf.Id, pm.Id
    FROM dbo.Perfiles pf
    INNER JOIN dbo.Permisos pm ON pm.Codigo IN (
        'MIEMBROS_VER','MIEMBROS_EDITAR','PLANES_VER','PLANES_EDITAR',
        'ENTRENADORES_VER','ENTRENADORES_EDITAR','INSCRIPCIONES_VER','INSCRIPCIONES_EDITAR',
        'MEMBRESIAS_VER','MEMBRESIAS_GESTIONAR','PAGOS_VER','PAGOS_REGISTRAR','REPORTES_BASICOS')
    WHERE pf.Nombre = N'Operador';

    -- Consulta: solo reportes basicos (igual que en la version anterior)
    INSERT INTO dbo.PerfilPermisos (PerfilId, PermisoId)
    SELECT pf.Id, pm.Id
    FROM dbo.Perfiles pf
    INNER JOIN dbo.Permisos pm ON pm.Codigo = 'REPORTES_BASICOS'
    WHERE pf.Nombre = N'Consulta';
END
GO

/* ---------------------------------------------------------------------
   3. MEMBRESIAS (tabla Inscripciones): estados y reglas de integridad
   --------------------------------------------------------------------- */
IF COL_LENGTH('dbo.Inscripciones', 'FechaCongelacion') IS NULL
    ALTER TABLE dbo.Inscripciones ADD FechaCongelacion DATE NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'ck_Inscripciones_Estado')
    ALTER TABLE dbo.Inscripciones WITH CHECK ADD CONSTRAINT ck_Inscripciones_Estado
        CHECK (Estado IN ('Vigente', 'Vencida', 'Congelada', 'Cancelada'));
GO
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'ck_Inscripciones_Fechas')
    ALTER TABLE dbo.Inscripciones WITH CHECK ADD CONSTRAINT ck_Inscripciones_Fechas
        CHECK (FechaVencimiento >= FechaInscripcion);
GO
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'ck_Inscripciones_Total')
    ALTER TABLE dbo.Inscripciones WITH CHECK ADD CONSTRAINT ck_Inscripciones_Total
        CHECK (Total >= 0);
GO

/* ---------------------------------------------------------------------
   4. PAGOS (abonos parciales, metodo, anulacion con motivo)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Pagos', 'U') IS NULL
CREATE TABLE dbo.Pagos (
    Id               INT IDENTITY(1,1) CONSTRAINT pk_Pagos PRIMARY KEY,
    InscripcionId    INT NOT NULL
        CONSTRAINT fk_Pagos_Inscripciones REFERENCES dbo.Inscripciones(Id),
    FechaPago        DATETIME2(0) NOT NULL CONSTRAINT df_Pagos_FechaPago DEFAULT SYSDATETIME(),
    Monto            DECIMAL(18,2) NOT NULL CONSTRAINT ck_Pagos_Monto CHECK (Monto > 0),
    MetodoPago       NVARCHAR(20) NOT NULL
        CONSTRAINT ck_Pagos_Metodo CHECK (MetodoPago IN ('Efectivo','Tarjeta','Transferencia','Nequi','Daviplata')),
    Referencia       NVARCHAR(50)  NULL,
    Estado           NVARCHAR(10)  NOT NULL CONSTRAINT df_Pagos_Estado DEFAULT 'Aplicado'
        CONSTRAINT ck_Pagos_Estado CHECK (Estado IN ('Aplicado','Anulado')),
    UsuarioId        INT NULL
        CONSTRAINT fk_Pagos_Usuarios REFERENCES dbo.Usuarios(Id) ON DELETE SET NULL,
    Observacion      NVARCHAR(200) NULL,
    FechaAnulacion   DATETIME2(0)  NULL,
    MotivoAnulacion  NVARCHAR(200) NULL,
    CONSTRAINT ck_Pagos_Anulacion CHECK (Estado = 'Aplicado' OR MotivoAnulacion IS NOT NULL)
);
GO

/* ---------------------------------------------------------------------
   5. AUDITORIA Y LOG DE ERRORES
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Auditoria', 'U') IS NULL
CREATE TABLE dbo.Auditoria (
    Id             BIGINT IDENTITY(1,1) CONSTRAINT pk_Auditoria PRIMARY KEY,
    Fecha          DATETIME2(0) NOT NULL CONSTRAINT df_Auditoria_Fecha DEFAULT SYSDATETIME(),
    UsuarioId      INT NULL
        CONSTRAINT fk_Auditoria_Usuarios REFERENCES dbo.Usuarios(Id) ON DELETE SET NULL,
    NombreUsuario  NVARCHAR(50)  NOT NULL,
    Accion         NVARCHAR(40)  NOT NULL,
    Entidad        NVARCHAR(40)  NOT NULL,
    EntidadId      NVARCHAR(30)  NULL,
    Detalle        NVARCHAR(500) NULL
);
GO

IF OBJECT_ID('dbo.LogErrores', 'U') IS NULL
CREATE TABLE dbo.LogErrores (
    Id             BIGINT IDENTITY(1,1) CONSTRAINT pk_LogErrores PRIMARY KEY,
    Codigo         NVARCHAR(12) NOT NULL,
    Fecha          DATETIME2(0) NOT NULL CONSTRAINT df_LogErrores_Fecha DEFAULT SYSDATETIME(),
    NombreUsuario  NVARCHAR(50)  NULL,
    Contexto       NVARCHAR(100) NULL,
    Mensaje        NVARCHAR(500) NOT NULL,
    StackTrace     NVARCHAR(MAX) NULL
);
GO

/* ---------------------------------------------------------------------
   6. INDICES (aceleran los JOIN, filtros y reportes mas usados)
   --------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'ix_Inscripciones_Miembro' AND object_id = OBJECT_ID('dbo.Inscripciones'))
    CREATE INDEX ix_Inscripciones_Miembro ON dbo.Inscripciones (MiembroId);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'ix_Inscripciones_Estado_Vencimiento' AND object_id = OBJECT_ID('dbo.Inscripciones'))
    CREATE INDEX ix_Inscripciones_Estado_Vencimiento ON dbo.Inscripciones (Estado, FechaVencimiento)
        INCLUDE (MiembroId, PlanId, Total);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'ix_Pagos_Inscripcion' AND object_id = OBJECT_ID('dbo.Pagos'))
    CREATE INDEX ix_Pagos_Inscripcion ON dbo.Pagos (InscripcionId) INCLUDE (Monto, Estado, FechaPago);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'ix_Pagos_FechaPago' AND object_id = OBJECT_ID('dbo.Pagos'))
    CREATE INDEX ix_Pagos_FechaPago ON dbo.Pagos (FechaPago) INCLUDE (Monto, MetodoPago, Estado);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'ix_Auditoria_Fecha' AND object_id = OBJECT_ID('dbo.Auditoria'))
    CREATE INDEX ix_Auditoria_Fecha ON dbo.Auditoria (Fecha DESC);
GO

/* ---------------------------------------------------------------------
   7. VISTAS (la logica de cartera vive en SQL y se reutiliza)
   --------------------------------------------------------------------- */

-- Total, pagado y saldo de cada inscripcion (solo cuentan pagos Aplicados)
CREATE OR ALTER VIEW dbo.vw_SaldoInscripcion
AS
SELECT i.Id AS InscripcionId,
       i.Total,
       ISNULL(SUM(CASE WHEN pg.Estado = 'Aplicado' THEN pg.Monto END), 0) AS Pagado,
       i.Total - ISNULL(SUM(CASE WHEN pg.Estado = 'Aplicado' THEN pg.Monto END), 0) AS Saldo
FROM dbo.Inscripciones i
LEFT JOIN dbo.Pagos pg ON pg.InscripcionId = i.Id
GROUP BY i.Id, i.Total;
GO

-- Cada membresia con su estado REAL (una Vigente cuya fecha ya paso cuenta como Vencida)
CREATE OR ALTER VIEW dbo.vw_Membresia
AS
SELECT i.Id AS InscripcionId,
       m.Id AS MiembroId,
       m.Nombre AS Miembro,
       m.Documento,
       p.Id AS PlanId,
       p.Nombre AS PlanNombre,
       ISNULL(e.Nombre, N'(Sin entrenador)') AS Entrenador,
       i.FechaInscripcion,
       i.FechaVencimiento,
       CASE WHEN i.Estado = 'Vigente' AND i.FechaVencimiento < CAST(GETDATE() AS DATE)
            THEN 'Vencida' ELSE i.Estado END AS EstadoReal,
       DATEDIFF(DAY, CAST(GETDATE() AS DATE), i.FechaVencimiento) AS DiasParaVencer,
       s.Total,
       s.Pagado,
       s.Saldo
FROM dbo.Inscripciones i
INNER JOIN dbo.Miembros m ON m.Id = i.MiembroId
INNER JOIN dbo.Planes p ON p.Id = i.PlanId
LEFT JOIN dbo.Entrenadores e ON e.Id = i.EntrenadorId
INNER JOIN dbo.vw_SaldoInscripcion s ON s.InscripcionId = i.Id;
GO

/* ---------------------------------------------------------------------
   8. DATOS DE PRUEBA DE PAGOS (solo si la tabla esta vacia)
      - Pagado completo: la mayoria
      - Abono parcial (50%): inscripciones cuyo Id es multiplo de 5
      - Sin pagos (moroso): inscripciones cuyo Id es multiplo de 7
   --------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Pagos)
BEGIN
    DECLARE @AdminId INT = (SELECT TOP 1 Id FROM dbo.Usuarios WHERE NombreUsuario = 'admin');

    INSERT INTO dbo.Pagos (InscripcionId, FechaPago, Monto, MetodoPago, Referencia, Estado, UsuarioId, Observacion)
    SELECT i.Id,
           CAST(i.FechaInscripcion AS DATETIME2(0)),
           CASE WHEN i.Id % 5 = 0 THEN i.Total / 2 ELSE i.Total END,
           CASE i.Id % 4 WHEN 0 THEN 'Efectivo' WHEN 1 THEN 'Tarjeta' WHEN 2 THEN 'Transferencia' ELSE 'Nequi' END,
           CASE WHEN i.Id % 4 = 0 THEN NULL ELSE CONCAT('REF-', i.Id) END,
           'Aplicado',
           @AdminId,
           N'Dato de prueba'
    FROM dbo.Inscripciones i
    WHERE i.Total > 0 AND i.Id % 7 <> 0;
END
GO

/* Marca como Vencida toda membresia Vigente cuya fecha ya paso */
UPDATE dbo.Inscripciones
SET Estado = 'Vencida'
WHERE Estado = 'Vigente' AND FechaVencimiento < CAST(GETDATE() AS DATE);
GO

PRINT 'GimnasioApp v9: mejoras aplicadas correctamente.';
GO
