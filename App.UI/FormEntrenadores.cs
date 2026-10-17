using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

public partial class FormEntrenadores : Form
{
    private System.Windows.Forms.Label lblNombre;
    private System.Windows.Forms.Label lblDocumento;
    private System.Windows.Forms.Label lblTelefono;
    private System.Windows.Forms.Label lblEspecialidad;
    private System.Windows.Forms.Label lblTarifaHora;
    private System.Windows.Forms.Label lblHoras;
    private System.Windows.Forms.TextBox txtNombre;
    private System.Windows.Forms.MaskedTextBox maskedDocumento;
    private System.Windows.Forms.MaskedTextBox maskedTelefono;
    private System.Windows.Forms.ComboBox cmbEspecialidad;
    private System.Windows.Forms.NumericUpDown numTarifaHora;
    private System.Windows.Forms.NumericUpDown numHoras;
    private System.Windows.Forms.CheckBox chkActivo;
    private System.Windows.Forms.Button btnGuardar;
    private System.Windows.Forms.Button btnEditar;
    private System.Windows.Forms.Button btnEliminar;
    private System.Windows.Forms.Button btnLimpiar;
    private System.Windows.Forms.DataGridView dgvEntrenadores;
    private System.Windows.Forms.Label lblBuscar;
    private System.Windows.Forms.TextBox txtBuscar;
    private System.Windows.Forms.Button btnBuscar;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.GroupBox groupBox1;

    private readonly EntrenadorService _service = new();
    private readonly bool _soloLectura;
    private int _idSeleccionado = -1;

    private static readonly string[] EspecialidadesBase =
        { "Crossfit", "Yoga", "Spinning", "BodyPump", "Personal", "Nutricion" };

    public FormEntrenadores(bool soloLectura = false)
    {
        InitializeComponent();
        this.lblNombre = new System.Windows.Forms.Label();
        this.lblDocumento = new System.Windows.Forms.Label();
        this.lblTelefono = new System.Windows.Forms.Label();
        this.lblEspecialidad = new System.Windows.Forms.Label();
        this.lblTarifaHora = new System.Windows.Forms.Label();
        this.lblHoras = new System.Windows.Forms.Label();
        this.txtNombre = new System.Windows.Forms.TextBox();
        this.maskedDocumento = new System.Windows.Forms.MaskedTextBox();
        this.maskedTelefono = new System.Windows.Forms.MaskedTextBox();
        this.cmbEspecialidad = new System.Windows.Forms.ComboBox();
        this.numTarifaHora = new System.Windows.Forms.NumericUpDown();
        this.numHoras = new System.Windows.Forms.NumericUpDown();
        this.chkActivo = new System.Windows.Forms.CheckBox();
        this.btnGuardar = new System.Windows.Forms.Button();
        this.btnEditar = new System.Windows.Forms.Button();
        this.btnEliminar = new System.Windows.Forms.Button();
        this.btnLimpiar = new System.Windows.Forms.Button();
        this.dgvEntrenadores = new System.Windows.Forms.DataGridView();
        this.lblBuscar = new System.Windows.Forms.Label();
        this.txtBuscar = new System.Windows.Forms.TextBox();
        this.btnBuscar = new System.Windows.Forms.Button();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        ((System.ComponentModel.ISupportInitialize)(this.numTarifaHora)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numHoras)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvEntrenadores)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.groupBox1.SuspendLayout();
        this.SuspendLayout();

        this.groupBox1.Text = "Datos del Entrenador";
        this.groupBox1.Location = new System.Drawing.Point(12, 12);
        this.groupBox1.Size = new System.Drawing.Size(700, 150);

        this.lblNombre.AutoSize = true;
        this.lblNombre.Location = new System.Drawing.Point(15, 30);
        this.lblNombre.Text = "Nombre";
        this.txtNombre.Location = new System.Drawing.Point(110, 27);
        this.txtNombre.Size = new System.Drawing.Size(200, 20);

        this.lblDocumento.AutoSize = true;
        this.lblDocumento.Location = new System.Drawing.Point(330, 30);
        this.lblDocumento.Text = "Documento";
        this.maskedDocumento.Location = new System.Drawing.Point(420, 27);
        this.maskedDocumento.Size = new System.Drawing.Size(130, 20);
        this.maskedDocumento.Mask = "0000000000";

        this.lblTelefono.AutoSize = true;
        this.lblTelefono.Location = new System.Drawing.Point(15, 62);
        this.lblTelefono.Text = "Telefono";
        this.maskedTelefono.Location = new System.Drawing.Point(110, 59);
        this.maskedTelefono.Size = new System.Drawing.Size(150, 20);
        this.maskedTelefono.Mask = "(000) 000-0000";

