/* =========================================================
   Proyecto Final Integrador - Gestion de Gimnasio (Grupo B)
   Script completo: CREATE DATABASE, tablas, PK/FK, datos de
   prueba y usuario administrador por defecto.
   ========================================================= */

IF DB_ID('GymDB') IS NULL
BEGIN
    CREATE DATABASE GymDB;
END
GO

USE GymDB;
GO

/* ---------- Perfiles ---------- */
-- (v9) Tablas nuevas primero, porque dependen de las anteriores por FK
IF OBJECT_ID('dbo.Pagos', 'U') IS NOT NULL DROP TABLE dbo.Pagos;
IF OBJECT_ID('dbo.Auditoria', 'U') IS NOT NULL DROP TABLE dbo.Auditoria;
IF OBJECT_ID('dbo.LogErrores', 'U') IS NOT NULL DROP TABLE dbo.LogErrores;
IF OBJECT_ID('dbo.PerfilPermisos', 'U') IS NOT NULL DROP TABLE dbo.PerfilPermisos;
IF OBJECT_ID('dbo.Permisos', 'U') IS NOT NULL DROP TABLE dbo.Permisos;
IF OBJECT_ID('dbo.vw_Membresia', 'V') IS NOT NULL DROP VIEW dbo.vw_Membresia;
IF OBJECT_ID('dbo.vw_SaldoInscripcion', 'V') IS NOT NULL DROP VIEW dbo.vw_SaldoInscripcion;
-- (corregido) antes este IF preguntaba por Perfiles pero borraba DetalleInscripcion
IF OBJECT_ID('dbo.DetalleInscripcion', 'U') IS NOT NULL DROP TABLE dbo.DetalleInscripcion;
IF OBJECT_ID('dbo.Inscripciones', 'U') IS NOT NULL DROP TABLE dbo.Inscripciones;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID('dbo.Entrenadores', 'U') IS NOT NULL DROP TABLE dbo.Entrenadores;
IF OBJECT_ID('dbo.Miembros', 'U') IS NOT NULL DROP TABLE dbo.Miembros;
IF OBJECT_ID('dbo.Planes', 'U') IS NOT NULL DROP TABLE dbo.Planes;
IF OBJECT_ID('dbo.Perfiles', 'U') IS NOT NULL DROP TABLE dbo.Perfiles;
GO

CREATE TABLE dbo.Perfiles (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre  NVARCHAR(30) NOT NULL UNIQUE
);
GO

INSERT INTO dbo.Perfiles (Nombre) VALUES
('Administrador'), ('Operador'), ('Consulta');
GO

/* ---------- Usuarios ---------- */
CREATE TABLE dbo.Usuarios (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario           NVARCHAR(50) NOT NULL UNIQUE,
    ContrasenaHash          NVARCHAR(200) NOT NULL,   -- hash (nunca texto plano); ver SeguridadHelper
    PerfilId                INT NOT NULL FOREIGN KEY REFERENCES dbo.Perfiles(Id),
    Activo                  BIT NOT NULL DEFAULT 1,
    PreguntaSeguridad       NVARCHAR(200) NULL,       -- para recuperar la contraseña
    RespuestaSeguridadHash  NVARCHAR(200) NULL        -- hash de la respuesta (normalizada), nunca texto plano
);
GO

-- USUARIOS DE DEMOSTRACION (cambia estas contrasenas si despliegas fuera de un entorno de pruebas):
-- admin / admin123   |  operador1 / oper123  |  consulta1 / consulta123
-- (hashes SHA256 heredados: la app los convierte a PBKDF2 en el primer inicio de sesion)
-- Respuestas de seguridad de prueba: admin -> "firulais", operador1 -> "azul", consulta1 -> "bogota"
INSERT INTO dbo.Usuarios (NombreUsuario, ContrasenaHash, PerfilId, Activo, PreguntaSeguridad, RespuestaSeguridadHash) VALUES
('admin',      '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, 1,
    '¿Cual es el nombre de tu primera mascota?', '9352d5d0cbdcc31f253ff97fed2aa73bb62d04319b0ea27ff71af29026713124'),
('operador1',  '13a93f9a5502e71eed80a806f87bc4424e03e6b0d90acd513ea9d561c853b738', 2, 1,
    '¿Cual es tu color favorito?', 'a4bd1d3a69aa0ea6ffb1298c8c26be4b333526cae7d27f2362f89857157701ce'),
