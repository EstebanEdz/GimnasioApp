# Changelog

Formato basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/).

## [9.0.0]

### Añadido
- Roles y permisos configurables (`Permisos`, `PerfilPermisos`) y pantalla de administración.
- Control de membresías: renovar, congelar/reactivar, cancelar y alertas de vencimiento.
- Control de pagos con abonos parciales, métodos de pago y anulación con motivo.
- Reportes financieros: cobrado vs facturado, cartera, morosos, ingresos por mes y método.
- Auditoría de acciones y log de errores con código de soporte.
- Bloqueo de cuenta por intentos fallidos y cambio de contraseña propio.
- Índices, vistas (`vw_SaldoInscripcion`, `vw_Membresia`) y consultas SQL de demostración.
- Variable de entorno `GYM_DB_CONNECTION` para la cadena de conexión.

### Cambiado
- Contraseñas con PBKDF2-SHA256 con salt; los hashes SHA256 anteriores se migran en el primer login.
- Política de contraseñas: mínimo 8 caracteres con letras y números.
- Los menús y módulos dependen de permisos y no del nombre del perfil.

### Corregido
- `database.sql` verificaba una tabla distinta a la que borraba en el `DROP`.
- Eliminada una clave de cifrado de ejemplo que estaba en `App.config` y nunca se usaba.
- La cadena de conexión ya no apunta a un nombre de equipo concreto.

## [8.0.0]
- Versión académica inicial (Proyecto Final Integrador, Corte 3): CRUD de miembros, planes, entrenadores e inscripciones; login con perfiles; reportes con gráficos y exportación.