        this.lblEspecialidad.AutoSize = true;
        this.lblEspecialidad.Location = new System.Drawing.Point(330, 62);
        this.lblEspecialidad.Text = "Especialidad";
        this.cmbEspecialidad.Location = new System.Drawing.Point(420, 59);
        this.cmbEspecialidad.Size = new System.Drawing.Size(160, 20);
        this.cmbEspecialidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;

        this.lblTarifaHora.AutoSize = true;
        this.lblTarifaHora.Location = new System.Drawing.Point(15, 94);
        this.lblTarifaHora.Text = "Tarifa/hora";
        this.numTarifaHora.Location = new System.Drawing.Point(110, 91);
        this.numTarifaHora.Size = new System.Drawing.Size(110, 20);
        this.numTarifaHora.ThousandsSeparator = true;

        this.lblHoras.AutoSize = true;
        this.lblHoras.Location = new System.Drawing.Point(330, 94);
        this.lblHoras.Text = "Horas/mes";
        this.numHoras.Location = new System.Drawing.Point(420, 91);
        this.numHoras.Size = new System.Drawing.Size(80, 20);

        this.chkActivo.AutoSize = true;
        this.chkActivo.Location = new System.Drawing.Point(600, 94);
        this.chkActivo.Text = "Activo";

        this.btnGuardar.Location = new System.Drawing.Point(15, 118);
        this.btnGuardar.Size = new System.Drawing.Size(90, 26);
        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

