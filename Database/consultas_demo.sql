/* =========================================================================
   CONSULTAS DE DEMOSTRACION - GymDB (v9)
   Sirven para mostrar en una entrevista que sabes SQL "de verdad":
   JOIN, LEFT JOIN, GROUP BY/HAVING, subconsultas, CTE, funciones de ventana,
   vistas y relaciones muchos-a-muchos. Ejecutalas de a una (selecciona y F5).
   ========================================================================= */
USE GymDB;
GO

/* RELACIONES (modelo)
   Perfiles 1---N Usuarios            Usuarios 1---N Pagos / Auditoria
   Perfiles N---N Permisos  (tabla puente PerfilPermisos)
   Miembros 1---N Inscripciones       Planes 1---N Inscripciones
   Entrenadores 1---N Inscripciones (opcional)
   Inscripciones 1---N DetalleInscripcion (ON DELETE CASCADE)
   Inscripciones 1---N Pagos (sin cascade: un pago no desaparece si se borra la inscripcion)  */

-- 1) INNER JOIN de 4 tablas: inscripciones con miembro, plan y entrenador (LEFT porque el entrenador es opcional)
SELECT i.Id, m.Nombre AS Miembro, p.Nombre AS Plan, ISNULL(e.Nombre, N'(sin entrenador)') AS Entrenador,
       i.FechaInscripcion, i.FechaVencimiento, i.Estado, i.Total
FROM dbo.Inscripciones i
INNER JOIN dbo.Miembros m ON m.Id = i.MiembroId
INNER JOIN dbo.Planes p ON p.Id = i.PlanId
LEFT JOIN dbo.Entrenadores e ON e.Id = i.EntrenadorId
ORDER BY i.FechaInscripcion DESC;

-- 2) LEFT JOIN + IS NULL: miembros que NUNCA se han inscrito (clientes para captar)
SELECT m.Id, m.Nombre, m.Correo
FROM dbo.Miembros m
LEFT JOIN dbo.Inscripciones i ON i.MiembroId = m.Id
WHERE i.Id IS NULL;

-- 3) GROUP BY + HAVING: miembros con mas de una inscripcion (clientes recurrentes) y cuanto han facturado
SELECT m.Nombre, COUNT(*) AS Inscripciones, SUM(i.Total) AS TotalFacturado
FROM dbo.Miembros m
INNER JOIN dbo.Inscripciones i ON i.MiembroId = m.Id
GROUP BY m.Id, m.Nombre
HAVING COUNT(*) > 1
ORDER BY TotalFacturado DESC;

-- 4) Subconsulta: inscripciones cuyo total supera el promedio general
SELECT i.Id, m.Nombre, i.Total
FROM dbo.Inscripciones i
INNER JOIN dbo.Miembros m ON m.Id = i.MiembroId
WHERE i.Total > (SELECT AVG(Total) FROM dbo.Inscripciones)
ORDER BY i.Total DESC;

-- 5) Vista de cartera (la logica "total - pagado" vive en SQL, no se repite en C#)
SELECT Miembro, PlanNombre, EstadoReal, Total, Pagado, Saldo
FROM dbo.vw_Membresia
WHERE Saldo > 0 AND EstadoReal <> 'Cancelada'
ORDER BY Saldo DESC;

-- 6) Membresias que vencen en los proximos 7 dias (para llamar al cliente)
SELECT Miembro, Documento, PlanNombre, FechaVencimiento, DiasParaVencer
FROM dbo.vw_Membresia
WHERE EstadoReal = 'Vigente' AND DiasParaVencer BETWEEN 0 AND 7
ORDER BY FechaVencimiento;

-- 7) CTE + funcion de ventana: ingresos cobrados por mes con ACUMULADO y variacion contra el mes anterior
WITH IngresosMes AS (
    SELECT DATEFROMPARTS(YEAR(FechaPago), MONTH(FechaPago), 1) AS Mes,
           SUM(Monto) AS Cobrado
    FROM dbo.Pagos
    WHERE Estado = 'Aplicado'
    GROUP BY DATEFROMPARTS(YEAR(FechaPago), MONTH(FechaPago), 1)
)
SELECT Mes,
       Cobrado,
       SUM(Cobrado) OVER (ORDER BY Mes ROWS UNBOUNDED PRECEDING) AS Acumulado,
       Cobrado - LAG(Cobrado) OVER (ORDER BY Mes) AS VariacionVsMesAnterior
FROM IngresosMes
ORDER BY Mes;

-- 8) RANK() por grupo: el plan que mas ingresos facturados genera dentro de cada tipo de duracion
SELECT TipoDuracion, Plan, Facturado,
       RANK() OVER (PARTITION BY TipoDuracion ORDER BY Facturado DESC) AS Posicion
FROM (
    SELECT p.TipoDuracion, p.Nombre AS Plan, SUM(i.Total) AS Facturado
    FROM dbo.Inscripciones i
    INNER JOIN dbo.Planes p ON p.Id = i.PlanId
    GROUP BY p.TipoDuracion, p.Nombre
) AS t
ORDER BY TipoDuracion, Posicion;

-- 9) Metodos de pago: participacion porcentual de cada uno sobre lo cobrado
SELECT MetodoPago,
       COUNT(*) AS NumPagos,
       SUM(Monto) AS Total,
       CAST(100.0 * SUM(Monto) / SUM(SUM(Monto)) OVER () AS DECIMAL(5,1)) AS Porcentaje
FROM dbo.Pagos
WHERE Estado = 'Aplicado'
GROUP BY MetodoPago
ORDER BY Total DESC;

-- 10) Relacion muchos-a-muchos: permisos que tiene cada perfil
SELECT pf.Nombre AS Perfil, pm.Modulo, pm.Codigo
FROM dbo.PerfilPermisos pp
INNER JOIN dbo.Perfiles pf ON pf.Id = pp.PerfilId
INNER JOIN dbo.Permisos pm ON pm.Id = pp.PermisoId
ORDER BY pf.Nombre, pm.Modulo, pm.Codigo;

-- 11) Auditoria: quien registro mas pagos y por cuanto dinero (control de caja por usuario)
SELECT ISNULL(u.NombreUsuario, N'(usuario eliminado)') AS Cajero,
       COUNT(*) AS Pagos, SUM(pg.Monto) AS Total
FROM dbo.Pagos pg
LEFT JOIN dbo.Usuarios u ON u.Id = pg.UsuarioId
WHERE pg.Estado = 'Aplicado'
GROUP BY u.NombreUsuario
ORDER BY Total DESC;

-- 12) Control de integridad: ¿hay inscripciones con mas pagado que su total? (debe devolver 0 filas)
SELECT InscripcionId, Total, Pagado
FROM dbo.vw_SaldoInscripcion
WHERE Pagado > Total;

-- 13) Para mostrar el efecto de los indices: activa estadisticas y compara
-- SET STATISTICS IO ON;
-- SELECT * FROM dbo.Pagos WHERE InscripcionId = 5;   -- usa ix_Pagos_Inscripcion (Index Seek)
-- SET STATISTICS IO OFF;
