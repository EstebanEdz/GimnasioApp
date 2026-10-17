using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public class MiembroService
{
    private readonly MiembroDAL _dal = new();
    private readonly PlanDAL _planDal = new();
    private readonly AuditoriaService _auditoria = new();

    /// <summary>Colección principal de trabajo en memoria (requisito List&lt;T&gt;).</summary>
    public List<Miembro> ObtenerTodos()
    {
        var precioUltimoPlan = ObtenerPrecioUltimoPlanPorMiembro();
        var lista = new List<Miembro>();
        foreach (var r in _dal.ObtenerTodos())
        {
            var m = new Miembro(r.Nombre, r.Documento, r.Telefono, r.Correo, r.FechaIngreso)
            {
                Id = r.Id,
                Activo = r.Activo,
                PrecioPlanVigente = precioUltimoPlan.TryGetValue(r.Id, out var p) ? p : 0m
            };
            lista.Add(m);
        }
        return lista;
    }

    public List<Miembro> Buscar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return ObtenerTodos();
        var lista = new List<Miembro>();
        foreach (var r in _dal.Buscar(texto))
        {
            lista.Add(new Miembro(r.Nombre, r.Documento, r.Telefono, r.Correo, r.FechaIngreso)
            {
                Id = r.Id,
                Activo = r.Activo
            });
        }
        return lista;
    }

    public Miembro? ObtenerPorId(int id)
    {
        var r = _dal.ObtenerPorId(id);
        if (r == null) return null;
        return new Miembro(r.Nombre, r.Documento, r.Telefono, r.Correo, r.FechaIngreso)
        {
            Id = r.Id,
            Activo = r.Activo
        };
    }

    /// <summary>Valida reglas de negocio; devuelve la lista de errores (vacia si es valido).</summary>
    public List<string> Validar(Miembro m)
    {
        var errores = new List<string>();

        string? errorNombre = Validador.Nombre(m.Nombre);
        if (errorNombre != null) errores.Add(errorNombre);
        else if (_dal.ExisteNombre(m.Nombre, m.Id))
            errores.Add("Ya existe un miembro registrado con ese mismo nombre.");

        string? errorDocumento = Validador.Documento(m.Documento);
        if (errorDocumento != null) errores.Add(errorDocumento);
        else if (_dal.ExisteDocumento(m.Documento, m.Id))
            errores.Add("Ya existe un miembro con ese documento.");

        errores.AgregarSiHayError(Validador.Telefono(m.Telefono));
        errores.AgregarSiHayError(Validador.Correo(m.Correo));

        if (m.FechaIngreso > DateTime.Today)
            errores.Add("La fecha de ingreso no puede ser futura.");

        return errores;
    }

    public void Guardar(Miembro m)
    {
        SesionActual.Exigir(Permisos.MiembrosEditar);
        bool esNuevo = m.Id == 0;
        var row = new MiembroRow
        {
            Id = m.Id,
            Nombre = m.Nombre.Trim(),
            Documento = m.Documento.Trim(),
            Telefono = m.Telefono.Trim(),
            Correo = m.Correo.Trim(),
            FechaIngreso = m.FechaIngreso,
            Activo = m.Activo
        };

        if (m.Id > 0) _dal.Actualizar(row);
        else m.Id = _dal.Insertar(row);

        _auditoria.Registrar(esNuevo ? "MIEMBRO_CREADO" : "MIEMBRO_ACTUALIZADO", "Miembro", m.Id,
            $"{row.Nombre} (doc. {row.Documento})");
    }

    public void Eliminar(int id)
    {
        SesionActual.Exigir(Permisos.MiembrosEditar);
        var antes = _dal.ObtenerPorId(id);
        _dal.Eliminar(id);
        _auditoria.Registrar("MIEMBRO_ELIMINADO", "Miembro", id, antes?.Nombre);
    }

    private Dictionary<int, decimal> ObtenerPrecioUltimoPlanPorMiembro()
    {
        // Trae la ultima inscripcion de cada miembro para saber su cuota vigente (para CalcularCosto()).
        var inscripciones = new InscripcionDAL().ObtenerTodos();
        var resultado = new Dictionary<int, decimal>();
        foreach (var i in inscripciones.OrderByDescending(x => x.FechaInscripcion))
        {
            if (!resultado.ContainsKey(i.MiembroId))
                resultado[i.MiembroId] = i.Total;
        }
        return resultado;
    }
}