        this.btnEditar.Location = new System.Drawing.Point(112, 118);
        this.btnEditar.Size = new System.Drawing.Size(90, 26);
        this.btnEditar.Text = "Editar";
        this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);

        this.btnEliminar.Location = new System.Drawing.Point(209, 118);
        this.btnEliminar.Size = new System.Drawing.Size(90, 26);
        this.btnEliminar.Text = "Eliminar";
        this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

        this.btnLimpiar.Location = new System.Drawing.Point(306, 118);
        this.btnLimpiar.Size = new System.Drawing.Size(90, 26);
        this.btnLimpiar.Text = "Limpiar";
        this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

        this.groupBox1.Controls.Add(this.lblNombre);
        this.groupBox1.Controls.Add(this.txtNombre);
        this.groupBox1.Controls.Add(this.lblDocumento);
        this.groupBox1.Controls.Add(this.maskedDocumento);
        this.groupBox1.Controls.Add(this.lblTelefono);
        this.groupBox1.Controls.Add(this.maskedTelefono);
        this.groupBox1.Controls.Add(this.lblEspecialidad);
        this.groupBox1.Controls.Add(this.cmbEspecialidad);
        this.groupBox1.Controls.Add(this.lblTarifaHora);
        this.groupBox1.Controls.Add(this.numTarifaHora);
        this.groupBox1.Controls.Add(this.lblHoras);
        this.groupBox1.Controls.Add(this.numHoras);
        this.groupBox1.Controls.Add(this.chkActivo);
        this.groupBox1.Controls.Add(this.btnGuardar);
        this.groupBox1.Controls.Add(this.btnEditar);
        this.groupBox1.Controls.Add(this.btnEliminar);
        this.groupBox1.Controls.Add(this.btnLimpiar);

        this.lblBuscar.AutoSize = true;
        this.lblBuscar.Location = new System.Drawing.Point(15, 177);
        this.lblBuscar.Text = "Buscar (nombre, documento o especialidad)";
        this.txtBuscar.Location = new System.Drawing.Point(270, 174);
        this.txtBuscar.Size = new System.Drawing.Size(220, 20);
        this.btnBuscar.Location = new System.Drawing.Point(500, 172);
        this.btnBuscar.Size = new System.Drawing.Size(90, 26);
        this.btnBuscar.Text = "Buscar";
        this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

        this.dgvEntrenadores.Location = new System.Drawing.Point(12, 204);
        this.dgvEntrenadores.Size = new System.Drawing.Size(700, 320);
        this.dgvEntrenadores.ReadOnly = true;
        this.dgvEntrenadores.AllowUserToAddRows = false;
        this.dgvEntrenadores.RowHeadersVisible = false;
        this.dgvEntrenadores.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvEntrenadores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvEntrenadores.DoubleClick += new System.EventHandler(this.dgvEntrenadores_DoubleClick);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(724, 536);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.lblBuscar);
        this.Controls.Add(this.txtBuscar);
        this.Controls.Add(this.btnBuscar);
        this.Controls.Add(this.dgvEntrenadores);
        this.Name = "FormEntrenadores";
        this.Text = "Gestion de Entrenadores";
        ((System.ComponentModel.ISupportInitialize)(this.numTarifaHora)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numHoras)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvEntrenadores)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.ResumeLayout(false);
    
        _soloLectura = soloLectura;

        cmbEspecialidad.Items.AddRange(EspecialidadesBase);
        numTarifaHora.Maximum = 1_000_000;
        numTarifaHora.Increment = 1000;
        numHoras.Maximum = 300;
        numHoras.Value = 160;

        toolTip1.SetToolTip(txtNombre, "Nombre completo del entrenador");
        toolTip1.SetToolTip(maskedDocumento, "Numero de documento");
        toolTip1.SetToolTip(maskedTelefono, "Telefono de contacto");
        toolTip1.SetToolTip(cmbEspecialidad, "Especialidad principal");
        toolTip1.SetToolTip(numTarifaHora, "Tarifa por hora en pesos");
        toolTip1.SetToolTip(numHoras, "Horas mensuales estimadas de trabajo (usadas para calcular el pago)");

        chkActivo.Checked = true;

        if (_soloLectura)
        {
            btnGuardar.Enabled = false;
            btnEliminar.Enabled = false;
            Text += " (solo lectura)";
        }

        CargarGrid();
    }

    private void CargarGrid(string filtro = "")
    {
        var lista = _service.ObtenerTodos();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            lista = lista.Where(e =>
                e.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                e.Documento.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                e.Especialidad.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        dgvEntrenadores.DataSource = lista.Select(e => new
        {
            e.Id,
            e.Nombre,
            e.Documento,
            e.Telefono,
            e.Especialidad,
            e.TarifaHora,
            PagoMensualEstimado = e.CalcularCosto(), // polimorfismo: Entrenador.CalcularCosto()
            e.Activo
        }).ToList();
    }

    private void btnGuardar_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        var entrenador = new Entrenador(
            txtNombre.Text.Trim(),
            maskedDocumento.Text.Trim(),
            maskedTelefono.Text.Trim(),
            cmbEspecialidad.Text.Trim(),
            numTarifaHora.Value)
        {
            Id = _idSeleccionado > 0 ? _idSeleccionado : 0,
            HorasMensualesEstimadas = (int)numHoras.Value,
            Activo = chkActivo.Checked
        };

        var errores = _service.Validar(entrenador);
        if (errores.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(entrenador.Nombre)) errorProvider1.SetError(txtNombre, "Requerido");
            if (string.IsNullOrWhiteSpace(entrenador.Documento)) errorProvider1.SetError(maskedDocumento, "Requerido");
            if (string.IsNullOrWhiteSpace(entrenador.Especialidad)) errorProvider1.SetError(cmbEspecialidad, "Requerido");
            MessageBox.Show("Corrija los siguientes datos:\n\n- " + string.Join("\n- ", errores),
                "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _service.Guardar(entrenador);
        MessageBox.Show("Entrenador guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarGrid();
    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
        if (dgvEntrenadores.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un entrenador para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(dgvEntrenadores.CurrentRow.Cells["Id"].Value);
        var ent = _service.ObtenerTodos().FirstOrDefault(x => x.Id == id);
        if (ent == null) return;

        _idSeleccionado = ent.Id;
        txtNombre.Text = ent.Nombre;
        maskedDocumento.Text = ent.Documento;
        maskedTelefono.Text = ent.Telefono;
        cmbEspecialidad.Text = ent.Especialidad;
        numTarifaHora.Value = ent.TarifaHora;
        numHoras.Value = ent.HorasMensualesEstimadas;
        chkActivo.Checked = ent.Activo;
    }

    private void btnEliminar_Click(object sender, EventArgs e)
    {
        if (dgvEntrenadores.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un entrenador para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("¿Desea eliminar este entrenador?", "Confirmacion",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        int id = Convert.ToInt32(dgvEntrenadores.CurrentRow.Cells["Id"].Value);
        try
        {
            _service.Eliminar(id);
            LimpiarFormulario();
            CargarGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo eliminar el entrenador (puede tener inscripciones asociadas).\n\n" + ex.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        txtNombre.Clear();
        maskedDocumento.Clear();
        maskedTelefono.Clear();
        cmbEspecialidad.SelectedIndex = -1;
        cmbEspecialidad.Text = string.Empty;
        numTarifaHora.Value = 0;
        numHoras.Value = 160;
        chkActivo.Checked = true;
        errorProvider1.Clear();
    }

    private void dgvEntrenadores_DoubleClick(object sender, EventArgs e) => btnEditar_Click(sender, e);
}
