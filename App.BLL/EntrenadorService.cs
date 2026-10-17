using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public class EntrenadorService
{
    private readonly EntrenadorDAL _dal = new();

    public List<Entrenador> ObtenerTodos(bool soloActivos = false)
    {
        var lista = new List<Entrenador>();
        foreach (var r in _dal.ObtenerTodos(soloActivos))
        {
            lista.Add(new Entrenador(r.Nombre, r.Documento, r.Telefono, r.Especialidad, r.TarifaHora)
            {
                Id = r.Id,
                Activo = r.Activo
            });
        }
        return lista;
    }

    public List<string> Validar(Entrenador e)
    {
        var errores = new List<string>();
        errores.AgregarSiHayError(Validador.Nombre(e.Nombre));

        string? errorDocumento = Validador.Documento(e.Documento);
        if (errorDocumento != null) errores.Add(errorDocumento);
        else if (_dal.ExisteDocumento(e.Documento, e.Id))
            errores.Add("Ya existe un entrenador con ese documento.");

        if (!string.IsNullOrWhiteSpace(e.Telefono))
            errores.AgregarSiHayError(Validador.Telefono(e.Telefono));

        if (string.IsNullOrWhiteSpace(e.Especialidad))
            errores.Add("La especialidad es obligatoria.");

        if (e.TarifaHora < 0)
            errores.Add("La tarifa por hora no puede ser negativa.");

        return errores;
    }

    public void Guardar(Entrenador e)
    {
        SesionActual.Exigir(Permisos.EntrenadoresEditar);
        bool esNuevo = e.Id == 0;
        var row = new EntrenadorRow
        {
            Id = e.Id,
            Nombre = e.Nombre.Trim(),
            Documento = e.Documento.Trim(),
            Telefono = e.Telefono.Trim(),
            Especialidad = e.Especialidad.Trim(),
            TarifaHora = e.TarifaHora,
            Activo = e.Activo
        };
        if (e.Id > 0) _dal.Actualizar(row);
        else e.Id = _dal.Insertar(row);

        new AuditoriaService().Registrar(esNuevo ? "ENTRENADOR_CREADO" : "ENTRENADOR_ACTUALIZADO", "Entrenador", e.Id, row.Nombre);
    }

    public void Eliminar(int id)
    {
        SesionActual.Exigir(Permisos.EntrenadoresEditar);
        _dal.Eliminar(id);
        new AuditoriaService().Registrar("ENTRENADOR_ELIMINADO", "Entrenador", id, null);
    }
}