('consulta1',  '7fa95c704c2defa7b1295d28bcddfd752bf9b594db18886dd64721183cfc47a5', 3, 1,
    '¿En que ciudad naciste?', '4f18b075b430429762f19d880df444390a1168662086c74d825bf7f2240a58fc');
GO

/* ---------- Planes ---------- */
CREATE TABLE dbo.Planes (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(50) NOT NULL UNIQUE,
    TipoDuracion    NVARCHAR(20) NOT NULL,   -- Mensual / Trimestral / Anual
    Precio          DECIMAL(18,2) NOT NULL CHECK (Precio >= 0),
    IncluyeClases   BIT NOT NULL DEFAULT 0,
    Activo          BIT NOT NULL DEFAULT 1
);
GO

INSERT INTO dbo.Planes (Nombre, TipoDuracion, Precio, IncluyeClases, Activo) VALUES
('Gold Mensual',      'Mensual',    90000,  0, 1),
('Gold Trimestral',   'Trimestral', 250000, 0, 1),
('Premium Mensual',   'Mensual',    130000, 1, 1),
('Premium Trimestral','Trimestral', 360000, 1, 1),
('Black Mensual',     'Mensual',    180000, 1, 1),
('Black Anual',       'Anual',     1800000, 1, 1),
('Estudiante',        'Mensual',    70000,  0, 1),
('Elite Personal',    'Mensual',    250000, 1, 1),
('Fin de Semana',     'Mensual',    60000,  0, 1),
('Corporativo',       'Trimestral', 300000, 1, 1);
GO

/* ---------- Miembros ---------- */
CREATE TABLE dbo.Miembros (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(100) NOT NULL,
    Documento     NVARCHAR(20) NOT NULL UNIQUE,
    Telefono      NVARCHAR(20) NULL,
    Correo        NVARCHAR(100) NULL,
    FechaIngreso  DATE NOT NULL,
    Activo        BIT NOT NULL DEFAULT 1
);
GO

INSERT INTO dbo.Miembros (Nombre, Documento, Telefono, Correo, FechaIngreso, Activo) VALUES
('Ana Garcia',      '1001', '3210000001', 'ana1@example.com',      '2024-01-15', 1),
('Luis Perez',      '1002', '3210000002', 'luis2@example.com',     '2024-02-10', 1),
('Maria Lopez',     '1003', '3210000003', 'maria3@example.com',    '2024-03-05', 1),
('Carlos Diaz',     '1004', '3210000004', 'carlos4@example.com',   '2024-01-20', 1),
('Sofia Torres',    '1005', '3210000005', 'sofia5@example.com',    '2024-04-12', 1),
('Daniel Rios',     '1006', '3210000006', 'daniel6@example.com',   '2024-04-18', 1),
('Valentina Cruz',  '1007', '3210000007', 'valentina7@example.com','2024-05-01', 1),
('Andres Moreno',   '1008', '3210000008', 'andres8@example.com',   '2024-05-20', 1),
('Camila Ruiz',     '1009', '3210000009', 'camila9@example.com',   '2024-06-08', 1),
('Mateo Silva',     '1010', '3210000010', 'mateo10@example.com',   '2024-06-25', 1),
('Juliana Gomez',   '1011', '3210000011', 'juliana11@example.com', '2024-07-02', 1),
('Felipe Marin',    '1012', '3210000012', 'felipe12@example.com', '2024-07-19', 1);
GO

/* ---------- Entrenadores ---------- */
CREATE TABLE dbo.Entrenadores (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(100) NOT NULL,
    Documento     NVARCHAR(20) NOT NULL UNIQUE,
    Telefono      NVARCHAR(20) NULL,
    Especialidad  NVARCHAR(50) NOT NULL,
    TarifaHora    DECIMAL(18,2) NOT NULL DEFAULT 0,
    Activo        BIT NOT NULL DEFAULT 1
);
GO

