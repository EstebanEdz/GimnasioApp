using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

public partial class FormUsuarios : Form
{
    private System.Windows.Forms.Label lblNombreUsuario;
    private System.Windows.Forms.Label lblContrasena;
    private System.Windows.Forms.Label lblPerfil;
    private System.Windows.Forms.Label lblPreguntaSeguridad;
    private System.Windows.Forms.Label lblRespuestaSeguridad;
    private System.Windows.Forms.TextBox txtPreguntaSeguridad;
    private System.Windows.Forms.TextBox txtRespuestaSeguridad;
    private System.Windows.Forms.TextBox txtNombreUsuario;
    private System.Windows.Forms.TextBox txtContrasena;
    private System.Windows.Forms.ComboBox cmbPerfil;
    private System.Windows.Forms.CheckBox chkActivo;
    private System.Windows.Forms.Button btnGuardar;
    private System.Windows.Forms.Button btnEditar;
    private System.Windows.Forms.Button btnEliminar;
    private System.Windows.Forms.Button btnLimpiar;
    private System.Windows.Forms.Button btnDesbloquear;
    private System.Windows.Forms.DataGridView dgvUsuarios;
    private System.Windows.Forms.Label lblBuscar;
    private System.Windows.Forms.TextBox txtBuscar;
    private System.Windows.Forms.Button btnBuscar;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.GroupBox groupBox1;

    private readonly UsuarioService _service = new();
    private int _idSeleccionado = -1;

