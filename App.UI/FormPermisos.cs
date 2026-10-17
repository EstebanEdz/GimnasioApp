using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

/// <summary>Matriz editable de permisos por perfil (solo Administrador).</summary>
public partial class FormPermisos : Form
{
    private System.Windows.Forms.Label lblPerfil;
    private System.Windows.Forms.ComboBox cmbPerfil;
    private System.Windows.Forms.CheckedListBox clbPermisos;
    private System.Windows.Forms.Button btnGuardar;
    private System.Windows.Forms.Button btnDescartar;
    private System.Windows.Forms.Label lblInfo;

    private readonly UsuarioService _usuarioService = new();
    private readonly PermisoService _permisoService = new();
    private List<Permiso> _permisos = new();
    private bool _cargando;

    private static readonly string[] ProtegidosAdmin = { Permisos.UsuariosAdmin, Permisos.PermisosAdmin };

    public FormPermisos()
    {
        InitializeComponent();
        this.lblPerfil = new System.Windows.Forms.Label();
        this.cmbPerfil = new System.Windows.Forms.ComboBox();
        this.clbPermisos = new System.Windows.Forms.CheckedListBox();
        this.btnGuardar = new System.Windows.Forms.Button();
        this.btnDescartar = new System.Windows.Forms.Button();
        this.lblInfo = new System.Windows.Forms.Label();
        this.SuspendLayout();

        this.lblPerfil.AutoSize = true;
        this.lblPerfil.Location = new System.Drawing.Point(12, 17);
        this.lblPerfil.Text = "Perfil";
        this.cmbPerfil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbPerfil.Location = new System.Drawing.Point(55, 13);
        this.cmbPerfil.Size = new System.Drawing.Size(180, 21);
        this.cmbPerfil.SelectedIndexChanged += new System.EventHandler(this.cmbPerfil_SelectedIndexChanged);

        this.clbPermisos.Location = new System.Drawing.Point(12, 46);
        this.clbPermisos.Size = new System.Drawing.Size(500, 330);
        this.clbPermisos.CheckOnClick = true;
        this.clbPermisos.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbPermisos_ItemCheck);

        this.btnGuardar.Location = new System.Drawing.Point(12, 388);
        this.btnGuardar.Size = new System.Drawing.Size(120, 30);
        this.btnGuardar.Text = "Guardar cambios";
        this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

        this.btnDescartar.Location = new System.Drawing.Point(140, 388);
        this.btnDescartar.Size = new System.Drawing.Size(120, 30);
        this.btnDescartar.Text = "Descartar";
        this.btnDescartar.Click += new System.EventHandler(this.btnDescartar_Click);

        this.lblInfo.AutoSize = true;
        this.lblInfo.MaximumSize = new System.Drawing.Size(500, 0);
        this.lblInfo.ForeColor = System.Drawing.Color.DimGray;
        this.lblInfo.Location = new System.Drawing.Point(12, 428);
        this.lblInfo.Text = "Los cambios se aplican a cada usuario la proxima vez que inicie sesion. El perfil Administrador siempre conserva los permisos de gestionar usuarios y permisos.";

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(526, 480);
        this.Controls.Add(this.lblPerfil);
        this.Controls.Add(this.cmbPerfil);
        this.Controls.Add(this.clbPermisos);
        this.Controls.Add(this.btnGuardar);
        this.Controls.Add(this.btnDescartar);
        this.Controls.Add(this.lblInfo);
        this.Name = "FormPermisos";
        this.Text = "Permisos por Perfil (Administrador)";
        this.ResumeLayout(false);
        this.PerformLayout();

        try
        {
            _permisos = _permisoService.ObtenerTodos();
            foreach (var p in _usuarioService.ObtenerPerfiles()) cmbPerfil.Items.Add(p);
            if (cmbPerfil.Items.Count > 0) cmbPerfil.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Permisos.Cargar");
        }
    }

    private bool PerfilEsAdministrador => cmbPerfil.SelectedItem is Perfil { Nombre: "Administrador" };

    private void cmbPerfil_SelectedIndexChanged(object? sender, EventArgs e) => CargarPermisosDelPerfil();

    private void CargarPermisosDelPerfil()
    {
        if (cmbPerfil.SelectedItem is not Perfil perfil) return;

        try
        {
            var asignados = _permisoService.ObtenerCodigosPorPerfil(perfil.Id);
            _cargando = true;
            clbPermisos.Items.Clear();
            foreach (var permiso in _permisos)
                clbPermisos.Items.Add(permiso, asignados.Contains(permiso.Codigo));
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Permisos.CargarPerfil");
        }
        finally
        {
            _cargando = false;
        }
    }

    // Evita que se desmarquen los permisos que el Administrador necesita para no quedarse sin acceso.
    private void clbPermisos_ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (_cargando || !PerfilEsAdministrador) return;
        if (clbPermisos.Items[e.Index] is Permiso p && ProtegidosAdmin.Contains(p.Codigo) && e.NewValue == CheckState.Unchecked)
        {
            e.NewValue = CheckState.Checked;
        }
    }

    private void btnGuardar_Click(object? sender, EventArgs e)
    {
        if (cmbPerfil.SelectedItem is not Perfil perfil) return;

        var codigos = clbPermisos.CheckedItems.Cast<Permiso>().Select(p => p.Codigo).ToList();
        if (!Mensajes.Confirmar(this, $"¿Guardar {codigos.Count} permisos para el perfil {perfil.Nombre}?")) return;

        try
        {
            _permisoService.Guardar(perfil.Id, codigos);
            string extra = SesionActual.Usuario?.PerfilId == perfil.Id
                ? "\n\nComo es su propio perfil, vera los cambios al volver a iniciar sesion."
                : string.Empty;
            Mensajes.Info(this, "Permisos actualizados." + extra);
            CargarPermisosDelPerfil();
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Permisos.Guardar");
        }
    }

    private void btnDescartar_Click(object? sender, EventArgs e) => CargarPermisosDelPerfil();
}