INSERT INTO dbo.Entrenadores (Nombre, Documento, Telefono, Especialidad, TarifaHora, Activo) VALUES
('Jorge Ramirez',   '2001', '3110000001', 'Crossfit',   35000, 1),
('Paula Ospina',    '2002', '3110000002', 'Yoga',       28000, 1),
('Felipe Castro',   '2003', '3110000003', 'Spinning',   30000, 1),
('Natalia Vargas',  '2004', '3110000004', 'BodyPump',   32000, 1),
('Ricardo Nino',    '2005', '3110000005', 'Personal',   45000, 1),
('Laura Beltran',   '2006', '3110000006', 'Nutricion',  40000, 1),
('Simon Herrera',   '2007', '3110000007', 'Crossfit',   36000, 1),
('Diana Suarez',    '2008', '3110000008', 'Yoga',       27000, 1),
('Oscar Pena',      '2009', '3110000009', 'Spinning',   29000, 1),
('Camilo Rojas',    '2010', '3110000010', 'Personal',   47000, 1);
GO

/* ---------- Inscripciones (cabecera) ---------- */
CREATE TABLE dbo.Inscripciones (
    Id                INT IDENTITY(1,1) PRIMARY KEY,
    MiembroId         INT NOT NULL FOREIGN KEY REFERENCES dbo.Miembros(Id),
    PlanId            INT NOT NULL FOREIGN KEY REFERENCES dbo.Planes(Id),
    EntrenadorId      INT NULL     FOREIGN KEY REFERENCES dbo.Entrenadores(Id),
    FechaInscripcion  DATE NOT NULL,
    FechaVencimiento  DATE NOT NULL,
    Estado            NVARCHAR(20) NOT NULL DEFAULT 'Vigente',
    Total             DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

/* ---------- Detalle de Inscripcion ---------- */
CREATE TABLE dbo.DetalleInscripcion (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    InscripcionId    INT NOT NULL FOREIGN KEY REFERENCES dbo.Inscripciones(Id) ON DELETE CASCADE,
    Concepto         NVARCHAR(100) NOT NULL,
    Cantidad         INT NOT NULL DEFAULT 1 CHECK (Cantidad > 0),
    PrecioUnitario   DECIMAL(18,2) NOT NULL CHECK (PrecioUnitario >= 0),
    Subtotal         AS (Cantidad * PrecioUnitario) PERSISTED
);
GO

-- 10 inscripciones de prueba con su detalle, repartidas en varios meses (para el reporte "por mes")
INSERT INTO dbo.Inscripciones (MiembroId, PlanId, EntrenadorId, FechaInscripcion, FechaVencimiento, Estado, Total) VALUES
(1, 1, 1,  '2026-01-05', '2026-02-05', 'Vigente', 90000),
(2, 3, 5,  '2026-01-18', '2026-02-18', 'Vigente', 175000),
(3, 1, NULL,'2026-02-02', '2026-03-02', 'Vencida', 90000),
(4, 5, 7,  '2026-02-14', '2026-03-14', 'Vigente', 216000),
(5, 3, 3,  '2026-03-01', '2026-03-31', 'Vigente', 145000),
(6, 7, NULL,'2026-03-20', '2026-04-20', 'Vigente', 70000),
(7, 6, 1,  '2026-04-03', '2027-04-03', 'Vigente', 1800000),
(8, 2, NULL,'2026-04-22', '2026-07-22', 'Vigente', 250000),
(9, 8, 5,  '2026-05-10', '2026-06-10', 'Vigente', 295000),
(10,9, NULL,'2026-05-28', '2026-06-28', 'Vigente', 60000);
GO

INSERT INTO dbo.DetalleInscripcion (InscripcionId, Concepto, Cantidad, PrecioUnitario) VALUES
(1, 'Plan Gold Mensual', 1, 90000),
(2, 'Plan Premium Mensual', 1, 130000),
(2, 'Sesion personal (adicional)', 1, 45000),
(3, 'Plan Gold Mensual', 1, 90000),
(4, 'Plan Black Mensual', 1, 180000),
(4, 'Sesion personal (adicional)', 1, 36000),
(5, 'Plan Premium Mensual', 1, 130000),
(5, 'Sesion personal (adicional)', 1, 15000),
(6, 'Plan Estudiante', 1, 70000),
(7, 'Plan Black Anual', 1, 1800000),
(8, 'Plan Gold Trimestral', 1, 250000),
(9, 'Plan Elite Personal', 1, 250000),
(9, 'Sesion personal (adicional)', 1, 45000),
(10,'Plan Fin de Semana', 1, 60000);
GO

PRINT 'Base de datos GymDB (base v8) creada. Ahora ejecuta Database/mejoras_v9.sql';
GO