    private class ItemPerfil
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public override string ToString() => Nombre;
    }

    public FormUsuarios()
    {
        InitializeComponent();
        this.lblNombreUsuario = new System.Windows.Forms.Label();
        this.lblContrasena = new System.Windows.Forms.Label();
        this.lblPerfil = new System.Windows.Forms.Label();
        this.lblPreguntaSeguridad = new System.Windows.Forms.Label();
        this.lblRespuestaSeguridad = new System.Windows.Forms.Label();
        this.txtPreguntaSeguridad = new System.Windows.Forms.TextBox();
        this.txtRespuestaSeguridad = new System.Windows.Forms.TextBox();
        this.txtNombreUsuario = new System.Windows.Forms.TextBox();
        this.txtContrasena = new System.Windows.Forms.TextBox();
        this.cmbPerfil = new System.Windows.Forms.ComboBox();
        this.chkActivo = new System.Windows.Forms.CheckBox();
        this.btnGuardar = new System.Windows.Forms.Button();
        this.btnEditar = new System.Windows.Forms.Button();
        this.btnEliminar = new System.Windows.Forms.Button();
        this.btnLimpiar = new System.Windows.Forms.Button();
        this.btnDesbloquear = new System.Windows.Forms.Button();
        this.dgvUsuarios = new System.Windows.Forms.DataGridView();
        this.lblBuscar = new System.Windows.Forms.Label();
        this.txtBuscar = new System.Windows.Forms.TextBox();
        this.btnBuscar = new System.Windows.Forms.Button();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.groupBox1.SuspendLayout();
        this.SuspendLayout();

        this.groupBox1.Text = "Datos del Usuario";
        this.groupBox1.Location = new System.Drawing.Point(12, 12);
        this.groupBox1.Size = new System.Drawing.Size(560, 200);

        this.lblNombreUsuario.AutoSize = true;
        this.lblNombreUsuario.Location = new System.Drawing.Point(15, 30);
        this.lblNombreUsuario.Text = "Usuario";
        this.txtNombreUsuario.Location = new System.Drawing.Point(110, 27);
        this.txtNombreUsuario.Size = new System.Drawing.Size(200, 20);

        this.lblContrasena.AutoSize = true;
        this.lblContrasena.Location = new System.Drawing.Point(330, 30);
        this.lblContrasena.Text = "Contraseña";
        this.txtContrasena.Location = new System.Drawing.Point(410, 27);
        this.txtContrasena.Size = new System.Drawing.Size(130, 20);
        this.txtContrasena.PasswordChar = '*';

        this.lblPerfil.AutoSize = true;
        this.lblPerfil.Location = new System.Drawing.Point(15, 62);
        this.lblPerfil.Text = "Perfil";
        this.cmbPerfil.Location = new System.Drawing.Point(110, 59);
        this.cmbPerfil.Size = new System.Drawing.Size(200, 21);
        this.cmbPerfil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

        this.chkActivo.AutoSize = true;
        this.chkActivo.Location = new System.Drawing.Point(330, 62);
        this.chkActivo.Text = "Activo";

        this.lblPreguntaSeguridad.AutoSize = true;
        this.lblPreguntaSeguridad.Location = new System.Drawing.Point(15, 94);
        this.lblPreguntaSeguridad.Text = "Pregunta seguridad";
        this.txtPreguntaSeguridad.Location = new System.Drawing.Point(150, 91);
        this.txtPreguntaSeguridad.Size = new System.Drawing.Size(390, 20);

        this.lblRespuestaSeguridad.AutoSize = true;
        this.lblRespuestaSeguridad.Location = new System.Drawing.Point(15, 126);
        this.lblRespuestaSeguridad.Text = "Respuesta";
        this.txtRespuestaSeguridad.Location = new System.Drawing.Point(150, 123);
        this.txtRespuestaSeguridad.Size = new System.Drawing.Size(390, 20);

        this.btnGuardar.Location = new System.Drawing.Point(15, 160);
        this.btnGuardar.Size = new System.Drawing.Size(90, 26);
        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

        this.btnEditar.Location = new System.Drawing.Point(112, 160);
        this.btnEditar.Size = new System.Drawing.Size(90, 26);
        this.btnEditar.Text = "Editar";
        this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);

        this.btnEliminar.Location = new System.Drawing.Point(209, 160);
        this.btnEliminar.Size = new System.Drawing.Size(90, 26);
        this.btnEliminar.Text = "Eliminar";
        this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

        this.btnLimpiar.Location = new System.Drawing.Point(306, 160);
        this.btnLimpiar.Size = new System.Drawing.Size(90, 26);
        this.btnLimpiar.Text = "Limpiar";
        this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

        this.groupBox1.Controls.Add(this.lblNombreUsuario);
        this.groupBox1.Controls.Add(this.txtNombreUsuario);
        this.groupBox1.Controls.Add(this.lblContrasena);
        this.groupBox1.Controls.Add(this.txtContrasena);
        this.groupBox1.Controls.Add(this.lblPerfil);
        this.groupBox1.Controls.Add(this.cmbPerfil);
        this.groupBox1.Controls.Add(this.chkActivo);
        this.groupBox1.Controls.Add(this.lblPreguntaSeguridad);
        this.groupBox1.Controls.Add(this.txtPreguntaSeguridad);
        this.groupBox1.Controls.Add(this.lblRespuestaSeguridad);
        this.groupBox1.Controls.Add(this.txtRespuestaSeguridad);
        this.groupBox1.Controls.Add(this.btnGuardar);
        this.groupBox1.Controls.Add(this.btnEditar);
        this.groupBox1.Controls.Add(this.btnEliminar);
        this.btnDesbloquear.Location = new System.Drawing.Point(403, 160);
        this.btnDesbloquear.Size = new System.Drawing.Size(100, 26);
        this.btnDesbloquear.Text = "Desbloquear";
        this.btnDesbloquear.Click += new System.EventHandler(this.btnDesbloquear_Click);

        this.groupBox1.Controls.Add(this.btnLimpiar);
        this.groupBox1.Controls.Add(this.btnDesbloquear);

        this.lblBuscar.AutoSize = true;
        this.lblBuscar.Location = new System.Drawing.Point(15, 227);
        this.lblBuscar.Text = "Buscar (usuario o perfil)";
        this.txtBuscar.Location = new System.Drawing.Point(160, 224);
        this.txtBuscar.Size = new System.Drawing.Size(200, 20);
        this.btnBuscar.Location = new System.Drawing.Point(370, 222);
        this.btnBuscar.Size = new System.Drawing.Size(90, 26);
        this.btnBuscar.Text = "Buscar";
        this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

        this.dgvUsuarios.Location = new System.Drawing.Point(12, 254);
        this.dgvUsuarios.Size = new System.Drawing.Size(560, 260);
        this.dgvUsuarios.ReadOnly = true;
        this.dgvUsuarios.AllowUserToAddRows = false;
        this.dgvUsuarios.RowHeadersVisible = false;
        this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvUsuarios.DoubleClick += new System.EventHandler(this.dgvUsuarios_DoubleClick);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 526);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.lblBuscar);
        this.Controls.Add(this.txtBuscar);
        this.Controls.Add(this.btnBuscar);
        this.Controls.Add(this.dgvUsuarios);
        this.Name = "FormUsuarios";
        this.Text = "Gestion de Usuarios (Administrador)";
        ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.ResumeLayout(false);
    

        toolTip1.SetToolTip(txtNombreUsuario, "Nombre de usuario para iniciar sesion");
        toolTip1.SetToolTip(txtContrasena, "Deje en blanco para no cambiar la contraseña al editar");
        toolTip1.SetToolTip(cmbPerfil, "Perfil de acceso (Administrador / Operador / Consulta)");
        toolTip1.SetToolTip(txtPreguntaSeguridad, "Pregunta usada para recuperar la contraseña");
        toolTip1.SetToolTip(txtRespuestaSeguridad, "Deje en blanco para no cambiar la respuesta al editar");

        foreach (var p in _service.ObtenerPerfiles())
            cmbPerfil.Items.Add(new ItemPerfil { Id = p.Id, Nombre = p.Nombre });

        chkActivo.Checked = true;
        CargarGrid();
    }

    private void CargarGrid(string filtro = "")
    {
        var lista = _service.ObtenerTodos();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            lista = lista.Where(u =>
                u.NombreUsuario.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                u.PerfilNombre.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        dgvUsuarios.DataSource = lista.Select(u => new
        {
            u.Id,
            u.NombreUsuario,
            Perfil = u.PerfilNombre,
            u.Activo,
            Bloqueado = u.Bloqueado ? "Si" : "",
            UltimoAcceso = u.UltimoAcceso?.ToString("yyyy-MM-dd HH:mm") ?? "-"
        }).ToList();
    }

    private void btnGuardar_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (cmbPerfil.SelectedItem is not ItemPerfil perfilSel)
        {
            MessageBox.Show("Seleccione un perfil.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var usuario = new Usuario
        {
            Id = _idSeleccionado > 0 ? _idSeleccionado : 0,
            NombreUsuario = txtNombreUsuario.Text.Trim(),
            PerfilId = perfilSel.Id,
            Activo = chkActivo.Checked
        };

        string? nuevaContrasena = string.IsNullOrWhiteSpace(txtContrasena.Text) ? null : txtContrasena.Text;
        string? nuevaPregunta = string.IsNullOrWhiteSpace(txtPreguntaSeguridad.Text) ? null : txtPreguntaSeguridad.Text;
        string? nuevaRespuesta = string.IsNullOrWhiteSpace(txtRespuestaSeguridad.Text) ? null : txtRespuestaSeguridad.Text;

        var errores = _service.Validar(usuario, nuevaContrasena);
        if (_idSeleccionado <= 0 && (nuevaPregunta == null || nuevaRespuesta == null))
            errores.Add("La pregunta y la respuesta de seguridad son obligatorias para un usuario nuevo (se usan para recuperar la contraseña).");

        if (errores.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario)) errorProvider1.SetError(txtNombreUsuario, "Requerido");
            MessageBox.Show("Corrija los siguientes datos:\n\n- " + string.Join("\n- ", errores),
                "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _service.Guardar(usuario, nuevaContrasena, nuevaPregunta, nuevaRespuesta);
        MessageBox.Show("Usuario guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarGrid();
    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
        if (dgvUsuarios.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un usuario para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells["Id"].Value);
        var u = _service.ObtenerTodos().FirstOrDefault(x => x.Id == id);
        if (u == null) return;

        _idSeleccionado = u.Id;
        txtNombreUsuario.Text = u.NombreUsuario;
        txtContrasena.Clear();
        txtPreguntaSeguridad.Text = u.PreguntaSeguridad;
        txtRespuestaSeguridad.Clear();
        chkActivo.Checked = u.Activo;

        for (int i = 0; i < cmbPerfil.Items.Count; i++)
        {
            if (cmbPerfil.Items[i] is ItemPerfil ip && ip.Id == u.PerfilId)
            {
                cmbPerfil.SelectedIndex = i;
                break;
            }
        }
    }

    private void btnEliminar_Click(object sender, EventArgs e)
    {
        if (dgvUsuarios.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un usuario para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("¿Desea eliminar este usuario?", "Confirmacion",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        int id = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells["Id"].Value);
        try
        {
            _service.Eliminar(id);
            LimpiarFormulario();
            CargarGrid();
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Usuarios.Eliminar");
        }
    }

    private void btnDesbloquear_Click(object sender, EventArgs e)
    {
        if (dgvUsuarios.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un usuario para desbloquear.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells["Id"].Value);
        try
        {
            _service.Desbloquear(id);
            CargarGrid(txtBuscar.Text.Trim());
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Usuarios.Desbloquear");
        }
    }

    private void btnBuscar_Click(object sender, EventArgs e) => CargarGrid(txtBuscar.Text.Trim());

    private void btnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        txtBuscar.Clear();
        CargarGrid();
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = -1;
        txtNombreUsuario.Clear();
        txtContrasena.Clear();
        txtPreguntaSeguridad.Clear();
        txtRespuestaSeguridad.Clear();
        cmbPerfil.SelectedIndex = -1;
        chkActivo.Checked = true;
        errorProvider1.Clear();
    }

    private void dgvUsuarios_DoubleClick(object sender, EventArgs e) => btnEditar_Click(sender, e);
}
