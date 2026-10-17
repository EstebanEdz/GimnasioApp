using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public class PlanService
{
    private readonly PlanDAL _dal = new();

    public List<Plan> ObtenerTodos(bool soloActivos = false)
    {
        var lista = new List<Plan>();
        foreach (var r in _dal.ObtenerTodos(soloActivos))
        {
            lista.Add(new Plan
            {
                Id = r.Id,
                Nombre = r.Nombre,
                TipoDuracion = r.TipoDuracion,
                Precio = r.Precio,
                IncluyeClases = r.IncluyeClases,
                Activo = r.Activo
            });
        }
        return lista;
    }

    public Plan? ObtenerPorId(int id)
    {
        var r = _dal.ObtenerPorId(id);
        if (r == null) return null;
        return new Plan { Id = r.Id, Nombre = r.Nombre, TipoDuracion = r.TipoDuracion, Precio = r.Precio, IncluyeClases = r.IncluyeClases, Activo = r.Activo };
    }

    public List<string> Validar(Plan p)
    {
        var errores = new List<string>();
        if (string.IsNullOrWhiteSpace(p.Nombre))
            errores.Add("El nombre del plan es obligatorio.");
        else if (_dal.ExisteNombre(p.Nombre, p.Id))
            errores.Add("Ya existe un plan con ese nombre.");

        if (p.Precio < 0)
            errores.Add("El precio no puede ser negativo.");

        if (p.TipoDuracion is not ("Mensual" or "Trimestral" or "Anual"))
            errores.Add("Seleccione un tipo de duracion valido.");

        return errores;
    }

    public void Guardar(Plan p)
    {
        SesionActual.Exigir(Permisos.PlanesEditar);
        bool esNuevo = p.Id == 0;
        var row = new PlanRow
        {
            Id = p.Id,
            Nombre = p.Nombre.Trim(),
            TipoDuracion = p.TipoDuracion,
            Precio = p.Precio,
            IncluyeClases = p.IncluyeClases,
            Activo = p.Activo
        };
        if (p.Id > 0) _dal.Actualizar(row);
        else p.Id = _dal.Insertar(row);

        new AuditoriaService().Registrar(esNuevo ? "PLAN_CREADO" : "PLAN_ACTUALIZADO", "Plan", p.Id,
            $"{row.Nombre} - ${row.Precio:N0}");
    }

    public void Eliminar(int id)
    {
        SesionActual.Exigir(Permisos.PlanesEditar);
        _dal.Eliminar(id);
        new AuditoriaService().Registrar("PLAN_ELIMINADO", "Plan", id, null);
    }
}
